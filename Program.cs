// AweNetInstallFlash
// Commandline program to check if Flash is configured for .NET Awesomium and installs it if not
// Written in Visual C# 2010 Express
// By Jared Sohn (jared.sohn@gmail.com, http://www.jaredsohn.com)
// github.com/jaredsohn/AweNetInstallFlash
// August 1, 2013
// Released under MIT License (http://www.opensource.org/licenses/mit-license.php)
//
// This program requires that .NET Awesomium libraries are installed to a shared location or the same path as this program.  In a typical installation scenario,
// this program should be run after installing your Awesomium-based product.
//
// If you want to check if Flash is installed from within your program, use AwesomiumFlashChecker.cs and see this file for example usage.
//
// If you want to include Flash as a part of your installer, fill out this form:
// http://www.adobe.com/products/players/flash-player-distribution.html, bundle the file with your installer, and include it as a commandline param when running this.
//
// If you want the user to be sent to their browser, send them to one of these sites or similar:
//
// Flash download site:
// http://get2.adobe.com/flashplayer/otherversions/
//
// A direct link (but won't age as well and assumes OS):
// http://get2.adobe.com/flashplayer/download/?installer=Flash_Player_11_for_Other_Browsers&os=Windows%207&browser_type=KHTML&browser_dist=Chrome&d=McAfee_Security_Scan_Plus_Chrome_Browser&dualoffer=false

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace AweNetInstallFlash
{
    static class Program
    {
        private static System.Windows.Forms.Timer _startTimer;
        private static System.Windows.Forms.Timer _timeoutTimer;

        private static string _filenameOrUrl = "";
        private static bool _quietMode = false;

        public static void onFlashInstalled()
        {
            _timeoutTimer.Enabled = false;
            // Don't show anything if Flash is already installed
            System.Diagnostics.Debug.WriteLine("Flash for .NET Awesomium detected.");
            Environment.Exit(0);
        }

        public static void onFlashNotInstalled()
        {
            _timeoutTimer.Enabled = false;
            if (_quietMode || (MessageBox.Show(null, "This software requires that the NPAPI build of Flash (usually for Firefox) is installed.  Do you want to install this now?", "Flash Check", MessageBoxButtons.YesNo) == DialogResult.Yes))
                System.Diagnostics.Process.Start(_filenameOrUrl);                
            Environment.Exit(1);
        }

        public static void onFlashCheckError()
        {
            _timeoutTimer.Enabled = false;
            if (!_quietMode)
                MessageBox.Show("Error found while checking Flash status.");
            Environment.Exit(2);
        }

        private static void ShowUsageAndQuit()
        {
            MessageBox.Show("Usage: AweNetInstallFlash [flashinstaller_filename_or_url] [/Q]");
            Environment.Exit(3);
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if ((args.Length != 1) && (args.Length != 2))
            {
                ShowUsageAndQuit();
            }
            if (args.Length == 2)
            {
                if (args[1] == "/Q")
                    _quietMode = true;
                else
                    ShowUsageAndQuit();
            }
            _filenameOrUrl = args[0];

            Awesomium.Core.WebConfig config = new Awesomium.Core.WebConfig();
            Awesomium.Core.WebCore.Initialize(config);

            _timeoutTimer = new System.Windows.Forms.Timer();
            _timeoutTimer.Interval = 5000; // We wait this long before giving up (in case there is no DocumentReady)
            _timeoutTimer.Tick += new EventHandler(_timeoutTimer_Tick);
            _timeoutTimer.Enabled = true;

            _startTimer = new System.Windows.Forms.Timer();
            _startTimer.Interval = 1;
            _startTimer.Tick += new EventHandler(_startTimer_Tick);
            _startTimer.Enabled = true;

            Application.Run();
        }

        static void _timeoutTimer_Tick(object sender, EventArgs e)
        {
            if (!_quietMode)
                MessageBox.Show("Timed out");
            Environment.Exit(4);
        }

        static void _startTimer_Tick(object sender, EventArgs e)
        {
            AwesomiumFlashChecker.AwesomiumFlashChecker.CheckFlash(new Action(AweNetInstallFlash.Program.onFlashInstalled), new Action(AweNetInstallFlash.Program.onFlashNotInstalled), new Action(AweNetInstallFlash.Program.onFlashCheckError));
            _startTimer.Enabled = false;
        }
    }
}