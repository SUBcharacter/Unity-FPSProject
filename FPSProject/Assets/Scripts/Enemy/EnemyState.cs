using UnityEngine;

public abstract class EnemyState
{
    public abstract void Start(Enemy enemy);
    public abstract void Update(Enemy enemy);
    public abstract void Exit(Enemy enemy);
}

public class IdleState : EnemyState
{
    public override void Start(Enemy enemy)
    {
        
    }

    public override void Update(Enemy enemy)
    {

    }

    public override void Exit(Enemy enemy)
    {
            
    }
}

public class PatrolState : EnemyState
{
    public override void Start(Enemy enemy)
    {
        
    }

    public override void Update(Enemy enemy)
    {
        
    }

    public override void Exit(Enemy enemy)
    {
        
    }
}

public class ChaseState : EnemyState
{
    public override void Start(Enemy enemy)
    {

    }

    public override void Update(Enemy enemy)
    {

    }

    public override void Exit(Enemy enemy)
    {

    }
}

public class AttackState : EnemyState
{
    public override void Start(Enemy enemy)
    {
        
    }

    public override void Update(Enemy enemy)
    {
        
    }

    public override void Exit(Enemy enemy)
    {
        
    }
}
