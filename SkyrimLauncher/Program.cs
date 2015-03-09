using System;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;

namespace SkyrimLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            // Time in milliseconds to poll running processes
            const int pollTime = 500;

            // Read in config file
            XElement eConfig = XElement.Load("launcher-config.xml");

            if (eConfig != null)
            {
                XElement eSteam = eConfig.Element("steam");
                XElement eEnbInjector = eConfig.Element("enbinjector");
                XElement eSkyrim = eConfig.Element("skyrim");

                // Check that config has proper elements
                if(eSteam == null || eEnbInjector == null || eSkyrim == null)
                {
                    Console.WriteLine("launcher-config.xml must contain steam, enbinjector, and skyrim elements");
                    WaitForInputAndClose();
                }

                // Check that config elements have path attributes
                if(eSteam.Attribute("path") == null || (eEnbInjector.Attribute("path") == null && eEnbInjector.Attribute("disable") == null) || eSteam.Attribute("path") == null)
                {
                    Console.WriteLine("launcher-config.xml steam and skyrim must contain path attributes");
                    Console.WriteLine("launcher-config.xml enbinjector must contain path and/or disable attributes");
                    WaitForInputAndClose();
                }

                string steamPath = eSteam.Attribute("path").Value;
                string enbInjectorPath = eEnbInjector.Attribute("path") != null ? eEnbInjector.Attribute("path").Value : null;
                string skyrimPath = eSkyrim.Attribute("path").Value;
                bool disableEnb = false;

                try
                {
                    disableEnb = bool.Parse(eEnbInjector.Attribute("disable").Value);
                }
                catch (Exception)
                {
                    disableEnb = false;
                }

                // Launch Steam if it's not running
                if (!GetIsRunningProcessByName("Steam"))
                {
                    Process steam = Process.Start(steamPath);

                    // Poll to see if steamerrorreporter is running every "pollTime" milliseconds
                    while (!GetIsRunningProcessByName("steamerrorreporter"))
                    {
                        Thread.Sleep(pollTime);
                    }

                    // Steam seems to kill steamerrorreporter once it's fully launched; wait until then
                    while (GetIsRunningProcessByName("steamerrorreporter"))
                    {
                        Thread.Sleep(pollTime);
                    }
                }

                if (!disableEnb)
                {
                    // Launch ENBInjector if it's not running
                    if (!GetIsRunningProcessByName("ENBInjector"))
                    {
                        Process enbInjector = Process.Start(enbInjectorPath);
                        while (!GetIsRunningProcessByName(enbInjector.ProcessName))
                        {
                            Thread.Sleep(pollTime);
                        }
                    }

                    // enbseries.ini must be in the same directory as SkyrimMasterLauncher
                    // TODO: Create enbseries.ini file dynamically based on paths from config OR copy existing INI over (and update paths within)
                }

                // Launch Skyrim
                Process skyrim = Process.Start(skyrimPath);
            }
        }

        private static bool GetIsRunningProcessByName(string processName)
        {
            return Process.GetProcessesByName(processName).Length != 0;
        }

        private static void WaitForInputAndClose()
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
            Environment.Exit(1);
        }
    }
}
