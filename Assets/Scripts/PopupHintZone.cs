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

    private void Start()
    {
        if (popupObject != null)
            popupObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (ShouldHide()) return;

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

    private bool ShouldHide()
    {
        if (GameStateManager.Instance == null) return false;
        if (string.IsNullOrEmpty(hideFlag)) return false;

        return GameStateManager.Instance.GetFlag(hideFlag);
    }
}