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

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        if (visualArrow != null)
            visualArrow.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool isClose = dist <= interactRange;

        // הצגת/הסתרת חץ רק אם קרובים ובמצב משחק Playing
        if (visualArrow != null)
        {
            bool shouldShow = isClose &&
                              GameStateManager.Instance.CurrentState == GameState.Playing;

            if (visualArrow.activeSelf != shouldShow)
                visualArrow.SetActive(shouldShow);
        }

        // בדיקה ללחיצה על D
        if (isClose &&
            Keyboard.current != null &&
            Keyboard.current.dKey.wasPressedThisFrame &&
            GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        // 🟢 בדיקה לדיאלוג מותנה ראשון שמתאים
        if (conditionalDialogues != null && conditionalDialogues.Length > 0)
        {
            foreach (var cd in conditionalDialogues)
            {
                if (cd.CanPlay())
                {
                    DialogueManager.Instance.StartDialogue(cd.dialogue);
                    Debug.Log("Starting conditional dialogue with: " + gameObject.name);
                    return; // מפעילים את הדיאלוג הראשון שמתאים
                }
            }
        }

        // 🟢 fallback לדיאלוג רגיל אם Conditional לא מתאים או לא מוגדר
        if (dialogueAsset != null && dialogueAsset.lines != null && dialogueAsset.lines.Count > 0)
        {
            DialogueManager.Instance.StartDialogue(dialogueAsset);
            Debug.Log("Starting fallback dialogue with: " + gameObject.name);
        }
    }
}