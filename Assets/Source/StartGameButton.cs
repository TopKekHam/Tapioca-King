using System.Collections;
using UnityEngine;

public class StartGameButton : Interactable
{
    public Transform ButtonTransform;
    public AudioClip audioClick;

    public IEnumerable AnimatePress()
    {
        ButtonTransform.transform.localPosition += new Vector3(0, -0.5f, 0);
        
        return null;
    }

    public IEnumerable AnimateRelease()
    {
        ButtonTransform.transform.localPosition += new Vector3(0, 0, 0);
        
        return null;
    }

    public override void DeHighlight()
    {
       
    }

    public override void Highlight()
    {
     
    }

    public override void Interact(PlayerComponent player)
    {
        if (GameManager.instance.gameState == GameState.LOBBY)
        {
            GameManager.instance.BeginGame();
            GameManager.PlaySingle(audioClick);
        }
    }
}