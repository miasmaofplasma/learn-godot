using System;
using TootieFarm.Items;

namespace TootieFarm.Inventory;

public class InventoryItem
{
    public required ItemData Item { get; init; }
    public uint Count
    {
        get;
        set
        {
            if (Item.MaxStack == 1 && field == 0)
            {
                return;
            }
            else if (field > Item.MaxStack)
            {
                value = Item.MaxStack;
            }
            else
            {
                value = field;
            }
        }
    } = 1;
}
