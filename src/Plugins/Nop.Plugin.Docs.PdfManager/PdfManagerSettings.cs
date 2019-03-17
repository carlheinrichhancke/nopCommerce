using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Docs.PdfManager
{
    public class PdfManagerSettings : ISettings
    {
        /// <summary>
        /// Gets or sets whether verbose logging is enabled for the plugin.
        /// </summary>
        public bool EnableVerboseLogging { get; set; }

        /// <summary>
        /// The width to apply to logos on PDFs
        /// </summary>
        public float LogoWidth { get; set; }

        /// <summary>
        /// The height to apply to logos on PDFs
        /// </summary>
        public float LogoHeight { get; set; }
    }
}
