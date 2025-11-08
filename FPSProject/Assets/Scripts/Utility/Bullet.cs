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
    [SerializeField] Transform fireOrigin;

    [SerializeField] float speed;

    [SerializeField] int damage;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bulletLight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(Vector3 dir, Transform pos)
    {
        bulletLight.enabled = true;
        transform.position = pos.position;
        transform.rotation = pos.rotation;
        fireOrigin = pos;
        rigid.linearVelocity = Vector3.zero;
        MeshRenderer ren = GetComponent<MeshRenderer>();
        ren.enabled = true;
        gameObject.SetActive(true);
        rigid.AddForce(dir * speed,ForceMode.VelocityChange);

    }

    void Stop()
    {
        rigid.linearVelocity = Vector3.zero;
        bulletLight.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer == LayerMask.NameToLayer("Terrain")) || (other.gameObject.layer == LayerMask.NameToLayer("Enviroment")))
        {
            Stop();
            StartCoroutine(BulletTerrainImpact());
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Stop();
            other.GetComponent<Enemy>().Hit(damage,fireOrigin);
            StartCoroutine(BulletEnemyImpact());
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Stop();
            other.GetComponent<LocalPlayer>().Hit(damage);
            gameObject.SetActive(false);
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Border"))
        {
            gameObject.SetActive(false);
        }
        

    }

    IEnumerator BulletTerrainImpact()
    {
        int index = Random.Range(0, 4);

        MeshRenderer ren = GetComponent<MeshRenderer>();
        ren.enabled = false;

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
