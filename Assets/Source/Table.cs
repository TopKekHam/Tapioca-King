using UnityEngine;

public class Table : Interactable, IItemHolder
{

    public Item itemOnTable = null;

    public Transform itemPosition;

    public Item HoldedItem => itemOnTable;

    void Start()
    {
        if (itemOnTable != null)
        {
            Utils.HoldAction(this, itemOnTable);
        }
    }

    public override void Interact(PlayerComponent player)
    {
        if (player.IsHoldingItem())
        {
            if (ItemOnTable())
            {
                itemOnTable.Interact(player);
            }
            else
            {
                player.GiveItemTo(this);
            }
        }
        else if (player.IsHoldingItem() == false && ItemOnTable())
        {
            this.GiveItemTo(player);
        }
    }

    public bool ItemOnTable()
    {
        return itemOnTable != null;
    }

    public void HoldItem(Item item)
    {
        itemOnTable = item;
        itemOnTable.transform.parent = itemPosition;
        itemOnTable.transform.localRotation = Quaternion.identity;
        itemOnTable.transform.localPosition = Vector3.zero;
    }

    public Item ReleaseItem()
    {
        var temp = itemOnTable;
        itemOnTable = null;
        return temp;
    }

    public override void Highlight()
    {
        
    }

    public override void DeHighlight()
    {

    }
}
