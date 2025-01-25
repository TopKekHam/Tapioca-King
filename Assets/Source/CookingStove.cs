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
    public Transform itemOrigin;
    public Item startingPot;
    [HideInInspector] public Item pot;

    void Start()
    {
        this.HoldAction(startingPot);
    }

    void Update()
    {
        if (pot != null)
        {
            if (pot.itemInPot != null && pot.itemInPot.itemType.Cookable())
            {
                bool donePre = pot.itemInPot.cookingTimer >= gameConfig.fruitCookTime; ;
                pot.itemInPot.cookingTimer += Time.deltaTime;
                bool done = pot.itemInPot.cookingTimer >= gameConfig.fruitCookTime;
                label.text = $"{(done ? "DONE" : "")} {pot.itemInPot.cookingTimer:N2}";

                bool doneThisFrame = donePre == false && done;
                if(doneThisFrame)
                {
                    var fruit = pot.itemInPot;
                    var cookedVersion = gameConfig.GetCookedVersion(pot.itemInPot).gameObject;
                    var cookedItem = Instantiate(cookedVersion).GetComponent<Item>();
                    Utils.HoldAction(this.pot, cookedItem);
                    Destroy(fruit.gameObject);
                }

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
        else if (pot != null)
        {
            if (player.IsHoldingItem() == false)
            {
                this.GiveItemTo(player);
            }
            else if (player.holdedItem.itemType.Cookable() && pot.itemInPot == null)
            {
                player.GiveItemTo(pot);
            }
        }
    }

    public void HoldItem(Item item)
    {
        pot = item;
        pot.transform.position = itemOrigin.position;
        pot.rigidbody.linearVelocity = Vector3.zero;
        pot.rigidbody.isKinematic = true;
    }

    public Item ReleaseItem()
    {
        pot.rigidbody.isKinematic = false;
        var temp = pot;
        pot = null;
        return temp;
    }
}