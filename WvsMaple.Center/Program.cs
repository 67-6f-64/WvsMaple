using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WvsCenter
{
    static class Program
    {
        public static string ConfigurationFile;

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                MessageBox.Show("Please include the configuration file's name as the first argument.");
                return;
            }

            ConfigurationFile = args[0] + ".img";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
