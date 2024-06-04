using System;
using System.Collections.Generic;

namespace AmongUsSpecimen.ModOptions;

/// <summary>
/// Represents a floating-point option within the mod options framework.
/// </summary>
public class ModFloatOption : BaseModOption
{
    // Prefix and suffix for the display value of the option
    private readonly string _prefix;
    private readonly string _suffix;
    
    /// <summary>
    /// Gets the display value of the option for UI purposes, including prefix and suffix.
    /// </summary>
    public override string DisplayValue => $"{_prefix}{AvailableSelections.GetValue(CurrentSelection)}{_suffix}";

    /// <summary>
    /// Gets the display value of the option for UI purposes, including prefix and suffix.
    /// </summary>
    public override string DisplayUiValue => $"{_prefix}{AvailableSelections.GetValue(CurrentSelection)}{_suffix}";
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ModFloatOption"/> class with specified parameters.
    /// </summary>
    /// <param name="tab">The tab under which the option is categorized.</param>
    /// <param name="name">The display name of the option.</param>
    /// <param name="minValue">The minimum value of the option.</param>
    /// <param name="maxValue">The maximum value of the option.</param>
    /// <param name="step">The step value between each selectable option.</param>
    /// <param name="defaultValue">The default value of the option.</param>
    /// <param name="parent">The parent option, if any.</param>
    /// <param name="prefix">The prefix to display before the option value.</param>
    /// <param name="suffix">The suffix to display after the option value.</param>
    public ModFloatOption(ModOptionTab tab, string name, float minValue, float maxValue, float step, float defaultValue, BaseModOption parent = null, string prefix = "", string suffix = "") : base(OptionType.Float, tab, name, parent)
    {
        if ((minValue < 0f && maxValue < 0f) || step < 0f)
        {
            throw new Exception("ModFloatOption range cannot be negative");
        }
        var selections = new List<string>();
        for (var i = minValue; i <= maxValue; i += step)
        {
            selections.Add($"{i}");
        }

        _prefix = prefix;
        _suffix = suffix;

        AvailableSelections = new SelectionsController(selections);
        DefaultSelection = AvailableSelections.Values.IndexOf($"{defaultValue}");
        _currentSelection = GetStorageSelection();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ModFloatOption"/> class with dynamic selections.
    /// </summary>
    /// <param name="tab">The tab under which the option is categorized.</param>
    /// <param name="name">The display name of the option.</param>
    /// <param name="selectionsGetter">A function to dynamically get the selections for the option.</param>
    /// <param name="defaultValue">The default value of the option.</param>
    /// <param name="parent">The parent option, if any.</param>
    /// <param name="prefix">The prefix to display before the option value.</param>
    /// <param name="suffix">The suffix to display after the option value.</param>
    public ModFloatOption(ModOptionTab tab, string name, Func<List<string>> selectionsGetter, float defaultValue, BaseModOption parent = null, string prefix = "", string suffix = "") : base(OptionType.Float, tab, name, parent)
    {
        _prefix = prefix;
        _suffix = suffix;

        AvailableSelections = new SelectionsController(selectionsGetter);
        DefaultSelection = AvailableSelections.Values.IndexOf($"{defaultValue}");
        _currentSelection = GetStorageSelection();
    }
}
