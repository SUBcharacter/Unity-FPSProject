using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Minion : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public Transform target;
    public List<MinionState> states;
    [SerializeField] Transform muzzle;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Detector detector;
    [SerializeField] Image HPBar;
    [SerializeField] Transform patrolTarget;
    [SerializeField] Transform[] patrolPoint;
    [SerializeField] Magazine magazine;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] Medikit medikit;
    [SerializeField] BossSpawner spawner;
    [SerializeField] public EnemyMoveAudio moveAudio;
    [SerializeField] public EnemyActAudio actAudio;
    [SerializeField] public EnemyShotAudio shotAudio;
    [SerializeField] MinionState currentState;
    Coroutine slow;

    #region AudioClips

    [SerializeField] public AudioClip moveClip;
    [SerializeField] public AudioClip alertClip;
    [SerializeField] public AudioClip attackClip;
    [SerializeField] public AudioClip deahtClip;

    #endregion


    [SerializeField] public float attackDistance;
    [SerializeField] float speed;

    [SerializeField] int health;
    [SerializeField] int maxHealth;

    [SerializeField] public bool targetAcquired = false;
    [SerializeField] bool isDead = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        detector = GetComponentInChildren<Detector>();
        agent = GetComponent<NavMeshAgent>();
        medikit = FindAnyObjectByType<Medikit>();
        spawner = GetComponentInParent<BossSpawner>();
        health = maxHealth;
        speed = agent.speed;
        states = new List<MinionState>();
        states.Add(new WaitState());
        states.Add(new SearchState());
        states.Add(new PersuingState());
        states.Add(new EngagingState());
        ChangeState(states[0]);
    }

    private void Start()
    {
        moveAudio.audioSource.clip = moveClip;
        moveAudio.audioSource.loop = true;
        moveAudio.audioSource.Play();
    }
    private void Update()
    {
        if(!isDead)
        currentState.Update(this);
    }

    private void LateUpdate()
    {
        if(health <= 0)
        {
            HPBar.fillAmount = 0;
            isDead = true;
        }
        
        HPBar.fillAmount = (float)health / (float)maxHealth;
    }

    public void ChangeState(MinionState state)
    {
        currentState?.Exit(this);
        currentState = state;
        currentState.Start(this);
    }

    public void Init(Vector3 pos, Transform[] points, Magazine mag)
    {
        StopAllCoroutines();
        health = maxHealth;
        isDead = false;
        patrolPoint = points;
        magazine = mag;
        Collider coll = GetComponent<Collider>();
        coll.enabled = true;
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var r in renderers)
        {
            r.enabled = true;
        }
        gameObject.SetActive(true);
        moveAudio.audioSource.clip = moveClip;
        moveAudio.audioSource.Play();
        agent.enabled = true;
        agent.isStopped = false;
        agent.ResetPath();
        agent.Warp(pos);
        agent.speed = speed;
        ChangeState(states[0]);
        StartCoroutine(DetectEnemy());
    }

    public void Hit(int damage, Transform attacker)
    {
        animator.Play("Hurt");
        health -= damage;

        StartCoroutine(DamagedColor());
        if (slow != null)
            StopCoroutine(slow);

        slow = StartCoroutine(DamagedSlow());

        if(!targetAcquired && attacker != null)
        {
            target = attacker;
            targetAcquired = true;
            ChangeState(states[2]);
        }
    }

    public void Shoot()
    {
        animator.SetTrigger("Attack");
        shotAudio.PlaySound(attackClip);
        Vector3 dir = target.position - muzzle.position;
        magazine.Fire(dir, muzzle);
    }

    public bool ArrivedPoint()
    {
        return !agent.pathPending && agent.remainingDistance <= 1f;
    }

    public void StartPatrol()
    {
        patrolTarget = GetRandomPatrolPoint();

        if (patrolTarget != null)
        {
            agent.SetDestination(patrolTarget.position);
        }
        else
            return;
    }

    void GetMedikit()
    {
        float randomValue = Random.Range(0f, 1f);
        if(randomValue <= 0.7f)
        {
            medikit.Get(transform.position);
        }
        else
        {
            return;
        }
        
    }
    public void MoveOut(Transform target)
    {
        if (target == null)
            return;

        agent.SetDestination(target.position);
    }

    public void StopMove()
    {
        agent.isStopped = !agent.isStopped;
    }

    Transform GetRandomPatrolPoint()
    {
        if (patrolPoint == null || patrolPoint.Length == 0)
            return null;

        int index = Random.Range(0, patrolPoint.Length);
        return patrolPoint[index];
    }

    IEnumerator DetectEnemy()
    {
        while(!isDead)
        {
            targetAcquired = detector.DetectPlayer(out Transform detectedTarget);
    
            if (targetAcquired)
            {
                target = detectedTarget;
            }
            else
            {
                target = null;
            }
    
            yield return CoroutineCasher.Wait(0.03f);
        }
    
        StartCoroutine(OnDeath());
    }

    IEnumerator OnDeath()
    {
        spawner.OnEnemyDead();
        ChangeState(states[0]);
        Collider coll = GetComponent<Collider>();
        coll.enabled = false;
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var r in renderers)
        {
            r.enabled = false;
        }
        deathParticle.Play();
        moveAudio.audioSource.Stop();
        moveAudio.audioSource.PlayOneShot(deahtClip);
        yield return CoroutineCasher.Wait(1f);
        GetMedikit();
        gameObject.SetActive(false);
    }

    IEnumerator DamagedColor()
    {
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        

        foreach(var r in renderers)
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

    IEnumerator DamagedSlow()
    {
        agent.speed = speed * 0.5f;
        yield return CoroutineCasher.Wait(0.5f);
        agent.speed = speed;
    }
}
