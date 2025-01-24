using UnityEditor;

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

    public static bool Cookable(this ItemType itemType)
    {
        return itemType == ItemType.CHOOPED_FRUIT || itemType == ItemType.COOKED_TAPIOCA;
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
        return itemType == ItemType.TEA || itemType == ItemType.MILK || itemType == ItemType.COOKED_FRUIT;
    }
}