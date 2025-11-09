using System.Collections;
using System.Text;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Collider coll;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Light bulletLight;
    [SerializeField] ParticleSystem bulletSpark;
    [SerializeField] ParticleSystem bulletHole;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;
    [SerializeField] Transform fireOrigin;
    [SerializeField] LayerMask originMask;

    [SerializeField] float speed;

    [SerializeField] int damage;

    const int terrain = 3;
    const int enviroment = 14;
    const int enemy = 16;
    const int boss = 19;
    const int minion = 21;
    const int border = 17;
    const int player = 15;
    const int shield = 20;
    const int boss_Weakness = 22;

    private void Awake()
    {
        coll = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
        bulletLight = GetComponent<Light>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(Vector3 dir, Transform pos)
    {
        coll.enabled = true;
        bulletLight.enabled = true;
        transform.position = pos.position;
        transform.rotation = Quaternion.LookRotation(dir);
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
        coll.enabled = false;
        bulletLight.enabled = false;
    }

    void Triggered(Collider other)
    {
        if (((1 << other.gameObject.layer) & originMask) != 0)
            return;

        switch (other.gameObject.layer)
        {
            case terrain :
            case enviroment:
                Stop();
                StartCoroutine(BulletTerrainImpact());
                break;
            case enemy:
                Stop();
                other.GetComponent<Enemy>().Hit(damage, fireOrigin);
                StartCoroutine(BulletEnemyImpact());
                break;
            case minion:
                Stop();
                other.GetComponent<Minion>().Hit(damage, fireOrigin);
                StartCoroutine(BulletEnemyImpact());
                break;
            case boss:
                Stop();
                other.GetComponentInParent<Boss>().Hit(damage);
                StartCoroutine(BulletEnemyImpact());
                break;
            case boss_Weakness:
                Stop();
                other.GetComponentInParent<Boss>().Hit(damage * 2);
                break;
            case border:
                gameObject.SetActive(false);
                break;
            case shield:
                Stop();
                StartCoroutine(BulletEnemyImpact());
                break;
            case player:
                Stop();
                other.GetComponent<LocalPlayer>().Hit(damage);
                gameObject.SetActive(false);
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Triggered(other);
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
        MeshRenderer ren = GetComponent<MeshRenderer>();
        ren.enabled = false;

        int index = Random.Range(0, 4);
        audioSource.clip = clips[index];
        audioSource.Play();

        bulletSpark.Play();

        yield return CoroutineCasher.Wait(1f);

        gameObject.SetActive(false);
    }

    
}
