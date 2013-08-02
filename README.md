AweNetInstallFlash
=====================

Commandline program and library to check if Flash is configured for .NET [Awesomium](http://www.awesomium.com) and installs it if not.

This is useful because 1) for some applications, you want Flash installed upon first use, 2) it is better not to run the installer if it is not needed since it confuses users, and 3) it is difficult to determine whether or not it is needed.

This program requires that Awesomium libraries are installed to a shared location or the same path as this program.  In a typical installation scenario, this program should be run after installing your Awesomium-based product.

If you want to check if Flash is installed from within your program, use AwesomiumFlashChecker.cs and see Program.cs for example usage.
