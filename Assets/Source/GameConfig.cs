using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{

    public float fruitCookTime = 7.5f;
    public float burnTime = 12.5f;
    public Item coal;

}
