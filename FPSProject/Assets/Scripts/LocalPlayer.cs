using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform horizontal;
    [SerializeField] Animator[] animators;

    [SerializeField] Vector2 horizonFovVector;
    [SerializeField] Vector2 verticalFovVector;

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

    }
}
