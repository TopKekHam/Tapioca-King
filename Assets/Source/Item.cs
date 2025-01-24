using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public enum ItemType
{
    FRUIT = 1,
    POT = 2,
    COAL = 3,
    CHOOPED_FRUIT = 4,
    CUP = 5,
    TEA = 6,
    KETTLE = 7,
    MILK = 8,
    COOKED_FRUIT = 9,
    TAPIOCA = 10,
    COOKED_TAPIOCA = 11,
}

public enum FruitType
{
    STRAWBERRY = 1,
    BLUEBERRY = 2,
    BANANA = 3
}

public enum TeaType
{
    JASMINE = 1,
    MATCHA = 2,
    EARL_GREY = 3
}

public enum MilkType
{
    COW = 1,
    SOY = 2
}

[Serializable]
public class CupFilment
{
    public ItemType itemType;
    public FruitType fruitType;
    public MilkType milkType;
    public TeaType teaType;

    public bool Equals(CupFilment other)
    {
        if (itemType == other.itemType)
        {
            switch (itemType)
            {
                case ItemType.MILK: return milkType == other.milkType;
                case ItemType.TEA: return teaType == other.teaType;
                case ItemType.COOKED_FRUIT: return fruitType == other.fruitType;
                case ItemType.COOKED_TAPIOCA: return true;
            }
        }

        return false;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class Item : Interactable, IItemHolder
{
    public Collider collider;

    public ItemType itemType;
    public FruitType fruitType;
    public TeaType teaType;
    public MilkType milkType;
    
    public float[] filmentHeightOffest = new float[3];

    [HideInInspector] public CupFilment[] filments = new CupFilment[3];
    [HideInInspector] public int filmentsLength = 0;
    [HideInInspector] public Item itemInPot;
    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public float cookingTimer = 0;

    private IItemHolder myHolder;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (itemType == ItemType.POT && itemInPot != null)
        {
            itemInPot.transform.position = transform.position;
        }
    }


    public bool PotIsFull()
    {
        return itemInPot != null;
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
            if (player.holdedItem.itemType.Cookable())
            {
                player.GiveItemTo(this);
                this.itemInPot.transform.position = transform.position;
            }
        }
        else if (itemType == ItemType.CUP && CanAddFilment() && player.IsHoldingItem())
        {
            if (player.holdedItem.itemType.IsFilment())
            {
                var item = player.ReleaseItem();
                AddFilment(item.CreateFilment());
                Destroy(item.gameObject);
            }
            else if (player.holdedItem.itemType == ItemType.POT
                     && player.holdedItem.PotIsFull()
                     && player.holdedItem.itemInPot.itemType.IsFilment())
            {
                var item = player.holdedItem.ReleaseItem();
                AddFilment(item.CreateFilment());
                Destroy(item.gameObject);
            }
            else
            {
                // can't fill
            }
        }
        else if (IsHolded() == false && player.IsHoldingItem() == false)
        {
            SetHoldedBy(player);
            player.HoldItem(this);
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

    public CupFilment CreateFilment()
    {
        Debug.Assert(itemType.IsFilment());

        CupFilment filment = new CupFilment();
        filment.itemType = itemType;

        switch (filment.itemType)
        {
            case ItemType.MILK: filment.milkType = milkType; break;
            case ItemType.TEA: filment.teaType = teaType; break;
            case ItemType.COOKED_FRUIT: filment.fruitType = fruitType; break;
        }

        return filment;
    }

    public bool CanAddFilment()
    {
        return filmentsLength < 3;
    }

    public void AddFilment(CupFilment filment)
    {
        if (CanAddFilment() == false) return;

        filments[filmentsLength] = filment;

        var config = GameManager.instance.gameConfig;
        
        var obj = config.InstantiateFilment(filmentsLength, filment);

        
        obj.transform.parent = transform;
        obj.transform.localPosition = new Vector3(0, filmentHeightOffest[filmentsLength], 0);
        
        filmentsLength += 1;
    }
}