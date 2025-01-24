using UnityEngine;


public enum ItemType
{
    FRUIT = 1,
    POT = 2
}

public enum FruitType
{
    STRAWBERRY = 1,
}

[RequireComponent(typeof(Rigidbody))]
public class Item : Interactable, IItemHolder
{
    public Collider collider;
    public ItemType itemType;
    public FruitType fruitType;


    public Item potItem;
    [HideInInspector] public float cookingTimer = 0;

    private IItemHolder myHolder;
    [HideInInspector] public Rigidbody rigidbody;
    private ItemType type;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (itemType == ItemType.POT && potItem != null)
        {
            potItem.transform.position = transform.position;
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

        if (myHolder != null)
        {
            collider.isTrigger = true;
        }
        else
        {
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
            if (player.holdedItem.type == ItemType.FRUIT)
            {
                player.SwapWith(this);
            }
        }
        else if (IsHolded() == false && player.IsHoldingItem() == false)
        {
            player.HoldItem(this);
            SetHoldedBy(player);
        }
    }

    public void HoldItem(Item item)
    {
        if (itemType == ItemType.POT)
        {
            potItem = item;
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
            var temp = potItem;
            potItem = null;
            return temp;
        }
        else
        {
            Debug.Assert(false);
            return null;
        }
    }
}