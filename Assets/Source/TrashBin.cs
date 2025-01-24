using UnityEngine;

public class TrashBin : Interactable
{
    
    public override void Interact(PlayerComponent player)
    {
        if (player.holdedItem.itemType == ItemType.POT && player.holdedItem != null)
        {
            var item = player.holdedItem.itemInPot;
            Utils.DisholdItem(player.holdedItem, player.holdedItem.itemInPot);
            Destroy(item.gameObject);
        }
        else if (player.holdedItem.itemType != ItemType.POT && player.holdedItem != null)
        {
            var item = player.holdedItem;
            Utils.DisholdItem(player, player.holdedItem);
            Destroy(item.gameObject);
        }
    }
}