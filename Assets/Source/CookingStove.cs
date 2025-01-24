using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public interface IItemHolder
{
    void HoldItem(Item item);
    Item ReleaseItem();
}

public class CookingStove : Interactable, IItemHolder
{
    public GameConfig gameConfig;
    public TMP_Text label;
    [HideInInspector] public Item pot;

    void Start()
    {
    }

    void Update()
    {
        if (pot != null && pot.itemInPot != null)
        {
            pot.transform.position = transform.position;

            if (pot.itemInPot.itemType == ItemType.FRUIT)
            {
                pot.itemInPot.cookingTimer += Time.deltaTime;
                bool done = pot.itemInPot.cookingTimer >= gameConfig.fruitCookTime;
                label.text = $"{(done ? "DONE" : "")} {pot.itemInPot.cookingTimer:N2}";

                if (pot.itemInPot.cookingTimer > gameConfig.burnTime)
                {
                    var fruit = pot.itemInPot;
                    var coalItem = Instantiate(gameConfig.coal.gameObject).GetComponent<Item>();
                    Utils.HoldAction(this.pot, coalItem);
                    Destroy(fruit.gameObject);
                }
            }
            else
            {
                label.text = "";
            }
        }
        else
        {
            label.text = "";
        }
    }
    
    public override void Interact(PlayerComponent player)
    {
        if (pot == null && player.IsHoldingItem())
        {
            if (player.holdedItem.itemType == ItemType.POT)
            {
                HoldItem(player.holdedItem);
                player.ReleaseItem();
                pot.SetHoldedBy(this);
            }
        }
        else if (pot != null && player.IsHoldingItem() == false)
        {
            this.SwapWith(player);
        }
    }

    public void HoldItem(Item item)
    {
        pot = item;
        pot.transform.position = transform.position;
    }

    public Item ReleaseItem()
    {
        var temp = pot;
        pot = null;
        return temp;
    }
}