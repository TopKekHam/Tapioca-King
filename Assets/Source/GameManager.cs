using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class OrderToFill
{
    public CupFilment[] filments;
    public UIOrder uiElement;
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
    public float gameTimer = 0;
    public UIOrder uiOrderPrefab;
    public Transform uiOrderParent;
    public int score = 0;
    public Transform[] spawnPoints;
    public PlayerComponent playerPrefab;
    public Transform entitiesOrigin;
    public AudioSource audioSource;
    public AudioClip audioGameOver;
    public AudioClip audioGameOverAboutToBeOver;
    public GameObject winPanel, lostPanel;
    [HideInInspector] public List<OrderToFill> ordersToFill;

    float orderTimer = 0;
    [HideInInspector] public int playerCount = 0;

    bool playedOverSound = false;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ordersToFill = new List<OrderToFill>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.PLAYING:
            {
                UpdatePlaying();
            }
                break;
            case GameState.LOBBY:
            {
            }
                break;
        }
    }

    void UpdatePlaying()
    {
        gameTimer -= Time.deltaTime;
        orderTimer += Time.deltaTime;

        while (gameConfig.orderTimers[playerCount] <= orderTimer && gameConfig.maxConcurentOrders > ordersToFill.Count)
        {
            orderTimer -= gameConfig.orderTimers[playerCount];

            var order = GetRandomOrder();
            order.uiElement = Instantiate(uiOrderPrefab, uiOrderParent).GetComponent<UIOrder>();
            order.uiElement.ShowOrder(order);
            ordersToFill.Add(order);
        }

        if(playedOverSound == false && gameTimer < 60)
        {
            playedOverSound = true;
            PlaySingle(audioGameOverAboutToBeOver);
        }

        if (gameTimer <= 0)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        gameState = GameState.LOBBY;

        PlaySingle(audioGameOver);

        foreach (var order in ordersToFill)
        {
            Destroy(order.uiElement.gameObject);
        }
        
        ordersToFill.Clear();
        
        if (score >= gameConfig.pointsToPass[playerCount])
        {
            ShowWin();
        }
        else
        {
            ShowLose();
        }
    }

    void ShowWin()
    {
        winPanel.SetActive(true);
        StartCoroutine(HideWinLosePanels());
    }

    void ShowLose()
    {
        lostPanel.SetActive(true);
        StartCoroutine(HideWinLosePanels());
    }

    IEnumerator HideWinLosePanels()
    {
        yield return new WaitForSeconds(5);
        winPanel.SetActive(false);
        lostPanel.SetActive(false);
    }
    
    public void BeginGame()
    {
        playedOverSound = false;
        gameState = GameState.PLAYING;
        gameTimer = gameConfig.gameLengthInSeconds;
        orderTimer = gameConfig.orderTimers[playerCount];
        score = 0;
    }

    public void ServeDrink(Item cupItem)
    {
        var filments = cupItem.filments;

        Destroy(cupItem.gameObject);

        if (ordersToFill.Count == 0) return;

        var order = ordersToFill[0];

        for (int i = 0; i < ordersToFill.Count; i++)
        {
            if (filments[i] == null)
            {
                AnimateWrongDrinkServed();
                return;
            }

            if (filments[i].Equals(order.filments[i]) == false)
            {
                AnimateWrongDrinkServed();
                return;
            }
        }

        Destroy(ordersToFill[0].uiElement.gameObject);
        ordersToFill.RemoveAt(0);
        score += 100;
        AnimateGoodDrinkServed();
    }

    void AnimateGoodDrinkServed()
    {
        Debug.Log("Animating good drink served");
    }

    void AnimateWrongDrinkServed()
    {
        Debug.Log("Animate wrong drink served");
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
            orderToFill.filments[2] = new CupFilment()
            {
                itemType = ItemType.STEEPED_TEA,
                teaType = (TeaType)Random.Range(1, 4),
            };
        }
        else
        {
            // milk
            orderToFill.filments[2] = new CupFilment()
            {
                itemType = ItemType.MILK,
                milkType = (MilkType)Random.Range(1, 3),
            };
        }

        orderToFill.filments[1] = new CupFilment()
        {
            itemType = ItemType.COOKED_FRUIT,
            fruitType = (FruitType)Random.Range(1, 4),
        };

        orderToFill.filments[0] = new CupFilment()
        {
            itemType = ItemType.COOKED_TAPIOCA,
        };

        return orderToFill;
    }

    private int playerId = 0;


    public int OnPlayerJoin(GamePlayerInput playerInput)
    {
        if (gameState == GameState.LOBBY)
        {
            var spawnPoint = spawnPoints[playerId].position;
            playerCount++;
            playerId++;

            PlayerComponent player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity, entitiesOrigin).GetComponent<PlayerComponent>();

            player.input = playerInput;

            return playerId;
        }

        return -1;
    }

    public static void PlaySingle(AudioClip sound, float volume = 1.0f)
    {
        instance.audioSource.PlayOneShot(sound, volume);
    }
}