using UnityEngine;

public abstract class BossState
{
    public abstract void Start(Boss boss);
    public abstract void Update(Boss boss);
    public abstract void Exit(Boss boss);
}

public class SleepState : BossState
{
    public override void Start(Boss boss)
    {
        
    }
    public override void Update(Boss boss)
    {

    }
    public override void Exit(Boss boss)
    {

    }
}
public class EngageState : BossState
{
    public override void Start(Boss boss)
    {

    }
    public override void Update(Boss boss)
    {

    }
    public override void Exit(Boss boss)
    {

    }
}
public class SkillState : BossState
{
    public override void Start(Boss boss)
    {

    }
    public override void Update(Boss boss)
    {

    }
    public override void Exit(Boss boss)
    {

    }
}

