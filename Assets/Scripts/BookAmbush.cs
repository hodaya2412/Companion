using UnityEngine;

public class BookAmbush : MonoBehaviour
{
    [SerializeField] private ForestSceneController scene;
    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;

        if (scene.IsPuzzleSolved())
        {
            Debug.Log("Safe pickup!");
            scene.CollectBook();
        }
        else
        {
            Debug.Log("Ambush!");
            scene.StartCombat();
        }
    }
}