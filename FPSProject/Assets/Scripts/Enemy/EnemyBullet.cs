using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] Rigidbody rigid;
    [SerializeField] ParticleSystem spark;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clip;

    [SerializeField] float speed;

    [SerializeField] int damage;
    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(Vector3 dir, Vector3 pos)
    {
        transform.position = pos;
        rigid.linearVelocity = Vector3.zero;
        mesh.enabled = true;

        gameObject.SetActive(true);
        rigid.AddForce(dir * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.layer == LayerMask.NameToLayer("Terrain")) || (other.gameObject.layer == LayerMask.NameToLayer("Enviroment")))
        {
            int index = Random.Range(0, clip.Length);
            audioSource.clip = clip[index];
            audioSource.Play();
            StartCoroutine(BulletTerrainImpact());
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<LocalPlayer>().Hit(damage);
            gameObject.SetActive(false);
        }
    }

    IEnumerator BulletTerrainImpact()
    {
        rigid.linearVelocity = Vector3.zero;
        mesh.enabled = false;

        yield return CoroutineCasher.Wait(1f);
    }

    
}
