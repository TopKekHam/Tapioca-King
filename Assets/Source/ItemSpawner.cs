using UnityEngine;

public class ItemSpawner : Interactable
{
    
    public Item itemPrefab;
    public AudioClip interactAudio;
    
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
            Utils.HoldAction(player, newItem.GetComponent<Item>());

            if(interactAudio != null)
            {
                GameManager.PlaySingle(interactAudio);
            }
        }
    }
}
