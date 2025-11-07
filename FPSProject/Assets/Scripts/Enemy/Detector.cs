using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField] float viewDistance;
    [SerializeField] float viewAngle;
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;


    public bool DetectPlayer(out Transform target)
    {
        target = null;

        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewDistance,targetMask);

        if(targetsInView.Length <= 0 )
        {
            return false;
        }

        foreach(var t in targetsInView)
        {
            Vector3 dirToPlayer = (t.transform.position - transform.position).normalized;

            float dot = Vector3.Dot(transform.forward, dirToPlayer);
            float cos = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad);

            if (dot < cos)
            {
                continue;
            }
            else
            {
                if (!Physics.Linecast(transform.position, t.transform.position, obstacleMask))
                {
                    target = t.transform;
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }
}
