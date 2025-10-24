using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform vertical;
    [SerializeField] Animator[] animators;

    [SerializeField] Vector2 mouseInputVec;
    [SerializeField] Vector2 moveInputVec;
    [SerializeField] Vector3 moveDir;

    [SerializeField] float verticalRotation = 0;
    [SerializeField] float currentSpeed =0;
    [SerializeField] float moveSpeed;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float maxMouseSensitivity;
    [SerializeField] bool isWalk = false;

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
        Move();
        AnimationControler();
    }

    void AnimationControler()
    {
        Vector2 vector = new Vector2(rigid.linearVelocity.x, rigid.linearVelocity.z);
        float Velocity = vector.magnitude;
        if(isWalk)
        {
            animators[0].SetBool("Walk", true);
        }
        else
        {
            animators[0].SetBool("Walk", false);
        }

        animators[1].SetFloat("Velocity", Velocity);
        animators[2].SetFloat("Velocity", Velocity);

        animators[1].SetFloat("DirX", moveInputVec.x);
        animators[1].SetFloat("DirZ", moveInputVec.y);
        animators[2].SetFloat("DirX", moveInputVec.x);
        animators[2].SetFloat("DirZ", moveInputVec.y);
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
        Vector3 cameraForward = playerCamera.forward;
        Vector3 cameraRight = playerCamera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDir = cameraRight*moveInputVec.x + cameraForward*moveInputVec.y;
        if(moveInputVec.magnitude > 0.1f)
        {
            currentSpeed += 5f * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, moveSpeed);
        }
        

        Vector3 velocity = moveDir * currentSpeed;
        velocity.y = rigid.linearVelocity.y;
        rigid.linearVelocity = velocity;
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
            isWalk = true;
        }
        else if(context.canceled)
        {
            moveInputVec = new Vector2(0, 0);
            currentSpeed = 0;
            isWalk = false;
        }
    }
}
