using UnityEngine;

public class DrinkCounter : Interactable
{
    public override void DeHighlight()
    {
        
    }

    public override void Highlight()
    {
        
    }

    public override void Interact(PlayerComponent player)
    {
        if (player.IsHoldingItem() && player.holdedItem.itemType == ItemType.CUP)
        {
            GameManager.instance.ServeDrink(player.ReleaseItem());
        }
    }
}
