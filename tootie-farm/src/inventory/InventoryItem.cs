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
            if (Item.MaxStack == 1 && value == 0)
            {
                return;
            }
            else if (value > Item.MaxStack)
            {
                field = Item.MaxStack;
            }
            else
            {
                field = value;
            }
        }
    } = 1;
}
