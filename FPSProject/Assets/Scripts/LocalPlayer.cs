using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform vertical;
    [SerializeField] Animator[] animators;

    [SerializeField] Vector2 mouseInputVec;
    [SerializeField] Vector2 moveInputVec;

    [SerializeField] float verticalRotation = 0;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float maxMouseSensitivity;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animators = GetComponentsInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void View()
    {
        float mouseX = mouseInputVec.x * mouseSensitivity;
        float mouseY = -mouseInputVec.y * mouseSensitivity;

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);

        vertical.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up, mouseX);
    }

    void Move()
    {

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseInputVec = context.ReadValue<Vector2>();
        View();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            moveInputVec = context.ReadValue<Vector2>();
        }
        else if(context.canceled)
        {
            moveInputVec = new Vector2(0, 0);
        }
    }
}
