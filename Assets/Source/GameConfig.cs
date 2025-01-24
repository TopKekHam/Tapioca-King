using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{

    public float fruitCookTime = 7.5f;
    public float burnTime = 12.5f;
    public Item coal;
    public int choopsToCutFruit = 10;

    public Item[] choopedVersions;
    
    public Item GetFruitChoopedVersion(FruitType type)
    {
        foreach (Item item in choopedVersions)
        {
            if (item.fruitType == type)
            {
                return item;
            }
        }
        
        Debug.Assert(false);
        return null;
    }
    
}
