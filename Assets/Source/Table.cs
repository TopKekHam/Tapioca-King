using UnityEngine;

public class Table : Interactable, IItemHolder
{
    [HideInInspector]
    public Item heldItem = null;

    public Transform itemPosition;
    
    public override void Interact(PlayerComponent player)
    {
        if (player.IsHoldingItem() && IsHoldingItem() == false)
        {
            player.GiveItemTo(this);    
        } else if (player.IsHoldingItem() == false && IsHoldingItem())
        {
            this.GiveItemTo(player);
        }
    }

    public bool IsHoldingItem()
    {
        return heldItem != null;
    }

    public void HoldItem(Item item)
    {
        heldItem = item;
        heldItem.transform.position = itemPosition.position;
    }

    public Item ReleaseItem()
    {
        var temp = heldItem;
       heldItem = null;
       return temp;
    }
}
