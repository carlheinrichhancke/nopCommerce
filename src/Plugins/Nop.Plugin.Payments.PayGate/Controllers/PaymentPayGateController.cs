using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.PayGate.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Nop.Plugin.Payments.PayGate.Controllers
{
    public class PaymentPayGateController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly PayGatePaymentSettings _payGatePaymentSettings;

        public PaymentPayGateController(IWorkContext workContext,
            IStoreService storeService, 
            ISettingService settingService, 
            IPaymentService paymentService, 
            IOrderService orderService, 
            IOrderProcessingService orderProcessingService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger, 
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            PayGatePaymentSettings payGatePaymentSettings)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._payGatePaymentSettings = payGatePaymentSettings;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payGatePaymentSettings = _settingService.LoadSetting<PayGatePaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.UseSandbox = payGatePaymentSettings.UseSandbox;
            model.PayGateID = payGatePaymentSettings.PayGateID;
            model.EncryptionKey = payGatePaymentSettings.EncryptionKey;
            model.EnableIpn = payGatePaymentSettings.EnableIpn;
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(payGatePaymentSettings, x => x.UseSandbox, storeScope);
                model.PayGateID_OverrideForStore = _settingService.SettingExists(payGatePaymentSettings, x => x.PayGateID, storeScope);
                model.EncryptionKey_OverrideForStore = _settingService.SettingExists(payGatePaymentSettings, x => x.EncryptionKey, storeScope);
                model.EnableIpn_OverrideForStore = _settingService.SettingExists(payGatePaymentSettings, x => x.EnableIpn, storeScope);

            }

            return View("~/Plugins/Payments.PayGate/Views/PaymentPayGate/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payGatePaymentSettings = _settingService.LoadSetting<PayGatePaymentSettings>(storeScope);

            //save settings
            payGatePaymentSettings.UseSandbox = model.UseSandbox;
            payGatePaymentSettings.PayGateID = model.PayGateID;
            payGatePaymentSettings.EncryptionKey = model.EncryptionKey;
            payGatePaymentSettings.EnableIpn = model.EnableIpn;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.UseSandbox_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(payGatePaymentSettings, x => x.UseSandbox, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(payGatePaymentSettings, x => x.UseSandbox, storeScope);

            if (model.PayGateID_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(payGatePaymentSettings, x => x.PayGateID, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(payGatePaymentSettings, x => x.PayGateID, storeScope);

            if (model.EncryptionKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(payGatePaymentSettings, x => x.EncryptionKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(payGatePaymentSettings, x => x.EncryptionKey, storeScope);

            if (model.EnableIpn_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(payGatePaymentSettings, x => x.EnableIpn, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(payGatePaymentSettings, x => x.EnableIpn, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.PayGate/Views/PaymentPayGate/PaymentInfo.cshtml");
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        [ValidateInput(false)]
        public ActionResult PayGateReturnHandler(FormCollection form)
        {
            string[] keys = Request.Form.AllKeys;
            String transaction_status = "";
            String pay_request_id = "";
            String transaction_status_desc = "";
            Order order = _orderService.GetOrderById(Int32.Parse(Request.QueryString["pgnopcommerce"]));
            var sBuilder = new StringBuilder();
            var query_status = PaymentStatus.Pending;
            for (int i = 0; i < keys.Length; i++){
                if (keys[i] == "TRANSACTION_STATUS") {
                    transaction_status = Request.Form[keys[i]];
                }
                
                if (keys[i] == "PAY_REQUEST_ID"){
                    pay_request_id = Request.Form[keys[i]];
                }
            }

            using (var client = new System.Net.WebClient())
            {
                var queryData = new NameValueCollection();
                queryData["PAYGATE_ID"] = _payGatePaymentSettings.PayGateID;
                queryData["PAY_REQUEST_ID"] = pay_request_id;
                queryData["REFERENCE"] = Request.QueryString["pgnopcommerce"];
                string queryValues = string.Join("", queryData.AllKeys.Select(key => queryData[key]));
                queryData["CHECKSUM"] = new PayGateHelper().CalculateMD5Hash(queryValues + _payGatePaymentSettings.EncryptionKey);
                var response = client.UploadValues("https://secure.paygate.co.za/payweb3/query.trans", queryData);

                var responseString = Encoding.Default.GetString(response);
                if(responseString != null)
                {
                   Dictionary <string, string> dict =
                   responseString.Split('&')
                   .Select(x => x.Split('='))
                   .ToDictionary(y => y[0], y => y[1]);

                    try
                    {
                        String trans_id = dict["TRANSACTION_STATUS"].ToString();
                        String query_status_desc = "";
                        switch (trans_id)
                        {
                            case "1":
                                query_status = PaymentStatus.Paid;
                                query_status_desc = "Approved";
                                break;

                            case "2":
                                query_status_desc = "Declined";
                                break;

                            case "4":
                                query_status_desc = "Cancelled By Customer with back button on payment page";
                                break;

                            case "0":
                                query_status_desc = "Not Done";
                                break;
                            default:
                                break;
                        }

                        sBuilder.AppendLine("PayGate Query Data");
                        sBuilder.AppendLine("=======================");
                        sBuilder.AppendLine("PayGate Transaction_Id: " + dict["TRANSACTION_ID"]);
                        sBuilder.AppendLine("PayGate Status Desc: " + query_status_desc);
                        sBuilder.AppendLine("");

                    } catch (Exception e)
                    {
                        sBuilder.AppendLine("PayGate Query Data");
                        sBuilder.AppendLine("=======================");
                        sBuilder.AppendLine("PayGate Query Response: " + responseString);
                        sBuilder.AppendLine("");
                    }
                    
                }
               
            }

            var new_payment_status = PaymentStatus.Pending;
            switch(transaction_status){
                case "1":
                    new_payment_status = PaymentStatus.Paid;
                    transaction_status_desc = "Approved";
                    break;

                case "2":
                    transaction_status_desc = "Declined";
                    break;

                case "4":
                    transaction_status_desc = "Cancelled By Customer with back button on payment page";
                    break;

                case "0":
                    transaction_status_desc = "Not Done";
                    break;
                default:
                    break;
            }
               
            sBuilder.AppendLine("PayGate Return Data");
            sBuilder.AppendLine("=======================");
            sBuilder.AppendLine("PayGate PayRequestId: " + pay_request_id);
            sBuilder.AppendLine("PayGate Status Desc: " + transaction_status_desc);

            //order note
            order.OrderNotes.Add(new OrderNote
            {
                Note = sBuilder.ToString(),//sbbustring.Format("Order status has been changed to {0}", PaymentStatus.Paid),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });


            _orderService.UpdateOrder(order);
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var payGatePaymentSettings = _settingService.LoadSetting<PayGatePaymentSettings>(storeScope);

            //mark order as paid
            if (query_status == PaymentStatus.Paid){
                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    order.AuthorizationTransactionId = pay_request_id;
                    _orderService.UpdateOrder(order);

                    _orderProcessingService.MarkOrderAsPaid(order);
                }
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            } else if (new_payment_status == PaymentStatus.Paid){
                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    order.AuthorizationTransactionId = pay_request_id;
                    _orderService.UpdateOrder(order);

                    _orderProcessingService.MarkOrderAsPaid(order);
                }
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            } else {
             
                    order.AuthorizationTransactionId = pay_request_id;
                    _orderService.UpdateOrder(order);

                    _orderProcessingService.CancelOrder(order,false);
            }
            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}