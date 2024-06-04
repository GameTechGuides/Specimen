using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUsSpecimen.ModOptions;
using AmongUsSpecimen.Utils;
using UnityEngine;
using static AmongUsSpecimen.ModOptions.Helpers;

// Container for option tabs, defining the main tab for mod settings
[ModOptionContainer(ContainerType.Tabs)]
public static class OptionTabs
{
    public static readonly ModOptionTab MainTab;

    static OptionTabs()
    {
        MainTab = new ModOptionTab("Main", "Mods Settings", GetSprite("ModSettings"));
    }

    private static Sprite GetSprite(string name)
    {
        return Assembly.GetExecutingAssembly()
            .LoadSpriteFromResources($"AmongUsSpecimen.Resources.Sprites.{name}TabIcon.png", 400f);
    }
}

// Core options class, responsible for managing the game's core mod options
[ModOptionContainer]
public static class CoreOptions
{
    // Option for selecting the current preset
    public static readonly ModStringOption PresetSelection;

    static CoreOptions()
    {
        // Initialize the preset selection option and set up event listeners
        PresetSelection = Local(OptionTabs.MainTab.StringOption("Preset", GetPresetNames, GetPresetNames()[0]));
        PresetSelection.ValueChanged += UpdatePreset;
        PresetSelection.OnUiLabelClick = OnPresetLabelClick;
        GameEventManager.HostChanged += OnHostChanged;
    }

    // Event handler for preset label click, opens the preset manager window
    private static void OnPresetLabelClick()
    {
        ModOptionManager.PresetManagerWindow?.Toggle();
    }

    // Event handler for host change, updates the UI
    private static void OnHostChanged()
    {
        PresetSelection.UiOption?.UiUpdate();
    }

    // Updates the current preset based on the selection
    private static void UpdatePreset()
    {
        var presetIdx = PresetSelection.CurrentSelection;
        if (presetIdx < 0) return;
        OptionStorage.Current.CurrentPresetIdx = presetIdx;
        OptionStorage.Persist();
        OptionStorage.ApplyCurrentPreset();
    }

    // List of preset names managed by the host
    private static readonly List<string> ManagedByHostPresetNames = ["#Host Only#"];

    // Retrieves the list of preset names, including the online preset and any custom presets
    public static List<string> GetPresetNames()
    {
        if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost) return ManagedByHostPresetNames;
        var list = new List<string> { OptionStorage.Current.OnlinePreset.Name };
        list.AddRange(OptionStorage.Current.Presets.Select(preset => preset.Name));
        return list;

    }
}
