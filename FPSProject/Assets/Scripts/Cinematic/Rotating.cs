using UnityEngine;

public class Rotating : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        transform.Rotate(0, 10 * Time.deltaTime, 0);
    }
}
