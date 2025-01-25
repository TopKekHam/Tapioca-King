using TMPro;
using UnityEngine;

public class UIOrder : MonoBehaviour
{
    public TMP_Text[] text;

    public void ShowOrder(OrderToFill order)
    {
        text[0].text = RowToString(order.filments[0]);
        text[1].text = RowToString(order.filments[1]);
        text[2].text = RowToString(order.filments[2]);
    }

    public string RowToString(CupFilment filment)
    {
        string str = "";
        
        switch (filment.itemType)
        {
            case ItemType.COOKED_FRUIT:
            {
                str += $"{filment.fruitType}";
            }
                break;
            case ItemType.TEA:
            {
                str += $"{filment.teaType}";
            }
                break;
            case ItemType.MILK:
            {
                str += $"{filment.milkType}";
            }
                break;
            default:
            {
                str += $"{filment.itemType} ";
            }
                break;
        }

        return str;
    }
}