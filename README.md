Skyrim Master Launcher
======================

Skyrim Master Launcher is a simple program that will simplify the launching of a modded Skyrim by ensuring that common prerequisites, like Steam and ENBInjector, are running before attempting to launch Skyrim.

**It requires .NET 4.5 to run.**

It should now be able to look up your Steam install path using the Registry, and figure out where Skyrim, SKSE, and ENBInjector are from there, as long as they're all in the root Skyrim directory.  If they're not, you'll likely run into issues.

SkyrimMasterLauncher now stays running in the background after Skyrim has launched, and runs until Skyrim has closed.  This allows you to stream your SKSE-modded Skyrim to other computers using Steam's game streaming feature.
- Create a Steam shortcut to SkyrimMasterLauncher on the computer you wish to stream from
  - Use the "Add a Non-Steam Game" option
- It should show up when you've connected to your Steam account on other machines and allow you to steam it.

Possible Issues
---------------

If upon launching Skyrim you don't see the "Bethesda" logo in the
beginning, it likely means that something went wrong with
ENBInjector, and your game will crash when you try to load a save.
To prevent this from happening, just put SkyrimMasterLauncher.exe
and launcher-config.xml in the same directory as ENBInjector, and
the problem should be resolved.  You can read about the cause of
this [here](https://github.com/bsinky/SkyrimMasterLauncher/wiki/ENBInjector-Issues).

If the program crashes or gives some other errors, you may need to create the launcher-config.xml file, and fill in the correct paths to the following:
- TESV.exe or skse_loader.exe, depending on which you use
- ENBInjector.exe if you use it
- Steam.exe (assuming you launch Skyrim through Steam, non-Steam installs are currently unsupported thorugh SkyrimMasterLauncher)
