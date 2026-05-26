using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(musicClip);
    }
}