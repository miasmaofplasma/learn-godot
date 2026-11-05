using Godot;
using System;

namespace TootieFarm.Items;

public enum ActionType
{
    Till, Water, Harvest
}

[GlobalClass]
public partial class ToolItem : ItemData
{
    [Export] public ActionType Action { get; set; } = ActionType.Till;
}
