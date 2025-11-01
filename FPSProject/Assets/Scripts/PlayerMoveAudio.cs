using UnityEngine;

public class PlayerMoveAudio : MonoBehaviour
{
    public static AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
