using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] ParticleSystem spark;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip clip;
    [SerializeField] float speed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(Vector3 dir, Vector3 pos)
    {
        transform.position = pos;
        rigid.linearVelocity = Vector3.zero;

        gameObject.SetActive(true);
        rigid.AddForce(dir * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
