using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Light bulletLight;
    [SerializeField] ParticleSystem bulletSpark;
    [SerializeField] ParticleSystem bulletHole;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;

    [SerializeField] float speed;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bulletLight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(Vector3 dir, Vector3 pos)
    {
        bulletLight.enabled = true;
        transform.position = pos;
        rigid.linearVelocity = Vector3.zero;
        gameObject.SetActive(true);
        rigid.AddForce(dir * speed,ForceMode.VelocityChange);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.layer == LayerMask.NameToLayer("Terrain")))
            return;
        rigid.linearVelocity = Vector3.zero;
        bulletLight.enabled = false;

        StartCoroutine(BulletImpact());

    }

    IEnumerator BulletImpact()
    {
        int index = Random.Range(0, 4);

        audioSource.clip = clips[index];
        audioSource.Play();
        bulletHole.Play();
        bulletSpark.Play();

        yield return CoroutineCasher.Wait(3f);

        gameObject.SetActive(false);
    }
}
