using UnityEngine;

public class Rotating : MonoBehaviour
{ 
    void Update()
    {
        transform.Rotate(0, 10 * Time.deltaTime, 0);
    }
}
