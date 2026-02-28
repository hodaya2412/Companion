using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteractable : MonoBehaviour
{
    [Header("Dialogue")]
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

        // הצגת/הסתרת חץ רק אם קרובים ובמצב משחק
        if (visualArrow != null)
        {
            bool shouldShow = isClose &&
                              GameStateManager.Instance.CurrentState == GameState.Playing;

            if (visualArrow.activeSelf != shouldShow)
                visualArrow.SetActive(shouldShow);
        }

        // בדיקה בטוחה ללחיצה על D
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
        if (dialogueAsset == null) return;
        if (dialogueAsset.lines == null || dialogueAsset.lines.Count == 0) return;

        Debug.Log("Starting dialogue with: " + gameObject.name);
        GameEvents.OnDialogueRequested?.Invoke(dialogueAsset);
    }
}