using UnityEngine;
using UnityEngine.InputSystem;

public class ProximityInteract : MonoBehaviour
{
    [Header("Identity")]
    public string worldItemId;
    public string requiredFlag = "Forest_PuzzleSolved";

    [Header("Settings")]
    public float radius = 4f;

    private Transform player;
    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void OnEnable()
    {
        inputActions.Inventory.Enable();
        inputActions.Inventory.PickUp.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        inputActions.Inventory.PickUp.performed -= OnInteractPerformed;
        inputActions.Inventory.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        Debug.Log($"[Direct Input] Pressed E. Distance to {worldItemId}: {distance}");

        if (distance <= radius)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        bool noFlagRequired = string.IsNullOrEmpty(requiredFlag);
        bool isSolved = noFlagRequired ||
                        (GameStateManager.Instance != null && GameStateManager.Instance.GetFlag(requiredFlag));

        if (isSolved)
        {
            Debug.Log("<color=green>[Success]</color> Picking up: " + worldItemId);
            var item = WorldItemRegistry.Instance.Get(worldItemId);
            if (item != null) item.Pickup();
        }
        else
        {
            Debug.Log("<color=red>[ALERT]</color> Puzzle NOT solved! Combat Triggered.");

            if (GameStateManager.Instance != null &&
                GameStateManager.Instance.CurrentGameplayState != GameplayState.Combat)
            {
                GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Combat);
                GameEvents.OnCombatTriggered?.Invoke();
            }
        }
    }
}