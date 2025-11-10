using UnityEngine;

public class Rotating : MonoBehaviour
{
    [SerializeField] AudioSource BGMAudio;
    [SerializeField] AudioClip BGMClip;

    private void Awake()
    {
        Time.timeScale = 1f;
        BGMAudio.clip = BGMClip;
        BGMAudio.loop = true;
        BGMAudio.Play();
    }

    void Update()
    {
        transform.Rotate(0, 10 * Time.deltaTime, 0);
    }
}
