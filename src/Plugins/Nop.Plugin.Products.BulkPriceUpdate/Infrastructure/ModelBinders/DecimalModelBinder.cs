using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Products.BulkPriceUpdate.Infrastructure.ModelBinders
{
    /// <summary>
    /// A ModelBinder to force the conversion of decimal values to using the Invariant Culture.
    /// This provides support for both comma & period decimal separators.
    /// </summary>
    public class DecimalModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType == typeof(decimal)) {
                var value = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);
                if (value != null)
                    propertyDescriptor.SetValue(bindingContext.Model, Convert.ToDecimal(value.AttemptedValue, CultureInfo.InvariantCulture));
            } else {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }
    }
}
