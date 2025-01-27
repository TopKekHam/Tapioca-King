using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class Utils
{

    public static void GiveItemTo(this IItemHolder from, IItemHolder to)
    {
        var item = from.ReleaseItem();
        item.SetHoldedBy(to);
        to.HoldItem(item);
    }

    public static void HoldAction(this IItemHolder holder, Item item)
    {
        item.SetHoldedBy(holder);
        holder.HoldItem(item);
    }

    public static void DisholdItem(IItemHolder holder, Item item)
    {
        item.FreeSelf();
        holder.ReleaseItem();
    }

    public static bool IsCookable(this ItemType itemType)
    {
        return itemType == ItemType.CHOOPED_FRUIT || itemType == ItemType.TAPIOCA;
    }


    public static bool IsCooked(this ItemType itemType)
    {
        return itemType == ItemType.COOKED_FRUIT || itemType == ItemType.COOKED_TAPIOCA;
    }

    public static bool Steepable(this ItemType itemType)
    {
        return itemType == ItemType.TEA;
    }
    
    public static void LockFromChooping(this ChoopingBoard board, PlayerComponent playerComponent)
    {
        board.LockPlayer(playerComponent);
        playerComponent.LockChooping(board);
    }
    
    public static void ReleaseFromChooping(this ChoopingBoard board, PlayerComponent playerComponent)
    {
        board.UnlockPlayer();
        playerComponent.UnlookChooping();
    }

    public static bool IsFilment(this ItemType itemType)
    {
        return itemType == ItemType.STEEPED_TEA || itemType == ItemType.MILK || itemType == ItemType.COOKED_FRUIT || itemType == ItemType.COOKED_TAPIOCA;
    }

    public static bool TryFill(this Item cup, IItemHolder holder)
    {
        if (cup.CanAddFilment() == false) return false;

        if (holder.HoldedItem.itemType.IsFilment())
        {
            var item = holder.ReleaseItem();
            cup.AddFilment(item.CreateFilment());
            GameObject.Destroy(item.gameObject);
            return true;
        }
        else if(holder.HoldedItem.itemType == ItemType.POT && holder.HoldedItem.PotIsFull())
        {
            var item = holder.HoldedItem.ReleaseItem();
            cup.AddFilment(item.CreateFilment());
            GameObject.Destroy(item.gameObject);
            return true;
        }
        else if (holder.HoldedItem.itemType == ItemType.KETTLE && holder.HoldedItem.KettleIsFull())
        {
            var item = holder.HoldedItem.ReleaseItem();
            cup.AddFilment(item.CreateFilment());
            GameObject.Destroy(item.gameObject);
            return true;
        }

        return false;
    }

}