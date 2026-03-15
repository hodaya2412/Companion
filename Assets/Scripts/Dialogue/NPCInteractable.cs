using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteractable : MonoBehaviour
{
    [Header("Dialogue (Conditional)")]
    public ConditionalDialogueAsset[] conditionalDialogues;

    [Header("Fallback Dialogue (Optional)")]
    public DialogueAsset dialogueAsset;

    [Header("Visual")]
    public GameObject visualArrow;

    [Header("Settings")]
    public float interactRange = 5f;

    private Transform player;
    private InputActions inputAction;

    private void Awake()
    {
        inputAction = new InputActions();
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        if (visualArrow != null)
            visualArrow.SetActive(false);
    }

    private void OnEnable()
    {
        inputAction.Interact.Enable();
        inputAction.Interact.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        inputAction.Interact.Interact.performed -= OnInteract;
        inputAction.Interact.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (player == null) return;

        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canInteract = gameplayState == GameplayState.Playing && uiState == UIState.None;
        if (!canInteract) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool isClose = dist <= interactRange;

        if (isClose)
        {
            StartDialogue();
        }
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool isClose = dist <= interactRange;

        if (visualArrow != null)
        {
            GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
            UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

            bool shouldShow = isClose &&
                              gameplayState == GameplayState.Playing &&
                              uiState == UIState.None;

            if (visualArrow.activeSelf != shouldShow)
                visualArrow.SetActive(shouldShow);
        }
    }

    private void StartDialogue()
    {
        if (conditionalDialogues != null && conditionalDialogues.Length > 0)
        {
            foreach (var cd in conditionalDialogues)
            {
                if (cd != null && cd.dialogue != null && cd.CanPlay())
                {
                    GameEvents.OnDialogueRequested?.Invoke(cd.dialogue);
                    Debug.Log("Starting conditional dialogue with: " + gameObject.name);
                    return;
                }
            }
        }

        if (dialogueAsset != null && dialogueAsset.lines != null && dialogueAsset.lines.Count > 0)
        {
            GameEvents.OnDialogueRequested?.Invoke(dialogueAsset);
            Debug.Log("Starting fallback dialogue with: " + gameObject.name);
        }
    }
}