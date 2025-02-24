using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class KettleStation : Interactable, IItemHolder
{
    public GameConfig gameConfig;
    public TMP_Text label;
    public Transform itemOrigin;
    public AudioClip bubbleAudioClip;
    public AudioClip doneAudioClip;
    public GameObject flames;
    public HighlightableMesh highlightableMesh;
    public Item kettle;

    public Item HoldedItem => throw new NotImplementedException();

    void Start()
    {
        if (kettle != null)
        {
            Utils.HoldAction(this, kettle);
        }
    }

    void Update()
    {
        flames.SetActive(kettle != null && kettle.itemInKettle != null && kettle.itemInKettle.itemType.Steepable());


        if (kettle != null)
        {
            if (kettle.itemInKettle != null && kettle.itemInKettle.itemType.Steepable())
            {

                bool donePre = kettle.itemInKettle.cookingTimer >= gameConfig.fruitCookTime; ;
                kettle.itemInKettle.cookingTimer += Time.deltaTime;
                bool done = kettle.itemInKettle.cookingTimer >= gameConfig.fruitCookTime;
                label.text = $"{(done ? "DONE" : "")} {kettle.itemInKettle.cookingTimer:N2}";

                bool doneThisFrame = donePre == false && done;

                if (doneThisFrame)
                {
                    GameManager.PlaySingle(doneAudioClip);
                    var fruit = kettle.itemInKettle;
                    var cookedVersion = gameConfig.GetDoneVersion(kettle.itemInKettle).gameObject;
                    var cookedItem = Instantiate(cookedVersion).GetComponent<Item>();
                    Utils.HoldAction(this.kettle, cookedItem);
                    Destroy(fruit.gameObject);
                }

                if (kettle.itemInKettle.steepingTimer > gameConfig.burnTime)
                {
                    var fruit = kettle.itemInKettle;
                    var coalItem = Instantiate(gameConfig.coal.gameObject).GetComponent<Item>();
                    Utils.HoldAction(this.kettle, coalItem);
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
        if (kettle == null && player.IsHoldingItem())
        {
            if (player.holdedItem.itemType == ItemType.KETTLE)
            {
                HoldItem(player.holdedItem);
                player.ReleaseItem();
                kettle.SetHoldedBy(this);
            }
        }
        else if (kettle != null)
        {
            if (player.IsHoldingItem() == false)
            {
                this.GiveItemTo(player);
            }
            else if (player.holdedItem.itemType.Steepable() && kettle.itemInKettle == null)
            {
                GameManager.PlaySingle(bubbleAudioClip);
                player.GiveItemTo(kettle);
            }
        }
    }

    public void HoldItem(Item item)
    {
        kettle = item;
        kettle.transform.position = itemOrigin.position;
        kettle.rigidbody.linearVelocity = Vector3.zero;
        kettle.rigidbody.isKinematic = true;
    }

    public Item ReleaseItem()
    {
        kettle.rigidbody.isKinematic = false;
        var temp = kettle;
        kettle = null;
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