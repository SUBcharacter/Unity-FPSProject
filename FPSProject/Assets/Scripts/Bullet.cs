using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Light bulletLight;
    [SerializeField] ParticleSystem bulletSpark;
    [SerializeField] ParticleSystem bulletHole;

    [SerializeField] float speed;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        bulletLight = GetComponent<Light>();


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
        bulletHole.Play();
        bulletSpark.Play();

        yield return CoroutineCasher.Wait(3f);

        gameObject.SetActive(false);
    }
}
