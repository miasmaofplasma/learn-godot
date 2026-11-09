using System;
using Godot;

namespace TootieFarm;

public partial class Clock : Control
{
    [Export]
    private Timemanger _timeManager = null!;
    private Label _daysLabel = null!;
    private Label _hoursLabel = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _daysLabel = GetNode<Label>("%DaysLabel");
        _hoursLabel = GetNode<Label>("%HoursLabel");

        _timeManager.HourUpdated += OnHourChanged;
        _timeManager.DayUpdated += OnDayChanged;

        SetHourLabel(_timeManager.CurrentHours);
        SetDaysLabel(_timeManager.CurrentDay);
    }

    private void OnDayChanged(uint day)
    {
        SetDaysLabel(day);
    }

    private void OnHourChanged(uint hour)
    {
        SetHourLabel(hour);
    }

    private void SetDaysLabel(uint day)
    {
        _daysLabel.Text = $"Days {day}";
    }

    private void SetHourLabel(uint hour)
    {
        _hoursLabel.Text = $"Hours {hour}";
    }
}
