﻿using BasicScriptingLanguageEditor.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicScriptingLanguageEditor
{
    static class Program
    {
        public static string ExecutableLocation = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            string[] args = Environment.GetCommandLineArgs();
            SingleInstanceController controller = new SingleInstanceController();;
            if (args.Length > 1 && args.Length == 2)
                controller = new SingleInstanceController(args[1]);
            else if (args.Length > 2)
                controller = new SingleInstanceController(args);
            else if (args.Length == 0)
                controller = new SingleInstanceController();
            controller.Run(args);
        }
        /*static void Main(string[] args)
        {
            if (IsApplicationRunning())
            {
                MessageBox.Show("App already running");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if(args.Length > 0)
            {
                if (args.Length == 1)
                    Application.Run(new TabbedUI(args[0]));
                else
                    Application.Run(new TabbedUI(args));
            }
            else
                Application.Run(new TabbedUI());
        }*/

        private static bool IsApplicationRunning()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        private static string appGuid = "4687d67a-10e9-47c8-91f4-16cd53e03a40";

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern void SHChangeNotify(int wEventId, int uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public static void RegisterFileAssociations()
        {
            Icon FileFormatIcon = BasicScriptingLanguageEditor.Properties.Resources.Format_BSL;

            using (FileStream fs = new FileStream(ExecutableLocation + @"\format.ico", FileMode.Create))
                FileFormatIcon.Save(fs);
            /*
            // Initializes a new AF_FileAssociator to associate the .ABC file extension.
            if (!FileAssociation.IsAssociated(".bsl"))
            {
                FileAssociation.Associate(".bsl",
                    "BSL_File",
                    "BasicScriptingLanguage script source file",
                    Environment.CurrentDirectory + @"\format.ico",
                    Assembly.GetExecutingAssembly().Location);
                MessageBox.Show("Registered file types successfully!", "BasicScriptingLanguage Editor");
            }
            else
                MessageBox.Show("File associations already registered.", "BasicScriptingLanguage Editor");*/
            string path = ExecutableLocation + "format.ico";
            Create_abc_FileAssociation(path);

            SHChangeNotify(0x08000000, 0x0000, (IntPtr)null, (IntPtr)null);//SHCNE_ASSOCCHANGED SHCNF_IDLIST

            MessageBox.Show("Attempted file assosciation successfully", "BasicScriptingLanguage Editor");
        }

        private static void Create_abc_FileAssociation(string iconPath)
        {
            /***********************************/
            /**** Key1: Create ".abc" entry ****/
            /***********************************/
            
            Microsoft.Win32.RegistryKey key1 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);


            key1.CreateSubKey("Classes");
            key1 = key1.OpenSubKey("Classes", true);

            key1.CreateSubKey(".bsl");
            key1 = key1.OpenSubKey(".bsl", true);
            key1.SetValue("", "BSLFile"); // Set default key value

            key1.Close();

            /*******************************************************/
            /**** Key2: Create "DemoKeyValue\DefaultIcon" entry ****/
            /*******************************************************/
            Microsoft.Win32.RegistryKey key2 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            key2.CreateSubKey("Classes");
            key2 = key2.OpenSubKey("Classes", true);

            key2.CreateSubKey("BSLFile");
            key2 = key2.OpenSubKey("BSLFile", true);

            key2.CreateSubKey("DefaultIcon");
            key2 = key2.OpenSubKey("DefaultIcon", true);
            key2.SetValue("", "" + iconPath + ""); // Set default key value

            key2.Close();

            /**************************************************************/
            /**** Key3: Create "DemoKeyValue\shell\open\command" entry ****/
            /**************************************************************/
            Microsoft.Win32.RegistryKey key3 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            key3.CreateSubKey("Classes");
            key3 = key3.OpenSubKey("Classes", true);

            key3.CreateSubKey("BSLFile");
            key3 = key3.OpenSubKey("BSLFile", true);
            key3.SetValue("", "BasicScriptingLanguage Source File");

            key3.CreateSubKey("shell");
            key3 = key3.OpenSubKey("shell", true);
            key3.SetValue("", "open");

            key3.CreateSubKey("open");
            key3 = key3.OpenSubKey("open", true);

            key3.SetValue("", "Open with BasicScriptingLanguage Editor");

            key3.CreateSubKey("command");
            key3 = key3.OpenSubKey("command", true);
            key3.SetValue("", "\"" + Assembly.GetExecutingAssembly().Location + "\"" + " \"%1\""); // Set default key value

            key3.Close();
            ////HKEY_CURRENT_USER\SOFTWARE\MICROSOFT\WINDOWS\CURRENTVERSION\EXPLORER\FILEEXTS\-name of your extension-
            Microsoft.Win32.RegistryKey key4 = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true);

            key4.CreateSubKey("MICROSOFT");
            key4 = key4.OpenSubKey("MICROSOFT", true);

            key4.CreateSubKey("windows");
            key4 = key4.OpenSubKey("windows", true);

            key4.CreateSubKey("currentversion");
            key4 = key4.OpenSubKey("currentversion", true);

            key4.CreateSubKey("explorer");
            key4 = key4.OpenSubKey("explorer", true);

            key4.CreateSubKey("fileexts");
            key4 = key4.OpenSubKey("fileexts", true);

            key4.CreateSubKey(".bsl");
            key4 = key4.OpenSubKey(".bsl", true);
            key4.SetValue("", "BSLFile");
            key4.Close();
        }

    }
}
