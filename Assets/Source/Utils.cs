public static class Utils
{

    public static void SwapWith(this IItemHolder from, IItemHolder to)
    {
        var item = from.ReleaseItem();
        item.SetHoldedBy(to);
        to.HoldItem(item);
    }

    public static void HoldAction(IItemHolder holder, Item item)
    {
        item.SetHoldedBy(holder);
        holder.HoldItem(item);
    }

    public static void DisholdItem(IItemHolder holder, Item item)
    {
        item.FreeSelf();
        holder.ReleaseItem();
    }
    
}