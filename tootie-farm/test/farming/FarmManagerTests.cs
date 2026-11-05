using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using TootieFarm.Farming;
using TootieFarm.Items;

namespace TootieFarm.Tests;

[TestSuite]
public class FarmManagerTests
{
    private TestOutsideMap _testMap = null!;
    private FarmManager _farm = null!;
    private ToolItem _hoe = null!;
    private ToolItem _wateringCan = null!;

    [BeforeTest]
    public void Setup()
    {
        _testMap = new TestOutsideMap();
        _farm = new FarmManager
        {
            _outsideMap = _testMap
        };

        _hoe = new ToolItem
        {
            Action = ActionType.Till
        };

        _wateringCan = new ToolItem
        {
            Action = ActionType.Water
        };
    }

    [TestCase]
    public void CanUseItem_HoeOnOpenGrass_ReturnsYes()
    {
        var cell = new Vector2I(5, 5);
        _testMap.GrassCells.Add(cell);

        CanUseItemEnum result = _farm.CanUseItem(cell, _hoe);
        GD.Print("result", result);

        AssertObject(result).IsEqual(CanUseItemEnum.Yes);
    }

    [TestCase]
    public void CanUseItem_HoeOnObstacle_ReturnsNotFarmingTile()
    {
        var cell = new Vector2I(5, 5);
        _testMap.GrassCells.Add(cell);
        _testMap.ObstacleCells.Add(cell); // Rock or fence placed here!

        CanUseItemEnum result = _farm.CanUseItem(cell, _hoe);

        AssertObject(result).IsEqual(CanUseItemEnum.NotFarmingTile);
    }

    [TestCase]
    public void UseTool_HoeOnGrass_CreatesFlatDrySoil()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);

        UseToolResult result = _farm.UseTool(cell, _hoe);

        AssertBool(result.Success).IsTrue();
        AssertBool(_testMap.SoilStates.ContainsKey(cell)).IsTrue();
        AssertObject(_testMap.SoilStates[cell]).IsEqual(SoilState.FlatDry);
    }

    [TestCase]
    public void UseTool_HoeOnFlatDry_CreatesFlatPlowed()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.FlatDry);

        UseToolResult result = _farm.UseTool(cell, _hoe);

        AssertBool(result.Success).IsTrue();
        AssertBool(_testMap.SoilStates.ContainsKey(cell)).IsTrue();
        AssertObject(_testMap.SoilStates[cell]).IsEqual(SoilState.PlowedDry);
    }
    [TestCase]
    public void UseTool_HoeOnPlowedDry_DoesNotSucceed()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);

        UseToolResult result = _farm.UseTool(cell, _hoe);

        AssertBool(result.Success).IsFalse();
        AssertBool(_testMap.SoilStates.ContainsKey(cell)).IsTrue();
        AssertObject(_testMap.SoilStates[cell]).IsEqual(SoilState.PlowedDry);
    }

    [TestCase]
    public void UseTool_WateringCanOnFlatDrySoil_CreatesFlatWetSoil()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.FlatDry);

        UseToolResult result = _farm.UseTool(cell, _wateringCan);

        AssertBool(result.Success).IsTrue();
        AssertBool(_testMap.SoilStates.ContainsKey(cell)).IsTrue();
        AssertObject(_testMap.SoilStates[cell]).IsEqual(SoilState.FlatWet);
    }

    [TestCase]
    public void UseTool_WateringCanOnPlowedDrySoil_CreatesPlowedWetSoil()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);

        UseToolResult result = _farm.UseTool(cell, _wateringCan);

        AssertBool(result.Success).IsTrue();
        AssertBool(_testMap.SoilStates.ContainsKey(cell)).IsTrue();
        AssertObject(_testMap.SoilStates[cell]).IsEqual(SoilState.PlowedWet);
    }

}
