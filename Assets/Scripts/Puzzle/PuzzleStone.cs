using UnityEngine;
using UnityEngine.InputSystem; // חובה להוסיף

public class PuzzleStone : MonoBehaviour
{
    public string puzzleID = "ForestStones";
    public string solvedFlag = "Forest_PuzzleSolved";

    [Header("Block After Victory")]
    [Tooltip("שם הדגל שסימנו ב-GameStateManager כשהשודדים מתו")]
    public string banditsDefeatedFlag = "Forest_BanditsDefeated";

    private InputActions inputActions;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        
        inputActions.Interact.Enable();
        inputActions.Interact.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        
        inputActions.Interact.Interact.performed -= OnInteractPerformed;
        inputActions.Interact.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
     
        if (!isPlayerInRange) return;

        TryOpenStonePuzzle();
    }

    private void TryOpenStonePuzzle()
    {
        if (GameStateManager.Instance == null) return;

       
        if (GameStateManager.Instance.GetFlag(solvedFlag))
            return;

        
        if (GameStateManager.Instance.GetFlag(banditsDefeatedFlag))
        {
            Debug.Log("השודדים מתו, האבן כבר לא מגיבה.");
            return;
        }

        
        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canOpenPuzzle =
            (gameplayState == GameplayState.Playing || gameplayState == GameplayState.Combat) &&
            uiState == UIState.None;

        if (canOpenPuzzle)
        {
            Debug.Log($"Opening puzzle: {puzzleID} via E key");
            GameEvents.OnPuzzleStoneClicked?.Invoke(puzzleID);
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}