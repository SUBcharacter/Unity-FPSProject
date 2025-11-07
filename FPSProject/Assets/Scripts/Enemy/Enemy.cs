using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public Transform target;
    public List<EnemyState> states;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Detector detector;
    [SerializeField] Image HPBar;
    [SerializeField] Transform patrolTarget;
    [SerializeField] Transform[] patrolPoint;
    [SerializeField] Magazine mag;
    [SerializeField] ParticleSystem deathParticle;
    EnemyState currentState;
    Coroutine slow;

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
        health = maxHealth;
        speed = agent.speed;
        states = new List<EnemyState>();
        states.Add(new IdleState());
        states.Add(new PatrolState());
        states.Add(new ChaseState());
        states.Add(new AttackState());
        currentState = states[0];
        StartCoroutine(DetectEnemy());
    }

    private void Update()
    {
        currentState.Update(this);
    }

    public void ChangeState(EnemyState state)
    {
        currentState?.Exit(this);
        currentState = state;
        currentState.Start(this);
    }

    public void Init(Vector3 pos)
    {
        transform.position = pos;
        isDead = false;
        health = maxHealth;
        ChangeState(states[0]);
        gameObject.SetActive(true); 
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

    public bool ArrivedPoint()
    {
        return !agent.pathPending && agent.remainingDistance <= 0.3f;
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

    public void MoveOut(Transform target)
    {
        if (target == null)
            return;

        agent.SetDestination(target.position);
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
        deathParticle.Play();
        yield return CoroutineCasher.Wait(1f);
        gameObject.SetActive(false);
    }

    IEnumerator Shoot()
    {
        while(targetAcquired)
        {
            yield return CoroutineCasher.Wait(2f);
        }
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
