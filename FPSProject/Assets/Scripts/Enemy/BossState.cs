using System.Collections;
using UnityEngine;

public abstract class BossState
{
    public abstract void Start(Boss boss);
    public abstract void Update(Boss boss);
    public abstract void LateUpdate(Boss boss);
    public abstract void Exit(Boss boss);
}

public class StandByState : BossState
{
    public override void Start(Boss boss)
    {
        boss.ShieldActive(true);
    }

    public override void Update(Boss boss)
    {
        if(boss.target)
        {
            boss.ChangeState(boss.states[1]);
            return;
        }
    }

    public override void LateUpdate(Boss boss)
    {

    }

    public override void Exit(Boss boss)
    {

    }
}
    
public class DeactivateState : BossState
{
    public override void Start(Boss boss)
    {
        boss.actAudio.spatialBlend = 0;
        boss.ShieldActive(true);
        boss.minions.WaveStart();
    }

    public override void Update(Boss boss)
    {
        if(boss.minions.activeEnemyCount <= 0)
        {
            boss.ChangeState(boss.states[2]);
            return;
        }
    }

    public override void LateUpdate(Boss boss)
    {

    }

    public override void Exit(Boss boss)
    {
        boss.ShieldActive(false);
    }
}
public class ActivateState : BossState
{
    float timer;
    float engageTime = 10f;
    public override void Start(Boss boss)
    {
        timer = 0;
        boss.ShieldActive(false);
        boss.animator.SetBool("IsActive", true);
        boss.Engage();
    }

    public override void Update(Boss boss)
    {
        if (boss.health <= 0)
        {
            boss.ChangeState(boss.states[3]);
            return;
        }

        timer += Time.deltaTime;
        if(timer > engageTime)
        {
            boss.ChangeState(boss.states[1]);
            return;
        }
    }

    public override void LateUpdate(Boss boss)
    {
        boss.HeadRotate();
    }

    public override void Exit(Boss boss)
    {
        boss.StopEngage();
        boss.animator.SetBool("IsActive", false);
        boss.ShieldActive(true);
    }
}

public class DestroyState : BossState
{
    public override void Start(Boss boss)
    {
        boss.Destroy();
    }

    public override void Update(Boss boss)
    {
        
    }

    public override void LateUpdate(Boss boss)
    {

    }

    public override void Exit(Boss boss)
    {

    }
}

