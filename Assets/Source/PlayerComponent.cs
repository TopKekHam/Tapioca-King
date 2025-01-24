using System;
using System.Numerics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public interface IPlayerInput
{
    Vector3 GetMovementDirection();
    bool IsInteracting();
    bool IsAction2(out float timer);
}

public class PlayerKeyboardInput : IPlayerInput
{
    private float throwTimeStart;

    public Vector3 GetMovementDirection()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            direction.y += 1;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            direction.y -= 1;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction.x -= 1;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction.x += 1;
        }

        return direction;
    }

    public bool IsInteracting()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public bool IsAction2(out float timer)
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            throwTimeStart = Time.fixedTime;
        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            timer = Time.fixedTime - throwTimeStart;
            return true;
        }

        timer = 0;
        return false;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerComponent : MonoBehaviour, IItemHolder
{
    public Item itemPrefab;
    public float SPEED = 2.5f;
    public float THROW_POWER = 250;
    public float THROW_MAX_HOLD_IN_SECONDS = 2.0f;
    public float THROW_HOLD_THRESHOLD_IN_SECONDS = 0.33f;
    public float RAY_CAST_SIZE = 2.0f;
    public AnimationCurve ACCEL_CURVE;

    float accelerationTimer = 0;
    Rigidbody rigidbody;
    IPlayerInput input;
    [HideInInspector] public Item holdedItem;
    int iteractableLayer;
    private Vector3 castRayDireaction = Vector3.left;
    private ChoopingBoard choopingBoard;

    Animator characterAnim;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        input = new PlayerKeyboardInput();
        iteractableLayer = LayerMask.GetMask("Interactable");

        characterAnim = GetComponent<Animator>();
    }

    void Update()
    {
        var gotInteractable = TryGetNearestInteractable(out var interactable);

        if (input.IsInteracting())
        {
            if (IsChooping())
            {
                choopingBoard.Choop();
            }
            else if (gotInteractable)
            {
                Debug.Log($"interacting with: {interactable.gameObject.name}");
                interactable.Interact(this);
            }
        }
        else if (input.IsAction2(out var timer))
        {
            if (IsChooping())
            {
                choopingBoard.ReleaseFromChooping(this);
            }
            else if (IsHoldingItem())
            {
                if (timer > THROW_HOLD_THRESHOLD_IN_SECONDS)
                {
                    ThrowItem(Mathf.Min(timer, THROW_MAX_HOLD_IN_SECONDS) / THROW_MAX_HOLD_IN_SECONDS);
                }

                Utils.DisholdItem(this, holdedItem);
            }
        }

        if (holdedItem != null)
        {
            UpdateHoldedItemPosition();
        }

    }

    public bool IsChooping()
    {
        return choopingBoard != null;
    }


    void ThrowItem(float powerNormal)
    {
        float power = 4 * powerNormal;
        Vector3 powerVector = castRayDireaction.normalized;
        holdedItem.rigidbody.AddForce(THROW_POWER * powerNormal * powerVector);
    }

    bool TryGetNearestInteractable(out Interactable interactable)
    {
        interactable = null;

        {
            if (Physics.Raycast(transform.position, castRayDireaction, out RaycastHit hit, RAY_CAST_SIZE,
                    iteractableLayer))
            {
                if (hit.collider.gameObject.TryGetComponent<Interactable>(out interactable))
                {
                    return true;
                }
            }
        }

        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, RAY_CAST_SIZE,
                    iteractableLayer))
            {
                if (hit.collider.gameObject.TryGetComponent<Interactable>(out interactable))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void UpdateHoldedItemPosition()
    {
        holdedItem.transform.position = transform.position + castRayDireaction;
    }

    private void FixedUpdate()
    {
        var moveDir = input.GetMovementDirection();

        if (moveDir != Vector3.zero && IsChooping() == false)
        {
            castRayDireaction = moveDir.normalized;

            accelerationTimer += Time.fixedDeltaTime;
            accelerationTimer = Math.Min(accelerationTimer, ACCEL_CURVE.keys[^1].value);

            rigidbody.linearVelocity = moveDir * SPEED * ACCEL_CURVE.Evaluate(accelerationTimer);

            characterAnim.SetBool("Flying", true);
        }
        else
        {
            accelerationTimer -= Time.fixedDeltaTime;
            accelerationTimer = Math.Max(accelerationTimer, 0);

            characterAnim.SetBool("Flying", false);
        }
    }

    public bool IsHoldingItem()
    {
        return holdedItem != null;
    }

    public void HoldItem(Item item)
    {
        holdedItem = item;
        UpdateHoldedItemPosition();
    }

    public Item ReleaseItem()
    {
        var temp = holdedItem;
        holdedItem = null;
        return temp;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        Gizmos.DrawLine(transform.position, transform.position + (castRayDireaction * RAY_CAST_SIZE));
    }

    public void LockChooping(ChoopingBoard board)
    {
        choopingBoard = board;
        rigidbody.linearVelocity = Vector3.zero;
    }

    public void UnlookChooping()
    {
        choopingBoard = null;
    }
}