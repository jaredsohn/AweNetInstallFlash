// AwesomiumFlashChecker
//
// Class that checks if Flash is configured for Awesomium that can run a callback to install it.
// Written in Visual C# 2010 Express
// By Jared Sohn (jared.sohn@gmail.com, http://www.jaredsohn.com)
// github.com/jaredsohn/AweNetInstallFlash
// August 1, 2013
//
// Released under MIT License (http://www.opensource.org/licenses/mit-license.php)
//
// If including within an existing program, include both this file and the swfobject.js resource.

using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;

namespace AwesomiumFlashChecker
{
    class AwesomiumFlashChecker
    {
        private static Awesomium.Core.WebView _webView = null;
        private static Awesomium.Core.WebSession _checkFlashSession;

        private static Action _flashInstalledCallback, _flashNotInstalledCallback, _errorCallback;

        // Make sure you run this on a UI thread
        public static void CheckFlash(Action flashInstalledCallback, Action flashNotInstalledCallback, Action errorCallback)
        {
            _flashInstalledCallback = flashInstalledCallback;
            _flashNotInstalledCallback = flashNotInstalledCallback;
            _errorCallback = errorCallback;

            // Read in swfobject.  Included locally so this runs without a network connection.
            System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("AwesomiumCheckFlashChecker.Resources.swfobject.js");
            System.IO.StreamReader sr = new System.IO.StreamReader(s);
            string swfObjectCode = sr.ReadToEnd();
            
            string html = "";
            html += "<div id='flashstatus'></div>";
            //html += "<script type='text/javascript' src='http://ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js'></script>";
            html += "<script type='text/javascript'>" + swfObjectCode + "</script>";            
            html += "<script type='text/javascript'>";
            html += "document.getElementById('flashstatus').innerHTML = swfobject.hasFlashPlayerVersion('1') ? 'FLASH INSTALLED' : 'NO FLASH';";
            html += "</script>";

            try
            {
                _checkFlashSession = Awesomium.Core.WebCore.CreateWebSession(new Awesomium.Core.WebPreferences());
                _webView = Awesomium.Core.WebCore.CreateWebView(100, 100, _checkFlashSession, Awesomium.Core.WebViewType.Offscreen);
                _webView.DocumentReady += new Awesomium.Core.UrlEventHandler(webView_DocumentReady);
                
                bool success = _webView.LoadHTML(html);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private static void webView_DocumentReady(object sender, Awesomium.Core.UrlEventArgs e)
        {
            try
            {
                Awesomium.Core.JSValue val = _webView.ExecuteJavascriptWithResult("document.getElementById('flashstatus').innerHTML;");
                if (val.ToString() == "FLASH INSTALLED")
                {
                    _flashInstalledCallback();
                }
                else if (val.ToString() == "NO FLASH")
                {
                    _flashNotInstalledCallback();
                }
                else
                {
                    _errorCallback();
                }

                if (_webView != null)
                {
                    _webView.Stop();
                    _webView.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }
    }
}
