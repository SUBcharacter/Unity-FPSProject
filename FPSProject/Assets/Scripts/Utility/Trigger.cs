using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] public Transform player;
    public bool entered = false;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        entered = true;
        player = other.transform;
    }
}
