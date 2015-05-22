using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;

namespace SkyrimLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Time in milliseconds to poll running processes
            const int PollTime = 500;
            const int SkyrimRunningPollTime = 5000;

            // Executable process names
            const string SkyrimProcessName = "TESV";
            const string SteamProcessName = "Steam";
            const string SteamErrorReporterProcessName = "steamerrorreporter";
            const string ENBInjectorProcessName = "ENBInjector";

            // Path to Skyrim folder in relation to SteamPath
            const string skyrimRelativePath = @"steamapps\common\skyrim\";

            // Read Registry values if possible
            string steamExePath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamExe", null);
            string steamFolderPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
            string skyrimExe = null;
            string enbExe = null;

            // Try to locate Skyrim, SKSE, and ENB
            if (steamFolderPath != null)
            {
                string skyrimFolder = Path.Combine(steamFolderPath, skyrimRelativePath);

                string skseTempPath = Path.Combine(skyrimFolder, "skse_loader.exe");
                string skyrimTempPath = Path.Combine(skyrimFolder, "TESV.exe");
                string enbTempPath = Path.Combine(skyrimFolder, "enbinjector.exe");

                // Use SKSE if it is found, otherwise fall back to vanilla Skyrim EXE
                skyrimExe = File.Exists(skseTempPath) ? skseTempPath : (File.Exists(skyrimTempPath) ? skyrimTempPath : null);
                enbExe = File.Exists(enbTempPath) ? enbTempPath : null;
            }

            // Read in config file as a fallback
            if (File.Exists("launcher-config.xml"))
            {
                XElement eConfig = XElement.Load("launcher-config.xml");
                if (eConfig != null)
                {
                    if (steamExePath == null)
                    {
                        XElement eSteam = eConfig.Element("steam");
                        steamExePath = eSteam.Attribute("path").Value;
                    }
                    if (enbExe == null)
                    {
                        XElement eEnbInjector = eConfig.Element("enbinjector");
                        string enbInjectorPath = eEnbInjector.Attribute("path") != null ? eEnbInjector.Attribute("path").Value : null;
                    }
                    if (skyrimExe == null)
                    {
                        XElement eSkyrim = eConfig.Element("skyrim");
                        string skyrimPath = eSkyrim.Attribute("path").Value;
                    }
                }
            }

            // Launch Steam if it's not running
            if (!GetIsRunningProcessByName(SteamProcessName))
            {
                Process steam = Process.Start(steamExePath);

                // Poll to see if steamerrorreporter is running every "pollTime" milliseconds
                while (!GetIsRunningProcessByName(SteamErrorReporterProcessName))
                {
                    Thread.Sleep(PollTime);
                }

                // Steam seems to kill steamerrorreporter once it's fully launched; wait until then
                while (GetIsRunningProcessByName(SteamErrorReporterProcessName))
                {
                    Thread.Sleep(PollTime);
                }
            }

            Process enbInjector = null;
            if (enbExe != null)
            {
                // Launch ENBInjector if it's not running
                if (!GetIsRunningProcessByName(ENBInjectorProcessName))
                {
                    enbInjector = Process.Start(enbExe);
                    while (!GetIsRunningProcessByName(enbInjector.ProcessName))
                    {
                        Thread.Sleep(PollTime);
                    }
                }

                // enbseries.ini must be in the same directory as SkyrimMasterLauncher
                // TODO: Possibly create enbseries.ini file dynamically based on paths from config OR copy existing INI over (and update paths within)
            }

            // Launch Skyrim (SKSE)
            Process skyrim = Process.Start(skyrimExe);

            while(!GetIsRunningProcessByName(SkyrimProcessName))
            {
                Thread.Sleep(PollTime);
            }
            
            while (GetIsRunningProcessByName(SkyrimProcessName))
            {
                Thread.Sleep(SkyrimRunningPollTime);
            }

            // Kill ENB when finished
            if (enbInjector != null)
            {
                enbInjector.Kill();
            }
        }

        private static bool GetIsRunningProcessByName(string processName)
        {
            return Process.GetProcessesByName(processName).Length != 0;
        }
    }
}
