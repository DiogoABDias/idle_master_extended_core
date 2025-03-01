﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Forms;

namespace IdleMasterExtended;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Set the Browser emulation version for embedded browser control
        try
        {
            RegistryKey ie_root = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            key.SetValue(programName, (int)10001, RegistryValueKind.DWord);
        }
        catch (Exception ex)
        {
            Logger.Exception(ex, "Program -> Main -> Registry and environment modifications resulted in an exception.");
        }

        Application.ThreadException += (o, a) => Logger.Exception(a.Exception);
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.Run(new FrmMain());
    }
}
