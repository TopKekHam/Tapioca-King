using UnityEngine;

public interface Highlightable
{
    void Highlight();
    void DeHighlight();
}

public abstract class Interactable : MonoBehaviour, Highlightable
{
    public abstract void DeHighlight();
    public abstract void Highlight();

    public abstract void Interact(PlayerComponent player);
}
