using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.PayGate
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //PayGateReturnHandler
            routes.MapRoute("Plugin.Payments.PayGate.PayGateReturnHandler",
                 "Plugins/PaymentPayGate/PayGateReturnHandler",
                 new { controller = "PaymentPayGate", action = "PayGateReturnHandler" },
                 new[] { "Nop.Plugin.Payments.PayGate.Controllers" }
            );
            //PayGateNotifyHandler
            routes.MapRoute("Plugin.Payments.PayGate.PayGateNotifyHandler",
                 "Plugins/PaymentPayGate/PayGateNotifyHandler",
                 new { controller = "PaymentPayGate", action = "PayGateNotifyHandler" },
                 new[] { "Nop.Plugin.Payments.PayGate.Controllers" }
            );
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
