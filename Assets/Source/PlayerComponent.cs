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


[RequireComponent(typeof(Rigidbody))]
public class PlayerComponent : MonoBehaviour, IItemHolder
{
    public Item itemPrefab;
    public float SPEED = 2.5f;
    public float THROW_POWER = 250;
    public float THROW_MAX_HOLD_IN_SECONDS = 2.0f;
    public float THROW_HOLD_THRESHOLD_IN_SECONDS = 0.33f;
    public float RAY_CAST_SIZE = 2.0f;
    public float TURN_TIME;
    public AnimationCurve ACCEL_CURVE;
    public AudioClip takeAudioClip;

    float accelerationTimer = 0;
    Rigidbody rigidbody;
    [HideInInspector] public IPlayerInput input;
    [HideInInspector] public Item holdedItem;
    int iteractableLayer;
    private Vector3 castRayDireaction = Vector3.left;
    private ChoopingBoard choopingBoard;

    Animator characterAnim;

    [SerializeField] Transform characterGFX;
    [SerializeField] Transform itemPos;
    private float turnTimer = 0;
    private bool right = true;
    Quaternion savedRot;

    Interactable lastInteractable;

    public Item HoldedItem => holdedItem;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        iteractableLayer = LayerMask.GetMask("Interactable");

        characterAnim = GetComponent<Animator>();
    }

    void Update()
    {
        var gotInteractable = TryGetNearestInteractable(out var interactable);

        if (gotInteractable)
        {
            if (lastInteractable != null && lastInteractable != interactable)
            {
                lastInteractable.DeHighlight();
            }

            interactable.Highlight();
            lastInteractable = interactable;
        }
        else
        {
            if (lastInteractable != null)
            {
                lastInteractable.DeHighlight();
            }

            lastInteractable = null;
        }

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

        characterAnim.SetTrigger("Throw");
        holdedItem.transform.parent = transform.root;
        //characterAnim.SetBool("Holding", false);
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
        //holdedItem.transform.position = transform.position + castRayDireaction;
        holdedItem.transform.position = itemPos.position;
        holdedItem.transform.rotation = characterGFX.rotation;
    }

    private void FixedUpdate()
    {
        var moveDir = input.GetMovementDirection();

        turnTimer += Time.fixedDeltaTime;
        if (moveDir.x > 0)
        {
            //Vector3(329.947937, 18.3123474, 350.589386)
            if (!right)
            {
                turnTimer = 0;
                right = true;
                savedRot = characterGFX.rotation;
            }
        }
        else if (moveDir.x < 0)
        {
            if (right)
            {
                turnTimer = 0;
                right = false;
                savedRot = characterGFX.rotation;
            }
        }

        if (right)
        {
            characterGFX.rotation = Quaternion.Lerp(savedRot, Quaternion.Euler(0, 72, 0), turnTimer / TURN_TIME);
        }
        else
        {
            characterGFX.rotation = Quaternion.Lerp(savedRot, Quaternion.Euler(0, 252, 0), turnTimer / TURN_TIME);
        }

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
        GameManager.PlaySingle(takeAudioClip);
        holdedItem.transform.parent = transform;
        UpdateHoldedItemPosition();
        characterAnim.SetBool("Holding", true);
    }

    public Item ReleaseItem()
    {
        GameManager.PlaySingle(takeAudioClip);
        var temp = holdedItem;
        holdedItem = null;
        characterAnim.SetBool("Holding", false);
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

        characterAnim.SetBool("Chopping", true);
    }

    public void UnlookChooping()
    {
        choopingBoard = null;

        characterAnim.SetBool("Chopping", false);
    }
}