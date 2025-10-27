using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayer : MonoBehaviour
{
    // ¾ÉÀ» ½Ã body YÁÂÇ¥ -0.5
    [SerializeField] Transform body;
    [SerializeField] Transform playerCamera;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform vertical;
    [SerializeField] Animator[] animators;

    [SerializeField] Vector2 mouseInputVec;
    [SerializeField] Vector2 moveInputVec;
    [SerializeField] Vector3 moveDir;

    [SerializeField] float verticalRotation = 0;
    [SerializeField] float currentSpeed =0;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float maxMouseSensitivity;

    [SerializeField] bool isWalk;
    [SerializeField] bool isSprint;
    [SerializeField] bool isReload;
    [SerializeField] bool aiming;
    [SerializeField] bool isGround;
    [SerializeField] bool isCrouching;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animators = GetComponentsInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        isWalk = false;
        isSprint = false;
        isReload = false;
        aiming = false;
        isGround = true;
        isCrouching = false;
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
        float Velocity = vector.magnitude/(sprintSpeed-1);
        float CrouchVel = vector.magnitude / (walkSpeed / 2);
       
        animators[0].SetBool("Walk", isWalk);
        animators[0].SetBool("Run", isSprint);
        animators[0].SetBool("Aim", aiming);
        animators[0].SetBool("Reloading", isReload);

        animators[1].SetBool("Crouch", isCrouching);
        animators[2].SetBool("Crouch", isCrouching);
        animators[1].SetFloat("Velocity", Velocity);
        animators[1].SetFloat("CrouchVelocity", CrouchVel);
        animators[2].SetFloat("Velocity", Velocity);
        animators[2].SetFloat("CrouchVelocity", CrouchVel);

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

    public void Reloading()
    {
        isReload = !isReload;
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
            
            if(isSprint && !isReload && !isCrouching)
            {
                if (aiming)
                {
                    currentSpeed += 5f * Time.fixedDeltaTime;
                    currentSpeed = Mathf.Min(currentSpeed, walkSpeed / 2);
                }
                else
                {
                    currentSpeed += 10f * Time.fixedDeltaTime;
                    currentSpeed = Mathf.Min(currentSpeed, sprintSpeed);
                }
                
            }
            else
            {
                if (aiming || isCrouching)
                {
                    currentSpeed += 5f * Time.fixedDeltaTime;
                    currentSpeed = Mathf.Min(currentSpeed, walkSpeed / 2);
                }
                else
                {
                    currentSpeed += 5f * Time.fixedDeltaTime;
                    currentSpeed = Mathf.Min(currentSpeed, walkSpeed);
                }
                
            }
            
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
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isSprint = true;
            isCrouching = false;
        }
        else if(context.canceled)
        {
            isSprint = false;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            animators[0].SetTrigger("Shoot");                                                       
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            aiming = true;
        }
        else if(context.canceled)
        {
            aiming = false;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        animators[0].SetTrigger("Reload");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGround)
        {
            if(isCrouching)
            {
                isCrouching = false;
            }
            else
            {
                Vector3 jumpVel = rigid.linearVelocity;
                jumpVel.y = jumpPower;
                rigid.linearVelocity = jumpVel;
            }
        }
        
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            isCrouching = !isCrouching;
        }
    }
}
