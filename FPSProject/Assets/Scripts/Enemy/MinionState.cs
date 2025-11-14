using UnityEngine;

public abstract class MinionState
{
    public abstract void Start(Minion enemy);
    public abstract void Update(Minion enemy);
    public abstract void Exit(Minion enemy);
}

public class WaitState : MinionState
{
    float waitTime;
    float timer;

    public override void Start(Minion enemy)
    {
        Debug.Log("가만히");
        enemy.animator.SetBool("Patrol", false);
        enemy.animator.SetBool("Detected", false);
        enemy.animator.SetBool("OnRange", false);
        timer = 0;
        waitTime = Random.Range(0, 4);
    }

    public override void Update(Minion enemy)
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

    public override void Exit(Minion enemy)
    {
        timer = 0;
        waitTime = 0;
    }
}

public class SearchState : MinionState
{
    public override void Start(Minion enemy)
    {
        enemy.animator.SetBool("Patrol", true);
        enemy.StartPatrol();
    }

    public override void Update(Minion enemy)
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

    public override void Exit(Minion enemy)
    {
        enemy.animator.SetBool("Patrol", false);
    }
}

public class PersuingState : MinionState
{
    Transform lastKnownPos;
    float lostTime;
    float MaxLostTime = 3f;

    public override void Start(Minion enemy)
    {
        enemy.animator.SetTrigger("Exposed");
        enemy.animator.SetBool("Detected", true);
        enemy.moveAudio.PlayOneShot(enemy.alertClip);
        lostTime = 0f;
        lastKnownPos = enemy.target;
    }

    public override void Update(Minion enemy)
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

    public override void Exit(Minion enemy)
    {
        enemy.animator.SetBool("Detected", false);
    }
}

public class EngagingState : MinionState
{
    float timer;
    float attackTimer = 3f;

    public override void Start(Minion enemy)
    {
        Debug.Log("공격");
        enemy.animator.SetBool("OnRange", true);
        enemy.StopMove();
        timer = 0;
    }

    public override void Update(Minion enemy)
    {
        if(enemy.target == null)
        {
            enemy.ChangeState(enemy.states[0]);
            return;
        }

        Vector3 dir = enemy.target.position - enemy.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            enemy.transform.rotation = rot;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);

        if(distance > enemy.attackDistance)
        {
            enemy.ChangeState(enemy.states[2]);
            return;
        }
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = attackTimer;
            enemy.Shoot();
        }
    }

    public override void Exit(Minion enemy)
    {
        enemy.StopMove();
        enemy.animator.SetBool("OnRange", false);
    }
}
