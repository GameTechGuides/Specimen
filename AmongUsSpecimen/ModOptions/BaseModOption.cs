using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using AmongUsSpecimen.UI.LobbyHud;
using AmongUsSpecimen.Utils;
using UnityEngine;
using UniverseLib.UI;

namespace AmongUsSpecimen.ModOptions;

public abstract class BaseModOption
{
    // Unique identifier for the option
    public int Id { get; }
    // Type of the option (e.g., boolean, float, string)
    public OptionType Type { get; }
    // Tab under which the option is categorized
    public ModOptionTab Tab { get; private init; }
    // Display name of the option
    public string Name { get; }
    // Controller for available selections for the option
    protected SelectionsController AvailableSelections { get; init; }
    // Default selection index
    public int DefaultSelection { get; protected init; }
    // Current selection index
    protected int _currentSelection { get; set; }

    // Restriction level of the option (public, private, local)
    public OptionRestriction Restriction { get; private set; } = OptionRestriction.Public;
    // Location where the option's value is saved (preset, local, global)
    public OptionSaveLocation SaveLocation { get; private set; } = OptionSaveLocation.Preset;
    // Determines if the option is enabled when its parent is disabled
    public bool EnabledIfParentDisabled { get; private set; }
    // Action triggered on UI label click
    internal Action OnUiLabelClick { get; set; }

    // Updates the UI and behavior based on the current selection
    public void Update()
    {
        try
        {
            UiOption?.UiUpdate();
            BehaviourUpdate();
        }
        catch
        {
        }
    }

    // Sets the restriction level for the option
    public BaseModOption SetRestriction(OptionRestriction restriction = OptionRestriction.Public)
    {
        Restriction = restriction;
        Update();
        return this;
    }

    // Sets the save location for the option's value
    public BaseModOption SetSaveLocation(OptionSaveLocation location = OptionSaveLocation.Preset)
    {
        SaveLocation = location;
        SetCurrentSelection(GetStorageSelection(), false);
        Update();
        return this;
    }

    // Sets whether the option is enabled if its parent is disabled
    public BaseModOption SetEnabledIfParentDisabled(bool value = false)
    {
        EnabledIfParentDisabled = value;
        Update();
        return this;
    }

    // Sets whether the option is a header
    public BaseModOption SetIsHeader(bool? value = null)
    {
        if (!value.HasValue)
        {
            IsHeader = Parent == null;
        }
        else
        {
            IsHeader = value.Value;
        }

        Update();
        return this;
    }

    // Gets or sets the current selection index, saving the option if necessary
    public int CurrentSelection
    {
        get => _currentSelection;
        set => SetCurrentSelection(value);
    }

    internal const float BaseOptionYOffset = 0.5f;
    internal const float AdditionalHeaderYOffset = 0.25f;

    // Updates the behavior of the option based on its type and current state
    internal void BehaviourUpdate()
    {
        if (!OptionBehaviour) return;
        if (Type == OptionType.Boolean)
        {
            var toggleOption = OptionBehaviour as ToggleOption;
            if (!toggleOption) return;
            toggleOption.CheckMark.enabled = toggleOption.oldValue = GetBool();
        }
        else
        {
            var stringOption = OptionBehaviour as StringOption;
            if (!stringOption) return;
            stringOption.Value = stringOption.oldValue = CurrentSelection;
            stringOption.ValueText.text = DisplayValue;
        }

        var enabled = IsParentEnabled;
        OptionBehaviour.gameObject.SetActive(enabled);
    }

    // Sets the current selection index and updates the option accordingly
    internal void SetCurrentSelection(int value, bool updatePreset = true)
    {
        var min = AvailableSelections.MinSelection;
        var max = AvailableSelections.MaxSelection;
        var index = value < min ? max : value > max ? min : value;
        if (index == _currentSelection) return;
        _currentSelection = index;
        if (Restriction == OptionRestriction.Public && SaveLocation != OptionSaveLocation.Local &&
            AmongUsClient.Instance && AmongUsClient.Instance.AmHost)
        {
            ModOptionManager.RpcSetOptionSelection(Id, _currentSelection);
        }

        BehaviourUpdate();

        if (updatePreset)
        {
            SaveOption();
        }

        ValueChanged?.Invoke();
    }

    // Parent option, if any
    public BaseModOption Parent { get; }
    // The actual UI component representing the option
    internal OptionBehaviour OptionBehaviour { get; set; }
    // Custom UI option component
    internal UiCustomOption UiOption { get; set; }
    // Indicates if the option serves as a header
    public bool IsHeader { get; private set; }
    // Determines if the option is enabled based on its current selection and parent's state
    internal bool IsEnabled => CurrentSelection > 0 && IsParentEnabled;

