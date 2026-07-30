using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; 

public class DoorInteractable : MonoBehaviour, IPuzzlePanelOwner
{
    [Header("References")]
    public PuzzlePanelController puzzlePanelController;
    public ItemGiver itemGiver;

    [Header("Events")]
    public UnityEvent onPuzzleOpened;
    public UnityEvent onPuzzleClosed;

    private InputActions inputActions; 
    private bool isOpen = false;
    private bool canOpenPuzzle = false;
    private bool isPlayerInRange = false;

    private float lastInteractTime;
    private float interactCooldown = 0.2f;

    private void Awake()
    {
        
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
      
        inputActions.Interact.Enable();
        inputActions.Interact.Interact.performed += OnInteractPerformed;

        GameEvents.OnDialogueEvent += HandleDialogueEvent;
    }

    private void OnDisable()
    {
        
        inputActions.Interact.Interact.performed -= OnInteractPerformed;
        inputActions.Interact.Disable();

        GameEvents.OnDialogueEvent -= HandleDialogueEvent;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (!isPlayerInRange) return;

        
        if (Time.time < lastInteractTime + interactCooldown) return;
        lastInteractTime = Time.time;

        if (isOpen)
        {
            Debug.Log("Closing puzzle with E");
            ClosePuzzle();
        }
        else
        {
            Debug.Log("Trying to open puzzle with E");
            TryOpenPuzzle();
        }
    }

    private void TryOpenPuzzle()
    {
        
        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canInteract = gameplayState == GameplayState.Playing && uiState == UIState.None;

        if (!canInteract || !canOpenPuzzle || isOpen) return;

       
        if (itemGiver != null && itemGiver.giveOnlyOnce && itemGiver.AlreadyGiven)
            return;

        OpenPuzzle();
    }

    private void OpenPuzzle()
    {
        isOpen = true;

        if (puzzlePanelController != null)
            puzzlePanelController.Open(this);

        onPuzzleOpened?.Invoke();
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player in range of door. Press E to interact.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

 

    private void HandleDialogueEvent(DialogueAction action)
    {
        if (action is TriggerGameEventAction triggerAction)
        {
            if (triggerAction.eventType == DialogueGameEventType.UnlockPuzzle)
            {
                canOpenPuzzle = true;
            }
        }
    }

    public void ClosePuzzle()
    {
        if (puzzlePanelController != null)
            puzzlePanelController.RequestClose();
    }

    public void OnPuzzlePanelClosed()
    {
        isOpen = false;
        onPuzzleClosed?.Invoke();
    }
}