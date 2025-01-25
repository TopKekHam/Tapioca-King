using TMPro;
using UnityEngine;

public class UIGameTimer : MonoBehaviour
{
    public TMP_Text timeLabel;
    
    void Update()
    {
        if (GameManager.instance.gameState == GameState.PLAYING)
        {
            var timer = GameManager.instance.gameTimer;

            int minutes = Mathf.FloorToInt(timer / 60.0f);
            int seconds = Mathf.FloorToInt(timer % 60);

            timeLabel.text = $"{minutes:D2}:{seconds:D2}";
        }
        else
        {
            timeLabel.text = "";
        }
    }
}