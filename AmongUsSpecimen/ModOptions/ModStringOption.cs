using System;
using System.Collections.Generic;

namespace AmongUsSpecimen.ModOptions;

/// <summary>
/// Represents a string option within the mod options framework.
/// </summary>
public class ModStringOption : BaseModOption
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModStringOption"/> class with specified parameters.
    /// </summary>
    /// <param name="tab">The tab under which the option is categorized.</param>
    /// <param name="name">The display name of the option.</param>
    /// <param name="selections">The list of string selections for the option.</param>
    /// <param name="defaultValue">The default value of the option.</param>
    /// <param name="parent">The parent option, if any.</param>
    public ModStringOption(ModOptionTab tab, string name, List<string> selections, string defaultValue, BaseModOption parent = null) : base(OptionType.String, tab, name, parent)
    {
        AvailableSelections = new SelectionsController(selections);
        DefaultSelection = AvailableSelections.Values.IndexOf(defaultValue);
        _currentSelection = GetStorageSelection();
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ModStringOption"/> class with dynamic selections.
    /// </summary>
    /// <param name="tab">The tab under which the option is categorized.</param>
    /// <param name="name">The display name of the option.</param>
    /// <param name="selectionsGetter">A function to dynamically get the selections for the option.</param>
    /// <param name="defaultValue">The default value of the option.</param>
    /// <param name="parent">The parent option, if any.</param>
    public ModStringOption(ModOptionTab tab, string name, Func<List<string>> selectionsGetter, string defaultValue, BaseModOption parent = null) : base(OptionType.String, tab, name, parent)
    {
        AvailableSelections = new SelectionsController(selectionsGetter);
        DefaultSelection = AvailableSelections.Values.IndexOf(defaultValue);
    }
}
