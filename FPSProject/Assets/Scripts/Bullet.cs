using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Light bulletLight;
    [SerializeField] ParticleSystem bulletHole;
    [SerializeField] ParticleSystem bulletSpark;

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

        bulletHole.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        bulletSpark.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameObject.SetActive(true);

        rigid.linearVelocity = dir * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(other.gameObject.layer == LayerMask.NameToLayer("Terrain")))
            return;
        bulletLight.enabled = false;
        rigid.linearVelocity = Vector3.zero;

        StartCoroutine(TerrainImpact());
    }

    IEnumerator TerrainImpact()
    {
        bulletHole.Play();
        bulletSpark.Play();

        

        yield return CoroutineCasher.Wait(3f);

        gameObject.SetActive(false);
    }

}
