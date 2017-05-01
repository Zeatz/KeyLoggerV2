using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLoggerV2
{
    class IpAddress
    {
        public string GetIpAddress()
        {
            try
            {
                string url = "http://checkip.dyndns.org/";
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());

                string newResponse = sr.ReadToEnd().Trim();

                int startingIndex = newResponse.IndexOf(':') + 2;

                string sub = newResponse.Substring(startingIndex);

                int newStartingIndex = sub.IndexOf('<');

                string ip = sub.Substring(0, newStartingIndex);

                return ip;
            }
            catch (Exception)
            {

                return "return not available";
            }
        }
    }
}
