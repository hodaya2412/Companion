using UnityEngine;

public class ForestSceneController : MonoBehaviour
{
    [Header("Scene refs")]
    [SerializeField] private GameObject caveBlocker;
    [SerializeField] private GameObject banditGroup;
    [SerializeField] private GameObject bookObject;

    private bool puzzleSolved;
    private bool bookAcquired;

    private void Start()
    {
        ApplySceneState();
    }

    private void ApplySceneState()
    {
        if (caveBlocker) caveBlocker.SetActive(puzzleSolved);
        if (bookObject) bookObject.SetActive(!bookAcquired);
        // banditGroup נשאר כרגע פעיל - בהמשך נשנה אם תרצי
    }

    public bool IsPuzzleSolved() => puzzleSolved;

    public void MarkPuzzleSolved()
    {
        puzzleSolved = true;
        ApplySceneState();
        Debug.Log("Puzzle solved -> Cave blocked!");
    }

    public void CollectBook()
    {
        if (bookAcquired) return;
        bookAcquired = true;
        ApplySceneState();
        Debug.Log("Book acquired!");
    }

    public void StartCombat()
    {
        Debug.Log("Combat starts! (Placeholder)");
        // כרגע נעשה כאילו השחקן ניצח ישר (לבדיקה)
        // בהמשך נחליף למערכת קרב אמיתית
        CombatWin();
    }

    public void CombatWin()
    {
        Debug.Log("Combat win!");
        CollectBook();
    }

    public void CombatLose()
    {
        Debug.Log("Combat lose! Book is lost forever in this run.");
        // בינתיים פשוט נכבה את הספר כדי שלא יהיה אפשר להשיג
        if (bookObject) bookObject.SetActive(false);
    }
}