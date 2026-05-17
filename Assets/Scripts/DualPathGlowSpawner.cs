using UnityEngine;

public class DualPathGlowSpawner : MonoBehaviour
{
    [SerializeField] private Transform leftSide;
    [SerializeField] private GameObject glowParticlePrefab;
    [SerializeField] private float spacing = 1f;

    private void Start()
    {
        SpawnSide(leftSide);
    }

    private void SpawnSide(Transform side)
    {
        if (side == null || glowParticlePrefab == null) return;

        for (int i = 0; i < side.childCount - 1; i++)
        {
            Vector3 start = side.GetChild(i).position;
            Vector3 end = side.GetChild(i + 1).position;

            float distance = Vector3.Distance(start, end);
            int count = Mathf.Max(1, Mathf.FloorToInt(distance / spacing));

            for (int j = 0; j <= count; j++)
            {
                float t = j / (float)count;
                Vector3 position = Vector3.Lerp(start, end, t);

                Instantiate(glowParticlePrefab, position, Quaternion.identity, side);
            }
        }
    }
}