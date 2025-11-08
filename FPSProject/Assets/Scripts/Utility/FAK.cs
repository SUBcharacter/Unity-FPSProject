using System.Collections;
using UnityEngine;

public class FAK : MonoBehaviour
{
    [SerializeField] int healAmount;
    [SerializeField] MeshRenderer ren;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;

    [SerializeField] float time;
    [SerializeField] float lifeTime;

    private void Awake()
    {
        time = 0;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.Rotate(0, 360 * Time.deltaTime, 0);

        time += Time.deltaTime;

        if(time >= lifeTime)
        {
            time = 0;
            gameObject.SetActive(false);
        }
    }

    public void Init(Vector3 pos)
    {
        transform.position = pos;
        ren.enabled = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        other.GetComponent<LocalPlayer>().Heal(healAmount);
        StartCoroutine(Get());
    }

    IEnumerator Get()
    {
        ren.enabled = false;
        audioSource.clip = clip;
        audioSource.Play();
        yield return CoroutineCasher.Wait(1f);
        gameObject.SetActive(false);
    }
}
