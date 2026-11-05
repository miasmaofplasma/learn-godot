
using Godot;
using System;
public partial class Main : Node
{
    [Export]
    public float Speed { get; set; } = 400f;
    public override void _Ready()
    {
        var reciever = GetNode<SignalReactor>("SignalReactor");
        SpeedChanged += reciever.OnSpeedChanged;
        EmitSignal(SignalName.SpeedChanged, Speed);
    }


    [Signal]
    public delegate void SpeedChangedEventHandler(float newValue);
}
