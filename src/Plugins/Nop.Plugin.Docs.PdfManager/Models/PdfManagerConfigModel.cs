using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Docs.PdfManager.Models
{
    public class PdfManagerConfigModel
    {
        [NopResourceDisplayName("Plugins.Docs.PdfManager.Fields.LogoWidth")]
        public int LogoWidth { get; set; }

        [NopResourceDisplayName("Plugins.Docs.PdfManager.Fields.LogoHeight")]
        public int LogoHeight { get; set; }

        [NopResourceDisplayName("Plugins.Docs.PdfManager.Fields.EnableVerboseLogging")]
        public bool EnableVerboseLogging { get; set; }
    }
}
