using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ItemToSprite
{
    public CupFilment filment;
    public Sprite sprite;
}

public class UIOrder : MonoBehaviour
{
    public Image[] images;
    public ItemToSprite[] imageToSprite;

    public Sprite FilmentToSprite(CupFilment filment)
    {
        for (int i = 0; i < imageToSprite.Length; i++)
        {
            if (imageToSprite[i].filment.Equals(filment))
            {
                return imageToSprite[i].sprite;
            }
        }

        return null;
    }
    
    public void ShowOrder(OrderToFill order)
    {
        images[0].sprite = FilmentToSprite(order.filments[0]);
        images[1].sprite = FilmentToSprite(order.filments[1]);
        images[2].sprite = FilmentToSprite(order.filments[2]);
    }
}