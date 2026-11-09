using Godot;

namespace TootieFarm;

public partial class Timemanger : Node
{
    Timer _worldTimer = null!;
    public uint CurrentDay
    {
        get; private set
        {
            field = value;
            EmitSignal(SignalName.DayUpdated, CurrentDay);
        }
    } = 0;
    public uint CurrentHours
    {
        get; private set
        {
            if (value >= 24)
            {
                field = 0;
                EmitSignal(SignalName.HourUpdated, CurrentHours);
                CurrentDay++;
            }
            else
            {
                field = value;
                EmitSignal(SignalName.HourUpdated, CurrentHours);
            }
        }
    }

    public override void _Ready()
    {
        _worldTimer = GetNode<Timer>("WorldTimer");
        _worldTimer.Timeout += OnTimerReady;
        CurrentHours = 8;
    }

    private void OnTimerReady()
    {
        CurrentHours++;
    }

    [Signal]
    public delegate void DayUpdatedEventHandler(uint day);
    [Signal]
    public delegate void HourUpdatedEventHandler(uint hour);
}