    // Determines if the parent option is enabled
    internal bool IsParentEnabled => Parent == null || (!EnabledIfParentDisabled && Parent.IsEnabled) ||
                                     (EnabledIfParentDisabled && !Parent.IsEnabled && Parent.IsParentEnabled);

    // Children options of this option
    internal IEnumerable<BaseModOption> Children => ModOptionManager.Options.Where(x => x.Parent == this);

    // Generates a unique name for the option based on its tab and parent
    private string UniqueName => $"{Tab.Key}:" + Name + (Parent == null ? string.Empty : $":{Parent.UniqueName}");

    // Constructor initializing the option with its basic properties
    protected BaseModOption(OptionType type, ModOptionTab tab, string name, BaseModOption parent = null)
    {
        Tab = tab;
        Type = type;
        Name = name;
        Parent = parent;
        Id = GetInt32HashCode(UniqueName);
        ModOptionManager.Options.Add(this);
        IsHeader = Parent == null;
    }

    // Saves the current selection of the option based on its save location
    private void SaveOption()
    {
        switch (SaveLocation)
        {
            case OptionSaveLocation.Global:
                OptionStorage.Current.Global[Id] = CurrentSelection;
                break;
            case OptionSaveLocation.Local:
                OptionStorage.Current.Local[Id] = CurrentSelection;
                break;
            case OptionSaveLocation.Preset:
            default:
                OptionStorage.Current.GetCurrentPreset().Values[Id] = CurrentSelection;
                break;
        }

        OptionStorage.SaveCurrentPreset();
    }

    // Retrieves the current selection from storage based on the save location
    protected int GetStorageSelection()
    {
        switch (SaveLocation)
        {
            case OptionSaveLocation.Global:
                return OptionStorage.Current.Global.GetValueOrDefault(Id, DefaultSelection);
            case OptionSaveLocation.Local:
                return OptionStorage.Current.Local.GetValueOrDefault(Id, DefaultSelection);
            case OptionSaveLocation.Preset:
                return OptionStorage.Current.GetCurrentPreset().Values.GetValueOrDefault(Id, DefaultSelection);
        }

        return DefaultSelection;
    }

    // Generates a hash code for the option based on its unique name
    private readonly SHA1 hash = SHA1.Create();

    private int GetInt32HashCode(string strText)
    {
        if (string.IsNullOrEmpty(strText)) return 0;
        var byteContents = Encoding.Unicode.GetBytes(strText);
        var hashText = hash.ComputeHash(byteContents);
        var hashCodeStart = BitConverter.ToInt32(hashText, 0);
        var hashCodeMedium = BitConverter.ToInt32(hashText, 8);
        var hashCodeEnd = BitConverter.ToInt32(hashText, 16);
        var hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        return int.MaxValue - hashCode;
    }

    // Calculates the indentation level for the UI display based on the option's hierarchy
    internal int GetUiIndentation()
    {
        var i = 0;
        var cursor = Parent;
        while (cursor != null)
        {
            if (cursor.Tab == Tab)
            {
                i++;
            }

            cursor = cursor.Parent;
        }

        return i;
    }

    // Generates the display name for the option, including indentation for hierarchy
    internal string GetDisplayName(int prefixSize = 1)
    {
        const string blankChar = "H";
        var indentation = GetUiIndentation();
        if (indentation == 0) return Name;
        var prefix = string.Empty;
        for (var i = 0; i < indentation; i++)
        {
            prefix += blankChar;
        }

        return $"<size={prefixSize}>{ColorHelpers.Colorize(UIPalette.Transparent, prefix)}</size>{Name}";
    }

    // Display name of the option, including any UI-specific formatting
    public virtual string DisplayName => GetDisplayName();
    // Display value of the option for UI purposes
    public virtual string DisplayValue => AvailableSelections.GetValue(CurrentSelection);
    // Display value of the option for UI purposes, potentially with different formatting
    public virtual string DisplayUiValue => AvailableSelections.GetValue(CurrentSelection);

    // Event triggered when the option's value changes
    public event Action ValueChanged;

    // Helper methods for retrieving the option's value in various formats
    public bool GetBool() => IsEnabled;
    public string GetString() => AvailableSelections.GetValue(CurrentSelection);
    public float GetFloat() => float.Parse(GetString());
    public int GetInt() => Mathf.RoundToInt(GetFloat());
    public static implicit operator bool(BaseModOption option) => option.GetBool();
    public static implicit operator string(BaseModOption option) => option.GetString();
    public static implicit operator float(BaseModOption option) => option.GetFloat();
    public static implicit operator int(BaseModOption option) => option.GetInt();
}
