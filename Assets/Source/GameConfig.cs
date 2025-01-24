using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

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
    public CupFilmentMaterial[] filmentMaterials;
    public GameObject[] filmentPrefabs;
    
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

    public GameObject InstantiateFilment(int number, CupFilment filment)
    {
        var prefab = filmentPrefabs[number];
        
        var obj = GameObject.Instantiate(prefab);
        var renderer = obj.GetComponent<MeshRenderer>();
        renderer.sharedMaterials[0] = GetFilmentMaterial(filment);

        return obj;
    }
    
    public Material GetFilmentMaterial(CupFilment filment)
    {
        foreach (var f in filmentMaterials)
        {
            if (f.filment.Equals(filment))
            {
                return f.material;
            }
        }
        
        return null;
    }
    
}
