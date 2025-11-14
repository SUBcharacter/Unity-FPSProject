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

}
