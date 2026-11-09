using Godot;
using System;

namespace TootieFarm;

public partial class DayNightCycle : CanvasModulate
{
    private Color DAY = Colors.White;
    private Color TWILIGHT = new Color("#b39779");
    private Color NIGHT = new Color("#2f368e");
    [Export]
    private Timemanger _timeManager = null!;
    private Tween? _cycleTween;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _timeManager.HourUpdated += ProcessHourUpdate;
        ProcessHourUpdate(_timeManager.CurrentHours);
    }
    public void ProcessHourUpdate(uint hour)
    {
        uint nextHour = (hour + 1) % 24;

        _cycleTween?.Kill();
        _cycleTween = CreateTween();
        _cycleTween.TweenProperty(this, "color", ColorAtHour(nextHour), 2.0);
    }

    private Color ColorAtHour(uint hour)
    {
        if (hour < 6)
            return NIGHT;

        if (hour < 8)
            return NIGHT.Lerp(TWILIGHT, (hour - 6) / 2.0f);

        if (hour < 10)
            return TWILIGHT.Lerp(DAY, (hour - 8) / 2.0f);

        if (hour < 18)
            return DAY;

        if (hour < 20)
            return DAY.Lerp(TWILIGHT, (hour - 18) / 2.0f);

        if (hour < 22)
            return TWILIGHT.Lerp(NIGHT, (hour - 20) / 2.0f);

        return NIGHT;
    }
}
