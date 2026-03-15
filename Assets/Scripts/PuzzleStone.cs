using UnityEngine;

public class PuzzleStone : MonoBehaviour
{
    public string puzzleID = "ForestStones";
    public string solvedFlag = "Forest_PuzzleSolved";

    private void OnMouseDown()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.GetFlag(solvedFlag))
            return;

        GameplayState gameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
        UIState uiState = GameEvents.RequestCurrentUIState?.Invoke() ?? UIState.None;

        bool canOpenPuzzle =
            (gameplayState == GameplayState.Playing || gameplayState == GameplayState.Combat) &&
            uiState == UIState.None;

        if (canOpenPuzzle)
        {
            GameEvents.OnPuzzleStoneClicked?.Invoke(puzzleID);
        }
    }
}