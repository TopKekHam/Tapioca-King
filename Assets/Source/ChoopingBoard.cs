using TMPro;
using UnityEngine;

public class ChoopingBoard : Interactable, IItemHolder
{
    public TMP_Text label;
    public GameConfig config;
    public Transform fruitOrigin;
    public AudioClip choopSound;
    [HideInInspector] public Item holdedItem = null;
    [HideInInspector] public int choopCounter = 0;

    [HideInInspector] public PlayerComponent choopingPlayer = null;

    void Start()
    {
    }

    void Update()
    {
        UpdateChoopingCouterLabel();
    }

    public override void Interact(PlayerComponent player)
    {
        if (IsLocked()) return;
        
        if (player.IsHoldingItem() && player.holdedItem.itemType == ItemType.FRUIT && IsItemOnBoard() == false)
        {
            choopCounter = 0;
            player.GiveItemTo(this);
            Utils.LockFromChooping(this, player);
        }
        else if (player.IsHoldingItem() == false && IsItemOnBoard())
        {
            Utils.LockFromChooping(this, player); 
        } 
    }

    void UpdateChoopingCouterLabel()
    {
        if (holdedItem == null || choopingPlayer == null)
        {
            label.text = "";
        }
        else
        {
            label.text = $"{choopCounter}/{config.choopsToCutFruit}";
        }
    }

    public void Choop()
    {
        GameManager.PlaySingle(choopSound);
        choopCounter += 1;

        UpdateChoopingCouterLabel();

        if (choopCounter >= config.choopsToCutFruit)
        {
            var fruit = this.ReleaseItem();
            
            var choopedFruitPrefab = config.GetFruitChoopedVersion(fruit.fruitType);
           
            Destroy(fruit.gameObject);

            var choopedFruit = Instantiate(choopedFruitPrefab.gameObject).GetComponent<Item>();

            choopingPlayer.HoldAction(choopedFruit);
            
            this.ReleaseFromChooping(choopingPlayer);
            
            UpdateChoopingCouterLabel();
        }
    }

    public bool IsLocked()
    {
        return choopingPlayer != null;
    }

    public void LockPlayer(PlayerComponent player)
    {
        choopingPlayer = player;
    }
    
    public void UnlockPlayer()
    {
        choopingPlayer = null;
    }

    public bool IsItemOnBoard()
    {
        return holdedItem != null;
    }

    public void HoldItem(Item item)
    {
        holdedItem = item;
        holdedItem.transform.position = fruitOrigin.position;
    }

    public Item ReleaseItem()
    {
        var temp = holdedItem;
        holdedItem = null;
        return temp;
    }
}