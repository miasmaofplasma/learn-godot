using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using TootieFarm.Farming;
using TootieFarm.Inventory;
using TootieFarm.Items;
using GamePlayer = TootieFarm.Player.Player;

namespace TootieFarm.Tests;

[TestSuite]
[RequireGodotRuntime]
public class PlayerTests
{
    private TestOutsideMap _map = null!;
    private FarmManager _farm = null!;
    private InventoryManager _inventory = null!;
    private GamePlayer _player = null!;
    private TileSelector _selector = null!;
    private readonly Vector2I _cell = new(0, 0);

    [BeforeTest]
    public void Setup()
    {
        _map = new TestOutsideMap();
        _farm = new FarmManager { _outsideMap = _map };
        _inventory = new InventoryManager();
        _inventory._Ready();
        _player = GD.Load<PackedScene>("res://src/player/player.tscn").Instantiate<GamePlayer>();
        _player.farmManager = _farm;
        _player._inventory = _inventory;
        _map.GrassCells.Add(_cell);
        ((SceneTree)Engine.GetMainLoop()).Root.AddChild(_player);
        _player.SetPhysicsProcess(false);
        _selector = _player.GetNode<TileSelector>("TileSelector");
    }

    [AfterTest]
    public void Teardown()
    {
        Input.ActionRelease("move_right");
        _player.Free();
        _inventory.Free();
        _farm.Free();
        _map.Free();
    }

    private void UseItem()
    {
        using var action = new InputEventAction { Action = "use_tool", Pressed = true };
        _player._UnhandledInput(action);
    }

    [TestCase]
    public void UseTool_OnGrass_TillsSelectedCell()
    {
        UseItem();

        AssertObject(_map.GetFarmSoilState(_cell)).IsEqual(SoilState.FlatDry);
    }

    [TestCase]
    public void PlantSeed_OnPlowedSoil_ConsumesSeedAndPlacesCrop()
    {
        _map.SetSoil(_cell, SoilState.PlowedDry);
        _inventory.SelectedSlot = 3;
        var count = _inventory.Items[3]!.Count;

        UseItem();

        AssertBool(_map.PlacedCrops.ContainsKey(_cell)).IsTrue();
        AssertObject(_map.PlacedCrops[_cell].Crop).IsEqual(CropType.Radish);
        AssertInt((int)_inventory.Items[3]!.Count).IsEqual((int)count - 1);
    }

    [TestCase]
    public void PlantSeed_OnUnplowedSoil_DoesNotConsumeSeed()
    {
        _inventory.SelectedSlot = 3;
        var count = _inventory.Items[3]!.Count;

        UseItem();

        AssertBool(_map.PlacedCrops.ContainsKey(_cell)).IsFalse();
        AssertInt((int)_inventory.Items[3]!.Count).IsEqual((int)count);
    }

    [TestCase]
    public void HarvestMatureCrop_AddsCropToInventory()
    {
        _map.SetSoil(_cell, SoilState.PlowedDry);
        _inventory.SelectedSlot = 3;
        var seed = (SeedItem)_inventory.GetSelectedItem()!.Item;
        UseItem();
        for (int day = 0; day < seed.GrownCrop.CropCycles.GrowthDaysDuration; day++)
        {
            _farm.UseTool(_cell, ItemRepository._toolWateringCan);
            _farm.IncrementDay();
        }
        _inventory.SelectedSlot = 2;

        UseItem();

        AssertBool(_map.PlacedCrops.ContainsKey(_cell)).IsFalse();
        AssertObject(_inventory.Items[8]!.Item).IsEqual(ItemRepository.GetCropItem(CropType.Radish));
        AssertInt((int)_inventory.Items[8]!.Count).IsEqual(1);
    }

    [TestCase]
    public void SelectEmptySlot_HidesSelectorAndDoesNotUsePreviousTool()
    {
        _inventory.SelectedSlot = 8;

        UseItem();

        AssertBool(_selector.Visible).IsFalse();
        AssertObject(_map.GetFarmSoilState(_cell)).IsEqual(SoilState.Cleared);
    }

    [TestCase]
    public void Selection_UpdatesSelectorForUsableAndBlockedTiles()
    {
        AssertBool(_selector.Visible).IsTrue();
        AssertObject(_selector.BorderColor).IsEqual(Colors.Green);

        _inventory.SelectedSlot = 3;

        AssertBool(_selector.Visible).IsTrue();
        AssertObject(_selector.BorderColor).IsEqual(Colors.Red);
        _map.GrassCells.Clear();
        _inventory.SelectedSlot = 0;
        AssertBool(_selector.Visible).IsFalse();
    }

    [TestCase]
    public void MovingRight_ChangesVelocityFacingAndSelectorPosition()
    {
        Input.ActionPress("move_right");
        _player._PhysicsProcess(1.0 / 60.0);

        AssertObject(_player.Velocity).IsEqual(new Vector2(_player.Speed, 0));
        AssertObject(_selector.TargetPosition).IsEqual(_player.GlobalPosition + new Vector2(16, 0));
        AssertObject(_player.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation).IsEqual((StringName)"walk_right");

        Input.ActionRelease("move_right");
        _player._PhysicsProcess(1.0 / 60.0);
        AssertObject(_player.Velocity).IsEqual(Vector2.Zero);
        AssertObject(_player.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation).IsEqual((StringName)"idle_right");
    }
}
