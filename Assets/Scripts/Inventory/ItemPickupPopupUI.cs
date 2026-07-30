using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class ItemPickupPopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private float popupLifetime = 4f;

    private void OnEnable()
    {
        GameEvents.OnItemAdded += ShowPopup;
    }

    private void OnDisable()
    {
        GameEvents.OnItemAdded -= ShowPopup;
    }

    private void ShowPopup(InventoryItemData item, int amount)
    {
       
        GameObject popup = Instantiate(popupPrefab, container);

       
        TMP_Text text = popup.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = $"+{amount} {item.displayName}";
        }

        
        Transform iconTransform = popup.transform.Find("ItemIcon");
        if (iconTransform != null)
        {
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null && item.icon != null)
            {
                iconImage.sprite = item.icon;
            }
        }

       
        StartCoroutine(DestroyAfterTime(popup, popupLifetime));
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }
}