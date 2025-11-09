using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public Transform head;
    [SerializeField] public Transform muzzle;
    [SerializeField] Magazine bossMag;
    [SerializeField] Image HPBar;
    [SerializeField] Text HP;
    [SerializeField] GameObject shield;
    [SerializeField] public Animator animator;
    [SerializeField] public BossSpawner minions;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] public AudioSource actAudio;
    [SerializeField] public AudioSource shotAudio;
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip shieldActive;
    Coroutine engageRoutine;

    public List<BossState> states;
    BossState currentState;

    [SerializeField] public int health;
    [SerializeField] int maxHealth;

    [SerializeField] public bool isDead;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        health = maxHealth;
        isDead = false;
        states = new List<BossState>();
        states.Add(new StandByState());
        states.Add(new DeactivateState());
        states.Add(new ActivateState());
        states.Add(new DestroyState());
        actAudio.spatialBlend = 1;
        ChangeState(states[0]);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            DestroyAllMinion();
        }
        currentState?.Update(this);
    }

    private void LateUpdate()
    {
        HPUpdate();
        currentState?.LateUpdate(this);
    }

    public void HeadRotate()
    {
        Vector3 dir = (target.position - head.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        targetRot *= Quaternion.Euler(0f, 90f, 0);
        head.rotation = targetRot;
    }

    void HPBarUpdate()
    {
        if(health <= 0)
        {
            health = 0;
        }
        HPBar.fillAmount = (float)health / (float)maxHealth;
        HP.text = health.ToString();
    }

    void HPUpdate()
    {
        if (isDead)
            return;

        if(health <=0)
        {
            health = 0;
            currentState = states[3];
        }
        HPBarUpdate();
    }

    void Shoot()
    {
        shotAudio.clip = shootClip;
        shotAudio.Play();
        Vector3 dir = (target.position - muzzle.position).normalized;
        bossMag.Fire(dir, muzzle);
    }

    public void Hit(int damage)
    {
        animator.SetTrigger("OnDamaged");
        health -= damage;
        StartCoroutine(DamagedColor());
    }

    public void ChangeState(BossState state)
    {
        currentState?.Exit(this);
        currentState = state;
        currentState.Start(this);
    }

    public void ShieldActive(bool value)
    {
        actAudio.clip = shieldActive;
        actAudio.Play();
        shield.SetActive(value);
    }

    public void Engage()
    {
        if(engageRoutine == null)
        {
            engageRoutine = StartCoroutine(Engaging());
        }
    }

    public void StopEngage()
    {
        if(engageRoutine !=null)
        {
            StopCoroutine(engageRoutine);
            engageRoutine = null;
        }
        
    }

    void DestroyAllMinion()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Minion[] minions = FindObjectsByType<Minion>(FindObjectsSortMode.None);

        foreach(var e in enemies)
        {
            if(e.gameObject.activeSelf)
            {
                e.Hit(9999, e.transform);
            }
        }

        foreach(var m in minions)
        {
            if(m.gameObject.activeSelf)
            {
                m.Hit(9999, m.transform);
            }
        }
    }

    public void Destroy()
    {
        DestroyAllMinion();
        StartCoroutine(OnDeath());
    }

    IEnumerator TripleBurst()
    {
        for(int i = 0; i< 3; i++)
        {
            Shoot();
            yield return CoroutineCasher.Wait(0.2f);
        }
    }

    public IEnumerator Engaging()
    {
        while(true)
        {
            StartCoroutine(TripleBurst());

            yield return CoroutineCasher.Wait(2f);
        }
    }

    IEnumerator DamagedColor()
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();


        foreach (var r in renderers)
        {
            r.material.EnableKeyword("_EMISSION");
            r.material.SetColor("_EmissionColor", Color.white * 2f);
        }

        yield return CoroutineCasher.Wait(0.1f);

        foreach (var r in renderers)
        {
            r.material.SetColor("_EmissionColor", Color.black);
        }
    }

    public IEnumerator OnDeath()
    {
        animator.SetTrigger("Destroy");
        ShieldActive(false);

        yield return CoroutineCasher.Wait(3f);
        
        Collider coll = GetComponentInChildren<Collider>();
        coll.enabled = false;
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers)
        {
            r.enabled = false;
        }
        deathParticle.gameObject.SetActive(true);
        deathParticle.gameObject.GetComponent<AudioSource>().Play();
        actAudio.clip = deathClip;
        actAudio.Play();

        yield return CoroutineCasher.Wait(2f);

        isDead = true;
        gameObject.SetActive(false);
    }
}
