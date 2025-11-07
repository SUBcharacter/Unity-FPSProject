using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] public Animator animator;
    [SerializeField] Detector detector;
    [SerializeField] Image HPBar;
    [SerializeField] Transform target;
    EnemyState currentState;

    [SerializeField] bool targetAcquired = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        detector = GetComponentInChildren<Detector>();

        ChangeState(new IdleState());
    }

    private void Update()
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
        
        currentState.Update(this);
    }

    void ChangeState(EnemyState state)
    {
        currentState?.Exit(this);
        currentState = state;
        currentState.Start(this);
    }

}
