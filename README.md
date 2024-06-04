# Specimen
Specimen is a BepinEx plugin for Among Us that will relieve you of a lot of the work in mod development.

- **NuGet feed:** `https://nuget.eno.pm/v3/index.json`
- **Package Id:** `AmongUs.Specimen`

## Supported Versions:
- **Among Us:** 2024.3.5 (current version)
- **BepinEx:** 6.0.0-be688+

## Installation
- Download latest `AmongUs.Specimen.dll` file in [releases](https://github.com/EnoPM/Specimen/releases).
- Place this file in the BepinEx/plugins folder of the game folder containing your mod.

### For development
- Add my NuGet feed (``https://nuget.eno.pm/v3/index.json``) to your nuget.config file.
- Search and install latest version of `AmongUs.Specimen` from NuGet package manager.

## Features
- A mod updater that allows players to download an older version or update your mod.
- Custom cosmetics loader from github repositor (only hats are supported at moment).
- Custom regions manager: Private regions will be saved in a separate file so as not to influence the vanilla game.
- Simplest Custom RPC management.

### Mod updater
```csharp
using BepInEx;
using BepInEx.Unity.IL2CPP;
using AmongUsSpecimen;
using AmongUsSpecimen.Updater;

namespace SpecimenDemo;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency(Specimen.Guid)]
[ModUpdater("EnoPM", "Specimen", Version, "DemoPlugin.dll")]
public class DemoPlugin : BasePlugin
{
    private const string Guid = "demo.specimen.eno.pm";
    private const string Name = "DemoSpecimen";
    private const string Version = "0.0.1";
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo($"Plugin {Name} is loaded!");
    }
}
```

### Custom cosmetics
```csharp
using BepInEx;
using BepInEx.Unity.IL2CPP;
using AmongUsSpecimen;
using AmongUsSpecimen.Cosmetics;

namespace SpecimenDemo;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency(Specimen.Guid)]
[CustomCosmetics("EnoPM/BetterOtherHats", "CustomHats.json")]
public class DemoPlugin : BasePlugin
{
    private const string Guid = "demo.specimen.eno.pm";
    private const string Name = "DemoSpecimen";
    private const string Version = "0.0.1";
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo($"Plugin {Name} is loaded!");
    }
}
```

### Custom regions
```csharp
using BepInEx;
using BepInEx.Unity.IL2CPP;
using AmongUsSpecimen;

namespace SpecimenDemo;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency(Specimen.Guid)]
[CustomRegion("Specimen Europe", "specimen.example.com", "https://specimen.example.com", color: "#ff00ff")]
public class DemoPlugin : BasePlugin
{
    private const string Guid = "demo.specimen.eno.pm";
    private const string Name = "DemoSpecimen";
    private const string Version = "0.0.1";
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo($"Plugin {Name} is loaded!");
    }
}
```

### Custom RPCs
```csharp
using BepInEx;
using BepInEx.Unity.IL2CPP;
using AmongUsSpecimen;

namespace SpecimenDemo;

[BepInPlugin(Guid, Name, Version)]
[BepInDependency(Specimen.Guid)]
public class DemoPlugin : BasePlugin
{
    private const string Guid = "demo.specimen.eno.pm";
    private const string Name = "DemoSpecimen";
    private const string Version = "0.0.1";
    
    public override void Load()
    {
        // Plugin startup logic
        Log.LogInfo($"Plugin {Name} is loaded!");
    }

    [Rpc]
    public static void RpcKillPlayer(PlayerControl __sender, PlayerControl target)
    {
        __sender.MurderPlayer(target, MurderResultFlags.Succeeded);
    }
}
```
When the `DemoPlugin.RpcKillPlayer(PlayerControl.LocalPlayer, target)` method is called an rpc call will be made and the method will be called on the other clients with the same arguments.

Custom RPCs also work with extensions and this simplifies their use.
```csharp
using AmongUsSpecimen;
using UnityEngine;

namespace SpecimenDemo;

public static class PlayerControlExtensions
{
    [Rpc]
    public static void RpcKillPlayer(this PlayerControl __sender, PlayerControl target)
    {
        __sender.MurderPlayer(target, MurderResultFlags.Succeeded);
    }

    [Rpc]
    public static void RpcSetNameColor(this PlayerControl __sender, Color color)
    {
        __sender.cosmetics.nameText.color = color;
    }
}
```
`PlayerControl.LocalPlayer.RpcSetNameColor(Color.green)` will change the color of the player's nickname and share this change with other players.

## Custom Game Options Documentation

This section provides comprehensive guidance on implementing and utilizing custom game options within the Specimen framework for Among Us.

### Defining Custom Game Options

To define custom game options, you will need to create a new class that represents your custom options. This class should be static and contain public static fields or properties representing each option.

Example:
```csharp
using AmongUsSpecimen.ModOptions;

public static class MyCustomOptions
{
    var tab = new ModOptionTab("Example", "Example", Sprite); //Make sure the sprite exsit for no null references
}
```

### Using Custom Game Options

Once you have defined your custom game options, you can use them in your game logic just like any other variable. The Specimen framework automatically handles saving and loading these options, as well as syncing them with other players in a multiplayer game.

Example:
```csharp
if (MyCustomOptions.CustomBoolOption)
{
    // Custom game logic here
}
```

### Critical Files

- `AmongUsSpecimen/CoreOptions.cs`: This file contains the core logic for handling custom game options. It is responsible for registering custom options, saving/loading them, and syncing them in multiplayer games.

- `AmongUsSpecimen/ModOptions/`: This directory contains various helper classes and attributes used to define and manage custom game options.

By following these guidelines, you can easily add custom game options to your Among Us mod using the Specimen framework.

*This mod is not affiliated with Among Us or Innersloth LLC, and the content contained therein is not endorsed or otherwise sponsored by Innersloth LLC. Portions of the materials contained herein are property of Innersloth LLC. © Innersloth LLC.*

**Note:** It's important to include inline comments in your code to explain the purpose and usage of your methods and classes. This practice enhances code readability and maintenance.
