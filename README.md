Skyrim Master Launcher
======================

Skyrim Master Launcher is a simple program that will simplify the launching of a modded Skyrim by ensuring that common prerequisites, like Steam and ENBInjector, are running before attempting to launch Skyrim.

It requires .NET 4.5 to run.

Possible Issues
---------------

If upon launching Skyrim you don't see the "Bethesda" logo in the
beginning, it likely means that something went wrong with
ENBInjector, and your game will crash when you try to load a save.
To prevent this from happening, just put SkyrimMasterLauncher.exe
and launcher-config.xml in the same directory as ENBInjector, and
the problem should be resolved.  You can read about the cause of
this [here](https://github.com/bsinky/SkyrimMasterLauncher/wiki/ENBInjector-Issues).

If the program crashes or gives some other errors, the launcher-config.xml was either not found or is missing some vital information.  You need to fill in the correct paths to the following in the launcher-config.xml:
- TESV.exe or skse_loader.exe, depending on which you use
- ENBInjector.exe if you use it, or add a disable="true" attribute to the enbinjector element
- Steam.exe (assuming you launch Skyrim through Steam, non-Steam installs are currently unsupported thorugh SkyrimMasterLauncher)
