using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TechFix_Computer_Shop_System.PAL;
using TechFix_Computer_Shop_System.TechFix_Client.Admin;
using TechFix_Computer_Shop_System.TechFix_Client.Supplier;

namespace TechFix_Computer_Shop_System
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
