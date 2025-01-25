using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class Toggle
{
    public bool state;
    public bool pressed, released;
    public float duration;
    public float timer;

    public Toggle()
    {
        state = false;
        pressed = false;
        released = false;
        duration = 0f;
        timer = 0;
    }

    public void Action()
    {
        state = !state;

        if (state)
        {
            timer = Time.time;
            pressed = true;
        }
        else
        {
            released = true;
            duration = Time.time - timer;
        }
    }

    public void LateUpdate()
    {
        released = false;
        pressed = false;
    }
}

public class GamePlayerInput : MonoBehaviour, IPlayerInput
{
    public int playerId = 0;
    private Toggle action1 = new Toggle(), action2 = new Toggle();
    private Vector2 movement;

    public void Start()
    {
        playerId = GameManager.instance.OnPlayerJoin(this);
    }

    public void LateUpdate()
    {
        action1.LateUpdate();
        action2.LateUpdate();
    }
    
    public void MoveInput(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }

    public void Action1Input(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            action1.Action();
        }
    }

    public void Action2Input(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            action2.Action();
        }
    }

    public Vector3 GetMovementDirection()
    {
        return new Vector3(movement.x, movement.y, 0);
    }

    public bool IsInteracting()
    {
        return action1.pressed;
    }

    public bool IsAction2(out float timer)
    {
        timer = action2.duration;
        return action2.released;
    }
}