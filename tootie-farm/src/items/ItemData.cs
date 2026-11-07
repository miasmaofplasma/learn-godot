using System;
using Godot;

namespace TootieFarm.Items;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export]
    public string Id { get; set; } = "";

    [Export]
    public string DisplayName { get; set; } = "";

    [Export]
    public Texture2D Icon { get; set; } = null!;

    [Export]
    public uint MaxStack { get; set; } = 1;

    public virtual void TryReduceAmount() { }
}
