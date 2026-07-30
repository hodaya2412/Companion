using UnityEngine;
using TMPro;

public class PopupHintZone : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popupObject;
    [SerializeField] private TMP_Text popupText;

    [Header("Hint Text")]
    [TextArea(2, 4)]
    [SerializeField] private string hintText;

    [Header("Hide When Flag Is True")]
    [SerializeField] private string hideFlag;
    [SerializeField] private bool showOnlyOnce = true;
    [SerializeField] private string seenFlag = "Seen_Hint";
    private void Start()
    {
        if (popupObject != null)
            popupObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (showOnlyOnce && HasBeenSeen()) return;
        if (ShouldHide()) return;

        if (GameStateManager.Instance != null && !string.IsNullOrEmpty(seenFlag))
        {
            GameStateManager.Instance.SetFlag(seenFlag, true);
        }

        if (popupText != null)
            popupText.text = hintText;

        if (popupObject != null)
            popupObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (popupObject != null)
            popupObject.SetActive(false);
    }
    private bool HasBeenSeen()
    {
        if (GameStateManager.Instance == null) return false;
        return GameStateManager.Instance.GetFlag(seenFlag);
    }
    private bool ShouldHide()
    {
        if (GameStateManager.Instance == null) return false;
        if (string.IsNullOrEmpty(hideFlag)) return false;

        return GameStateManager.Instance.GetFlag(hideFlag);
    }
}