using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Core_Bible
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                if (!Properties.Settings.Default.SettingsUpgraded)
                {
                    Properties.Settings.Default.Upgrade();            // pull values from previous version
                    Properties.Settings.Default.SettingsUpgraded = true;
                    Properties.Settings.Default.Save();
                }
            }
            catch { /* ignore if no previous version exists */ }
            Application.Run(new MDIParentForm());
        }
    }
}
