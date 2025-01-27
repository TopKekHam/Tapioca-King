using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public interface IItemHolder
{
    void HoldItem(Item item);
    Item ReleaseItem();
    Item HoldedItem { get; }
}

public class CookingStove : Interactable, IItemHolder
{
    public GameConfig gameConfig;
    public TMP_Text label;
    public Transform itemOrigin;
    public AudioClip bubbleAudioClip;
    public AudioClip doneAudioClip;
    public GameObject flames;
    public HighlightableMesh highlightableMesh;
    public Item pot;

    public Item HoldedItem => pot;

    void Start()
    {
        if (pot != null)
        {
            Utils.HoldAction(this, pot);
        }
    }

    void Update()
    {
        flames.SetActive(pot != null && pot.itemInPot != null && pot.itemInPot.itemType.IsCookable());

        if (pot != null)
        {
            if (pot.itemInPot != null)
            {

                if (pot.itemInPot.itemType.IsCookable())
                {
                    pot.itemInPot.cookingTimer += Time.deltaTime;
                    label.text = $"{pot.itemInPot.cookingTimer:N2}";

                    if (pot.itemInPot.cookingTimer > gameConfig.fruitCookTime)
                    {
                        GameManager.PlaySingle(doneAudioClip);
                        var fruit = pot.itemInPot;
                        var cookedVersion = gameConfig.GetDoneVersion(pot.itemInPot).gameObject;
                        var cookedItem = Instantiate(cookedVersion).GetComponent<Item>();
                        Utils.HoldAction(this.pot, cookedItem);
                        Destroy(fruit.gameObject);

                    }
                }
                else if (pot.itemInPot.itemType.IsCooked())
                {
                    pot.itemInPot.cookingTimer += Time.deltaTime;
                    label.text = $"DONE {pot.itemInPot.cookingTimer:N2}";

                    if (pot.itemInPot.cookingTimer > gameConfig.burnTime)
                    {
                        var cookedItem = pot.itemInPot;
                        var coalItem = Instantiate(gameConfig.coal.gameObject).GetComponent<Item>();
                        Utils.HoldAction(this.pot, coalItem);
                        Destroy(cookedItem.gameObject);
                        label.text = "";
                    }
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
        if (GotPot() == false && player.IsHoldingItem())
        {
            if (player.holdedItem.itemType == ItemType.POT)
            {
                HoldItem(player.holdedItem);
                player.ReleaseItem();
                pot.SetHoldedBy(this);
            }
        }
        else if (GotPot())
        {
            if (player.IsHoldingItem())
            {
                if (player.holdedItem.itemType == ItemType.CUP && pot.PotIsFull())
                {
                    Utils.TryFill(player.holdedItem, this);
                }
                else if (player.holdedItem.itemType.IsCookable() && pot.itemInPot == null)
                {
                    GameManager.PlaySingle(bubbleAudioClip);
                    player.GiveItemTo(pot);
                }
            }
            else
            {
                this.GiveItemTo(player);
            }
        }
    }

    public bool GotPot()
    {
        return pot != null;
    }

    public void HoldItem(Item item)
    {
        pot = item;
        pot.transform.parent = itemOrigin;
        pot.transform.localPosition = Vector3.zero;
        pot.transform.localRotation = Quaternion.identity;

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

    public override void Highlight()
    {
        highlightableMesh.Highlight();
    }

    public override void DeHighlight()
    {
        highlightableMesh.DeHighlight();
    }
}