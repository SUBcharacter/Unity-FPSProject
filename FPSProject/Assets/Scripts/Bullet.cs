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
        if ((other.gameObject.layer == LayerMask.NameToLayer("Terrain")) || (other.gameObject.layer == LayerMask.NameToLayer("Enviroment")))
        {
            rigid.linearVelocity = Vector3.zero;
            bulletLight.enabled = false;

            StartCoroutine(BulletTerrainImpact());
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            rigid.linearVelocity = Vector3.zero;
            bulletLight.enabled = false;

            StartCoroutine(BulletEnemyImpact());
        }
        

    }

    IEnumerator BulletTerrainImpact()
    {
        int index = Random.Range(0, 4);

        audioSource.clip = clips[index];
        audioSource.Play();
        bulletHole.Play();
        bulletSpark.Play();

        yield return CoroutineCasher.Wait(3f);

        gameObject.SetActive(false);
    }
    
    IEnumerator BulletEnemyImpact()
    {
        int index = Random.Range(0, 4);
        audioSource.clip = clips[index];
        audioSource.Play();

        bulletSpark.Play();

        yield return CoroutineCasher.Wait(1f);

        gameObject.SetActive(false);
    }
}
