using System;
using System.Collections.Generic;
using AmongUsSpecimen.Utils;
using UnityEngine;

namespace AmongUsSpecimen.ModOptions;

/// <summary>
/// Represents a boolean option within the mod options framework.
/// </summary>
public class ModBoolOption : BaseModOption
{
    /// <summary>
    /// Gets the display value of the option for UI purposes, showing "yes" in green for true and "no" in red for false.
    /// </summary>
    public override string DisplayValue => CurrentSelection > 0 ? ColorHelpers.Colorize(Color.green, "yes") : ColorHelpers.Colorize(Color.red, "no");

    /// <summary>
    /// Gets the display value of the option for UI purposes, showing a checkmark for true and a cross for false.
    /// </summary>
    public override string DisplayUiValue => CurrentSelection > 0 ? ColorHelpers.Colorize(Color.green, "\u2714") : ColorHelpers.Colorize(Color.red, "\u2716");
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ModBoolOption"/> class with specified parameters.
    /// </summary>
    /// <param name="tab">The tab under which the option is categorized.</param>
    /// <param name="name">The display name of the option.</param>
    /// <param name="defaultValue">The default value of the option.</param>
    /// <param name="parent">The parent option, if any.</param>
    public ModBoolOption(ModOptionTab tab, string name, bool defaultValue, BaseModOption parent = null) : base(OptionType.Boolean, tab, name, parent)
    {
        AvailableSelections = new SelectionsController([ColorHelpers.Colorize(Color.red, "no"), ColorHelpers.Colorize(Color.green, "yes")]);
        DefaultSelection = defaultValue ? 1 : 0;
        _currentSelection = GetStorageSelection();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ModBoolOption"/> class with dynamic selections.
    /// </summary>
    /// <param name="tab">The tab under which the option is categorized.</param>
    /// <param name="name">The display name of the option.</param>
    /// <param name="selectionsGetter">A function to dynamically get the selections for the option.</param>
    /// <param name="defaultValue">The default value of the option.</param>
    /// <param name="parent">The parent option, if any.</param>
    public ModBoolOption(ModOptionTab tab, string name, Func<List<string>> selectionsGetter, bool defaultValue, BaseModOption parent = null) : base(OptionType.Boolean, tab, name, parent)
    {
        AvailableSelections = new SelectionsController(selectionsGetter);
        DefaultSelection = defaultValue ? 1 : 0;
        _currentSelection = GetStorageSelection();
    }
}
