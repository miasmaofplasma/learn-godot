using Godot;

namespace TootieFarm.World;

public partial class World : Node2D
{
    public override void _Ready()
    {
        GD.Print("World initialized!");
    }
}
