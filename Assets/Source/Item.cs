using UnityEngine;
using UnityEngine.Serialization;


public enum ItemType
{
    FRUIT = 1,
    POT = 2,
    COAL = 3,
}

public enum FruitType
{
    STRAWBERRY = 1
}

[RequireComponent(typeof(Rigidbody))]
public class Item : Interactable, IItemHolder
{
    public Collider collider;

    public ItemType itemType;
    public FruitType fruitType;

    [FormerlySerializedAs("potItem")] [HideInInspector] public Item itemInPot;
    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public float cookingTimer = 0;

    private IItemHolder myHolder;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (itemType == ItemType.POT && itemInPot != null)
        {
            itemInPot.transform.position = transform.position;
        }
    }

    public void FreeSelf()
    {
        myHolder = null;
        collider.isTrigger = false;
    }

    public void SetHoldedBy(IItemHolder holder)
    {
        myHolder = holder;

        Debug.Log("triggered");
        
        if (myHolder != null)
        {
            Debug.Log("trigger true");
            collider.isTrigger = true;
        }
        else
        {
            Debug.Log("trigger false");
            collider.isTrigger = false;
        }
    }

    public bool IsHolded()
    {
        return myHolder != null;
    }

    public override void Interact(PlayerComponent player)
    {
        if (itemType == ItemType.POT && player.IsHoldingItem())
        {
            if (player.holdedItem.itemType == ItemType.FRUIT)
            {
                player.GiveItemTo(this);
                this.itemInPot.transform.position = transform.position;
            }
        }
        else if (IsHolded() == false && player.IsHoldingItem() == false)
        {
            SetHoldedBy(player);
            player.HoldItem(this);
        }
        else
        {
        }
    }

    public void HoldItem(Item item)
    {
        if (itemType == ItemType.POT)
        {
            itemInPot = item;
        }
        else
        {
            Debug.Assert(false);
        }
    }

    public Item ReleaseItem()
    {
        if (itemType == ItemType.POT)
        {
            var temp = itemInPot;
            itemInPot = null;
            return temp;
        }
        else
        {
            Debug.Assert(false);
            return null;
        }
    }
}