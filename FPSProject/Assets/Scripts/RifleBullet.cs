using UnityEngine;

public class RifleBullet : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] ParticleSystem bulletHole;
    [SerializeField] ParticleSystem bulletSpark;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Init()
    {

    }

}
