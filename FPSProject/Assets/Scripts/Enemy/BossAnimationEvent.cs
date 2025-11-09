using UnityEngine;

public class BossAnimationEvent : MonoBehaviour
{
    [SerializeField] ParticleSystem[] sparks;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Destroy()
    {
        int index = Random.Range(0, sparks.Length);
        audioSource.clip = clip;
        audioSource.Play();
        sparks[index].Play();
    }
}
