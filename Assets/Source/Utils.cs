public static class Utils
{

    public static void SwapWith(this IItemHolder from, IItemHolder to)
    {
        var item = from.ReleaseItem();
        item.SetHoldedBy(to);
        to.HoldItem(item);
    }
    
}