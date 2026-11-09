using Godot;
using GdUnit4;
using static GdUnit4.Assertions;
using TootieFarm.Inventory;
using TootieFarm.Items;

namespace TootieFarm.Tests;

[TestSuite]
public class InventoryManagerTests
{
    private InventoryManager _inventory = null!;

    [BeforeTest]
    public void Setup()
    {
        _inventory = new InventoryManager();
        _inventory._Ready();
    }

    [AfterTest]
    public void Teardown()
    {
        _inventory.Free();
    }

    [TestCase]
    public void Ready_InitializesFixedCapacityAndStarterItems()
    {
        AssertInt(_inventory.Items.Count).IsEqual(InventoryManager.MaxSlots);
        AssertInt(_inventory.SelectedSlot).IsEqual(0);
        AssertObject(_inventory.GetSelectedItem()).IsNotNull();
        AssertObject(_inventory.GetSelectedItem()!.Item).IsEqual(ItemRepository._toolhoe);

        // Slots 8 and 9 are empty
        AssertObject(_inventory.Items[8]).IsNull();
        AssertObject(_inventory.Items[9]).IsNull();
    }

    [TestCase]
    public void SelectedSlot_WithinBounds_UpdatesSelectionAndEmitsSignal()
    {
        ushort emittedSlot = ushort.MaxValue;
        _inventory.SelectedItemChanged += slot => emittedSlot = slot;

        _inventory.SelectedSlot = 3;

        AssertInt(_inventory.SelectedSlot).IsEqual(3);
        AssertInt((int)emittedSlot).IsEqual(3);
        AssertObject(_inventory.GetSelectedItem()!.Item).IsEqual(ItemRepository._seedsRadish);
    }

    [TestCase]
    public void SelectedSlot_OnEmptySlot_ReturnsNullSelectedItem()
    {
        _inventory.SelectedSlot = 8;

        AssertInt(_inventory.SelectedSlot).IsEqual(8);
        AssertObject(_inventory.GetSelectedItem()).IsNull();
    }

    [TestCase]
    public void SelectedSlot_OutOfBounds_Ignored()
    {
        _inventory.SelectedSlot = 2;

        // Try setting beyond MaxSlots
        _inventory.SelectedSlot = (ushort)InventoryManager.MaxSlots;
        AssertInt(_inventory.SelectedSlot).IsEqual(2);

        _inventory.SelectedSlot = 99;
        AssertInt(_inventory.SelectedSlot).IsEqual(2);
    }

    [TestCase]
    public void AddItem_ExistingStackableItem_IncrementsCountAndEmitsSignal()
    {
        bool inventoryChangedEmitted = false;
        _inventory.InventoryChanged += () => inventoryChangedEmitted = true;

        uint initialCount = _inventory.Items[3]!.Count; // Radish seeds

        _inventory.AddItem(ItemRepository._seedsRadish);

        AssertInt((int)_inventory.Items[3]!.Count).IsEqual((int)initialCount + 1);
        AssertBool(inventoryChangedEmitted).IsTrue();
    }

    [TestCase]
    public void AddItem_NewItem_FillsFirstEmptySlot()
    {
        var newItem = new SeedItem
        {
            Id = "seed_strawberry",
            DisplayName = "Strawberry Seeds",
            MaxStack = 99
        };

        // Slot 8 is first empty slot
        AssertObject(_inventory.Items[8]).IsNull();

        _inventory.AddItem(newItem);

        AssertObject(_inventory.Items[8]).IsNotNull();
        AssertObject(_inventory.Items[8]!.Item).IsEqual(newItem);
        AssertInt((int)_inventory.Items[8]!.Count).IsEqual(1);
    }

    [TestCase]
    public void AddItem_WhenInventoryFull_DoesNotCrashOrOverwrite()
    {
        var item1 = new SeedItem { Id = "seed_1", DisplayName = "Seed 1", MaxStack = 99 };
        var item2 = new SeedItem { Id = "seed_2", DisplayName = "Seed 2", MaxStack = 99 };
        var overflowItem = new SeedItem { Id = "seed_overflow", DisplayName = "Overflow", MaxStack = 99 };

        // Fill slot 8 and 9
        _inventory.AddItem(item1);
        _inventory.AddItem(item2);

        // All 10 slots now full
        for (int i = 0; i < InventoryManager.MaxSlots; i++)
        {
            AssertObject(_inventory.Items[i]).IsNotNull();
        }

        // Try to add when full
        _inventory.AddItem(overflowItem);

        // Verify none of the 10 slots contain overflowItem
        bool containsOverflow = _inventory.Items.Exists(x => x?.Item == overflowItem);
        AssertBool(containsOverflow).IsFalse();
    }

    [TestCase]
    public void DecrementItem_StackableItemWithMultiple_ReducesCount()
    {
        uint initialCount = _inventory.Items[3]!.Count; // Radish seeds (5)
        AssertBool(initialCount > 1).IsTrue();

        _inventory.DecrementItem(ItemRepository._seedsRadish);

        AssertObject(_inventory.Items[3]).IsNotNull();
        AssertInt((int)_inventory.Items[3]!.Count).IsEqual((int)initialCount - 1);
    }

    [TestCase]
    public void DecrementItem_LastItem_ClearsSlotToNull()
    {
        var singleItem = new SeedItem { Id = "single_seed", DisplayName = "Single Seed", MaxStack = 99 };
        _inventory.AddItem(singleItem); // Goes into slot 8 with Count = 1

        AssertObject(_inventory.Items[8]).IsNotNull();
        AssertInt((int)_inventory.Items[8]!.Count).IsEqual(1);

        _inventory.DecrementItem(singleItem);

        AssertObject(_inventory.Items[8]).IsNull();
    }

    [TestCase]
    public void DecrementItem_NonExistentItem_DoesNothing()
    {
        var missingItem = new ToolItem { Id = "missing_tool", DisplayName = "Missing Tool" };

        // Should not throw or modify inventory
        _inventory.DecrementItem(missingItem);

        AssertInt(_inventory.Items.Count).IsEqual(InventoryManager.MaxSlots);
    }
}
