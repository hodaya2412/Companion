using UnityEngine;

public class MagicBarrier : MonoBehaviour
{
    [SerializeField] private GameObject barrierVisual;
    [SerializeField] private GameObject secondBarrierVisual; 
    [SerializeField] private Collider barrierCollider;
    [SerializeField] private AudioSource unlockSound; 
    private void OnEnable()
    {
        GameEvents.OnPuzzleOrCombatSolved += RemoveBarrier;
    }

    private void OnDisable()
    {
        GameEvents.OnPuzzleOrCombatSolved -= RemoveBarrier;
    }

    private void RemoveBarrier()
    {
        
        if (barrierVisual != null) barrierVisual.SetActive(false);
        if (secondBarrierVisual != null) secondBarrierVisual.SetActive(false);

       
        if (barrierCollider != null) barrierCollider.enabled = false;

       
        if (unlockSound != null) unlockSound.Play();

        Debug.Log("ניצחון זוהה - המחסומים הוסרו והופעל סאונד!");
    }
}