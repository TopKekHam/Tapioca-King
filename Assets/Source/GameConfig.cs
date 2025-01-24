using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class CupFilmentMaterial
{
    public CupFilment filment;
    public Material material;
}

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{

    public float fruitCookTime = 7.5f;
    public float burnTime = 12.5f;
    public Item coal;
    public int choopsToCutFruit = 10;

    public Item[] choopedVersions;
    public CupFilmentMaterial[] filments;
    
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

    public Material GetFilmentMaterial(CupFilment filment)
    {
        foreach (var f in filments)
        {
            if (f.filment.Equals(filment))
            {
                return f.material;
            }
        }
        
        return null;
    }
    
}
