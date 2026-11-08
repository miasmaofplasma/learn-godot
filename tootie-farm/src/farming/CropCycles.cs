using Godot;
namespace TootieFarm.Farming;

[GlobalClass]
public partial class CropCycles : Resource
{
    private const ushort STAGE_COUNT = 4;
    [Export] public ushort GrowthDaysDuration { get; set; } = 5;
    public ushort GrowthDay
    {
        get; set
        {
            field = (ushort)Mathf.Min(GrowthDaysDuration, value);
        }
    } = 0;
    public void IncrementDay()
    {
        GrowthDay++;
    }
    public ushort GetStage()
    {
        if (GrowthDaysDuration <= 0 || GrowthDay >= GrowthDaysDuration)
        {
            return STAGE_COUNT - 1;
        }
        return (ushort)Mathf.Clamp((GrowthDay * STAGE_COUNT) / GrowthDaysDuration, 0, STAGE_COUNT - 1);
    }
    public bool Finished()
    {
        return GrowthDaysDuration == GrowthDay;
    }
}
