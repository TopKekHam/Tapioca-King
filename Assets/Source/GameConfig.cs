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
    public float teaSteepTime = 7.5f;
    public float burnTime = 12.5f;
    public Item coal;
    public int choopsToCutFruit = 10;

    public float gameLengthInSeconds = 60f * 10;
    
    public int maxConcurentOrders;
    public float[] orderTimers;
    public float[] pointsToPass;
    public Item[] choopedVersions;
    public Item[] cookedVersions;
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

    public Item GetDoneVersion(Item notDoneItem)
    {

        foreach (Item doneItem in cookedVersions)
        {
            if(notDoneItem.itemType == ItemType.CHOOPED_FRUIT && doneItem.itemType == ItemType.COOKED_FRUIT)
            {
                if (doneItem.fruitType == notDoneItem.fruitType)
                {
                    return doneItem;
                }
            }
            else if(notDoneItem.itemType == ItemType.TEA && doneItem.itemType == ItemType.STEEPED_TEA)
            {
                if (doneItem.teaType == notDoneItem.teaType)
                {
                    return doneItem;
                }
            }
            else if (notDoneItem.itemType == ItemType.TAPIOCA && doneItem.itemType == ItemType.COOKED_TAPIOCA)
            {
                return doneItem;
            }
        }

        Debug.Assert(false);
        return null;
    }

    public GameObject InstantiateFilment(int number, CupFilment filment)
    {
        var prefab = filmentPrefabs[number];
        
        var obj = GameObject.Instantiate(prefab);
        //var renderer = obj.GetComponent<MeshRenderer>();
        //var renderer2 = ;
        var mat = GetFilmentMaterial(filment);
        Debug.Log(mat.name);
        obj.GetComponent<Renderer>().material = mat;
        //renderer.materials[0] = mat; 

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
