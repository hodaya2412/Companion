using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    private static bool created;

    private void Awake()
    {
        if (created) { Destroy(gameObject); return; }
        created = true;
        DontDestroyOnLoad(gameObject);
    }
}