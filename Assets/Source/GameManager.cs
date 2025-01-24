using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameConfig gameConfig;
    public static GameManager instance;
    
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
