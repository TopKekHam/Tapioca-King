using TMPro;
using UnityEngine;

public interface IItemHolder
{
    void HoldItem(Item item);
    Item ReleaseItem();
}

public class CookingStove : Interactable , IItemHolder
{
    public TMP_Text label;
    public Item holdedItem;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
        if (holdedItem != null && holdedItem.potItem != null)
        {
            holdedItem.potItem.cookingTimer += Time.deltaTime;
            label.text = $"{holdedItem.potItem.cookingTimer:N2}";
        }
        else
        {
            label.text = "";
        }
            
    }

    public override void Interact(PlayerComponent player)
    {
        if (holdedItem == null && player.IsHoldingItem())
        {
            if (player.holdedItem.itemType == ItemType.POT)
            {
                HoldItem(player.holdedItem);
                player.ReleaseItem();
                holdedItem.SetHoldedBy(this);
            }
        }
    }

    public void HoldItem(Item item)
    {
        holdedItem = item;
    }

    public Item ReleaseItem()
    {
        var temp = holdedItem;
        holdedItem = null;
        return temp;
    }
}
