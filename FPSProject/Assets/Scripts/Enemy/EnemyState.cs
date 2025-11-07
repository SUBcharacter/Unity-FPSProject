using UnityEngine;

public abstract class EnemyState
{
    public abstract void Start(Enemy enemy);
    public abstract void Update(Enemy enemy);
    public abstract void Exit(Enemy enemy);
}

public class IdleState : EnemyState
{
    float waitTime;
    float timer;

    public override void Start(Enemy enemy)
    {
        Debug.Log("가만히");
        enemy.animator.SetBool("Patrol", false);
        enemy.animator.SetBool("Detected", false);
        enemy.animator.SetBool("OnRange", false);
        timer = 0;
        waitTime = Random.Range(0, 4);
    }

    public override void Update(Enemy enemy)
    {
        if(enemy.targetAcquired)
        {
            enemy.ChangeState(enemy.states[2]);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            enemy.ChangeState(enemy.states[1]);
            return;
        }
    }

    public override void Exit(Enemy enemy)
    {
        timer = 0;
        waitTime = 0;
    }
}

public class PatrolState : EnemyState
{
    public override void Start(Enemy enemy)
    {
        enemy.animator.SetBool("Patrol", true);
        enemy.StartPatrol();
    }

    public override void Update(Enemy enemy)
    {
        if (enemy.targetAcquired)
        {
            enemy.ChangeState(enemy.states[2]);
            return;
        }

        if(enemy.ArrivedPoint())
        {
            enemy.ChangeState(enemy.states[0]);
            return;
        }
    }

    public override void Exit(Enemy enemy)
    {
        enemy.animator.SetBool("Patrol", false);
    }
}

public class ChaseState : EnemyState
{
    Transform lastKnownPos;
    float lostTime;
    float MaxLostTime = 3f;

    public override void Start(Enemy enemy)
    {
        enemy.animator.SetTrigger("Exposed");
        enemy.animator.SetBool("Detected", true);
        lostTime = 0f;
        lastKnownPos = enemy.target;
    }

    public override void Update(Enemy enemy)
    {
        if(enemy.targetAcquired && enemy.target != null)
        {
            Debug.Log("추적 중");
            lastKnownPos = enemy.target;
            enemy.MoveOut(lastKnownPos);

            float distance = Vector3.Distance(enemy.transform.position, lastKnownPos.position);
            if(distance <= enemy.attackDistance)
            {
                enemy.ChangeState(enemy.states[3]);
                return;
            }

            lostTime = 0f;
        }
        else
        {
            Debug.Log("놓침");
            if (lastKnownPos != null)
            {
                enemy.MoveOut(lastKnownPos);
            }

            lostTime += Time.deltaTime;

            if(enemy.ArrivedPoint() || lostTime >= MaxLostTime)
            {
                enemy.ChangeState(enemy.states[0]);
                return;
            }
        }
    }

    public override void Exit(Enemy enemy)
    {
        enemy.animator.SetBool("Detected", false);
    }
}

public class AttackState : EnemyState
{
    public override void Start(Enemy enemy)
    {
        Debug.Log("공격");
    }

    public override void Update(Enemy enemy)
    {
        
    }

    public override void Exit(Enemy enemy)
    {
        
    }
}
