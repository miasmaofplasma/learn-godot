using Godot;
using System;

public partial class SignalReactor : Node
{
    public void OnSpeedChanged(float speed)
    {
        GD.Print($"speed changed = {speed}");
    }
}
