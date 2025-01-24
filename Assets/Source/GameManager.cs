using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class OrderToFill
{
    public CupFilment[] filments;
}

public enum GameState
{
    LOBBY = 1,
    PLAYING = 2,
}

public class GameManager : MonoBehaviour
{
    public GameConfig gameConfig;
    public static GameManager instance;
    public GameState gameState = GameState.LOBBY;
    public StartGameButton startGameButton;

    [HideInInspector] public List<OrderToFill> orderToFill;

    float orderTimer = 0;
    int playerCount = 1;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        orderToFill = new List<OrderToFill>();
        orderTimer = gameConfig.orderTimers[playerCount];
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.PLAYING: UpdatePlaying(); break;
            case GameState.LOBBY: break;
        }
    }

    void UpdatePlaying()
    {
        orderTimer += Time.deltaTime;

        while (gameConfig.orderTimers[playerCount] <= orderTimer)
        {
            orderTimer -= gameConfig.orderTimers[playerCount];
            orderToFill.Add(GetRandomOrder());
        }
    }

    public void BeginGame()
    {
        gameState = GameState.PLAYING;
    }

    public OrderToFill GetRandomOrder()
    {
        OrderToFill orderToFill = new OrderToFill();

        orderToFill.filments = new CupFilment[3];

        // Tea or milk

        int teaOrMilk = Random.Range(0, 2);

        if (teaOrMilk == 0)
        {
            // tea
            orderToFill.filments[0] = new CupFilment()
            {
                itemType = ItemType.TEA,
                teaType = (TeaType)Random.Range(0, 3),
            };
        }
        else
        {
            // milk
            orderToFill.filments[0] = new CupFilment()
            {
                itemType = ItemType.MILK,
                milkType = (MilkType)Random.Range(0, 2),
            };
        }

        orderToFill.filments[1] = new CupFilment()
        {
            itemType = ItemType.COOKED_FRUIT,
            fruitType = (FruitType)Random.Range(0, 3),
        };

        orderToFill.filments[2] = new CupFilment()
        {
            itemType = ItemType.COOKED_TAPIOCA,
        };

        return orderToFill;
    }
}