using UnityEngine;

public class PuzzleStone : MonoBehaviour
{
  
        public string puzzleID = "ForestStones";
        public string solvedFlag = "Forest_PuzzleSolved"; // הדגל שבודק אם כבר פתרנו

        private void OnMouseDown()
        {
            // 1. בדיקה אם כבר פתרנו - אם כן, החידה לא נפתחת שוב
            if (GameStateManager.Instance.GetFlag(solvedFlag)) return;

            // 2. פתיחת החידה דרך האירוע
            if (GameStateManager.Instance.CurrentState == GameState.Playing)
            {
                GameEvents.OnPuzzleStoneClicked?.Invoke(puzzleID);
            }
        }
}