using UnityEngine;

public class ItemSpawner : Interactable
{
    
    public Item itemPrefab;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void Interact(PlayerComponent player)
    {
        if (player.IsHoldingItem() == false)
        {
            var newItem = Instantiate(itemPrefab);
            player.HoldItem(newItem.GetComponent<Item>());
        }
    }
}
