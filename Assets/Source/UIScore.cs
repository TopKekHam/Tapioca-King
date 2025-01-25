using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    public TMP_Text label;

    void Update()
    {
        var gameManager = GameManager.instance;

        if (gameManager.gameState == GameState.PLAYING)
        {
            label.text = $"{gameManager.score}";
        }
        else
        {
            label.text = "";
        }
    }
}