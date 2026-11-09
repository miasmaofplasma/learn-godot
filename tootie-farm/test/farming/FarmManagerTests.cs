using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using TootieFarm.Farming;
using TootieFarm.Items;

namespace TootieFarm.Tests;

[TestSuite]
[RequireGodotRuntime]
public class FarmManagerTests
{
    private TestOutsideMap _testMap = null!;
    private FarmManager _farm = null!;
    private ToolItem _hoe = null!;
    private ToolItem _wateringCan = null!;
    private ToolItem _basket = null!;

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

        _basket = new ToolItem
        {
            Action = ActionType.Harvest
        };
    }

    [AfterTest]
    public void Teardown()
    {
        _testMap.Free();
        _farm.Free();
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
    public void CanUseItem_HoeOnObstacle_ReturnsNo()
    {
        var cell = new Vector2I(5, 5);
        _testMap.GrassCells.Add(cell);
        _testMap.ObstacleCells.Add(cell); // Rock or fence placed here!

        CanUseItemEnum result = _farm.CanUseItem(cell, _hoe);

        AssertObject(result).IsEqual(CanUseItemEnum.No);
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

    private static SeedItem CreateTestSeed(CropType cropType = CropType.Radish, ushort duration = 4)
    {
        var cycles = new CropCycles { GrowthDaysDuration = duration };
        var cropData = new CropData
        {
            CropType = cropType,
            CropCycles = cycles,
        };
        return new SeedItem
        {
            GrownCrop = cropData,
            Id = "seed_test",
            DisplayName = "Test Seed",
        };
    }

    [TestCase]
    public void CanUseItem_SeedOnUnplowedSoil_ReturnsNo()
    {
        var cell = new Vector2I(4, 4);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.FlatDry);
        var seed = CreateTestSeed();

        CanUseItemEnum result = _farm.CanUseItem(cell, seed);

        AssertObject(result).IsEqual(CanUseItemEnum.No);
    }

    [TestCase]
    public void CanUseItem_SeedOnPlowedSoil_ReturnsYes()
    {
        var cellDry = new Vector2I(4, 4);
        var cellWet = new Vector2I(4, 5);
        _testMap.GrassCells.Add(cellDry);
        _testMap.GrassCells.Add(cellWet);
        _testMap.SetSoil(cellDry, SoilState.PlowedDry);
        _testMap.SetSoil(cellWet, SoilState.PlowedWet);
        var seed = CreateTestSeed();

        AssertObject(_farm.CanUseItem(cellDry, seed)).IsEqual(CanUseItemEnum.Yes);
        AssertObject(_farm.CanUseItem(cellWet, seed)).IsEqual(CanUseItemEnum.Yes);
    }

    [TestCase]
    public void CanUseItem_SeedOnObstacle_ReturnsNo()
    {
        var cell = new Vector2I(4, 4);
        _testMap.GrassCells.Add(cell);
        _testMap.ObstacleCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        var seed = CreateTestSeed();

        CanUseItemEnum result = _farm.CanUseItem(cell, seed);

        AssertObject(result).IsEqual(CanUseItemEnum.No);
    }

    [TestCase]
    public void CanUseItem_SeedOnNonGrass_ReturnsNotFarmingTile()
    {
        var cell = new Vector2I(4, 4);
        var seed = CreateTestSeed();

        CanUseItemEnum result = _farm.CanUseItem(cell, seed);

        AssertObject(result).IsEqual(CanUseItemEnum.NotFarmingTile);
    }

    [TestCase]
    public void PlantSeed_OnPlowedSoil_SucceedsAndSetsStage0()
    {
        var cell = new Vector2I(2, 2);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        var seed = CreateTestSeed(CropType.Radish, duration: 4);

        bool planted = _farm.PlantSeed(cell, seed);

        AssertBool(planted).IsTrue();
        AssertBool(_testMap.PlacedCrops.ContainsKey(cell)).IsTrue();
        AssertObject(_testMap.PlacedCrops[cell].Crop).IsEqual(CropType.Radish);
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(0);
    }

    [TestCase]
    public void PlantSeed_OnFlatSoil_Fails()
    {
        var cell = new Vector2I(2, 2);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.FlatDry);
        var seed = CreateTestSeed();

        bool planted = _farm.PlantSeed(cell, seed);

        AssertBool(planted).IsFalse();
        AssertBool(_testMap.PlacedCrops.ContainsKey(cell)).IsFalse();
    }

    [TestCase]
    public void IncrementDay_CropNeedsWaterAgainEachDay()
    {
        var cell = new Vector2I(2, 2);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedWet);
        _farm.PlantSeed(cell, CreateTestSeed(duration: 4));

        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(1);
        AssertObject(_testMap.GetFarmSoilState(cell)).IsEqual(SoilState.PlowedDry);

        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(1);

        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(2);
    }

    [TestCase]
    public void IncrementDay_DryCropDoesNotGrowUntilWatered()
    {
        var cell = new Vector2I(2, 2);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        _farm.PlantSeed(cell, CreateTestSeed(duration: 4));

        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(0);

        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(1);
    }

    [TestCase]
    public void WorldClock_NewDayAdvancesWateredCrop()
    {
        var world = GD.Load<PackedScene>("res://scenes/world/World.tscn").Instantiate<Node>();
        ((SceneTree)Engine.GetMainLoop()).Root.AddChild(world);
        try
        {
            var farm = world.GetNode<FarmManager>("FarmManager");
            var map = world.GetNode<OutsideMap>("OutsideMap");
            var clock = world.GetNode<Timemanger>("TimeManager");
            var timer = clock.GetNode<Timer>("WorldTimer");
            timer.Stop();

            Vector2I? farmableCell = null;
            foreach (var cell in map.GroundLayer.GetUsedCells())
            {
                if (map.IsGrass(cell) && !map.HasObstacle(cell))
                {
                    farmableCell = cell;
                    break;
                }
            }
            AssertBool(farmableCell.HasValue).IsTrue();
            var target = farmableCell!.Value;
            map.SetSoil(target, SoilState.PlowedWet);
            AssertBool(farm.PlantSeed(target, CreateTestSeed(duration: 4))).IsTrue();
            AssertInt(map.CropsLayer.GetCellAtlasCoords(target).X).IsEqual(0);

            for (int i = 0; i < 16; i++)
                timer.EmitSignal(Timer.SignalName.Timeout);
            AssertInt((int)clock.CurrentDay).IsEqual(1);

            AssertInt(map.CropsLayer.GetCellAtlasCoords(target).X).IsEqual(1);
        }
        finally
        {
            world.Free();
        }
    }

    [TestCase]
    public void IncrementDay_AdvancesCropStageToMaxStage()
    {
        var cell = new Vector2I(2, 2);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        var seed = CreateTestSeed(CropType.Carrot, duration: 4);

        _farm.PlantSeed(cell, seed);
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(0);

        // Day 1
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(1);

        // Day 2
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(2);

        // Day 3
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(3);

        // Day 4 (Finished/Ripe)
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(3);

        // Beyond Day 4 stays capped at stage 3
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cell].Stage).IsEqual(3);
    }

    [TestCase]
    public void IncrementDay_MultipleCropsGrowIndependently()
    {
        var cellA = new Vector2I(1, 1);
        var cellB = new Vector2I(1, 2);
        _testMap.GrassCells.Add(cellA);
        _testMap.GrassCells.Add(cellB);
        _testMap.SetSoil(cellA, SoilState.PlowedDry);
        _testMap.SetSoil(cellB, SoilState.PlowedDry);
        var seedA = CreateTestSeed(CropType.Radish, duration: 4);
        var seedB = CreateTestSeed(CropType.Tomato, duration: 4);

        // Plant Crop A on Day 0
        _farm.PlantSeed(cellA, seedA);

        // Advance 1 day
        _farm.UseTool(cellA, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cellA].Stage).IsEqual(1);

        // Plant Crop B on Day 1
        _farm.PlantSeed(cellB, seedB);
        AssertInt(_testMap.PlacedCrops[cellB].Stage).IsEqual(0);

        // Advance another day: Crop A should be stage 2, Crop B should be stage 1
        _farm.UseTool(cellA, _wateringCan);
        _farm.UseTool(cellB, _wateringCan);
        _farm.IncrementDay();
        AssertInt(_testMap.PlacedCrops[cellA].Stage).IsEqual(2);
        AssertInt(_testMap.PlacedCrops[cellB].Stage).IsEqual(1);
    }

    [TestCase]
    public void CanUseItem_BasketOnUnripeCrop_ReturnsNo()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        var seed = CreateTestSeed(CropType.Radish, duration: 4);
        _farm.PlantSeed(cell, seed);

        CanUseItemEnum result = _farm.CanUseItem(cell, _basket);

        AssertObject(result).IsEqual(CanUseItemEnum.No);
    }

    [TestCase]
    public void CanUseItem_BasketOnRipeCrop_ReturnsYes()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        var seed = CreateTestSeed(CropType.Radish, duration: 2);
        _farm.PlantSeed(cell, seed);

        // Advance to ripe
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();

        CanUseItemEnum result = _farm.CanUseItem(cell, _basket);

        AssertObject(result).IsEqual(CanUseItemEnum.Yes);
    }

    [TestCase]
    public void UseTool_HarvestRipeCrop_ClearsMapAndReturnsCropItemWithIcon()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        var seed = CreateTestSeed(CropType.Radish, duration: 2);
        _farm.PlantSeed(cell, seed);

        // Advance to ripe
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();
        _farm.UseTool(cell, _wateringCan);
        _farm.IncrementDay();

        UseToolResult result = _farm.UseTool(cell, _basket);

        AssertBool(result.Success).IsTrue();
        AssertObject(result.HarvestedCrops).IsNotNull();
        AssertObject(result.HarvestedCrops!.Icon).IsNotNull();
        AssertObject(result.HarvestedCrops!.DisplayName).IsEqual("Radish");
        AssertInt((int)result.HarvestedCrops!.MaxStack).IsEqual(99);
        AssertBool(_testMap.PlacedCrops.ContainsKey(cell)).IsFalse();
    }

    [TestCase]
    public void UseTool_HarvestUnripeCrop_Fails()
    {
        var cell = new Vector2I(3, 3);
        _testMap.GrassCells.Add(cell);
        _testMap.SetSoil(cell, SoilState.PlowedDry);
        var seed = CreateTestSeed(CropType.Radish, duration: 4);
        _farm.PlantSeed(cell, seed);

        UseToolResult result = _farm.UseTool(cell, _basket);

        AssertBool(result.Success).IsFalse();
        AssertObject(result.HarvestedCrops).IsNull();
        AssertBool(_testMap.PlacedCrops.ContainsKey(cell)).IsTrue();
    }

}
