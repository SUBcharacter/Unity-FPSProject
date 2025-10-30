using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class LocalPlayer : MonoBehaviour
{
    // ¾ÉÀ» ½Ã body YÁÂÇ¥ -0.5
    [SerializeField] Transform body;
    [SerializeField] Transform playerCamera;
    [SerializeField] CinemachineCamera fov;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform vertical;
    [SerializeField] Animator[] animators;

    [SerializeField] Vector2 mouseInputVec;
    [SerializeField] Vector2 moveInputVec;
    [SerializeField] Vector3 moveDir;

    [SerializeField] float verticalRotation = 0;
    [SerializeField] float currentSpeed =0;
    [SerializeField] float VelocityY;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float maxMouseSensitivity;
    [SerializeField] float defaultFov;
    [SerializeField] float aimFov;

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

        defaultFov = fov.Lens.FieldOfView;
        aimFov = fov.Lens.FieldOfView - 20;

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
        VelocityY = rigid.linearVelocity.y;
        Move();
        Crouching();
        AnimationControler();
    }

    private void FixedUpdate()
    {
        View();
        GroundCheck();
    }

    void AnimationControler()
    {
        if (isCrouching)
            isSprint = false;

        if (isReload)
            isSprint = false;

        Vector2 vector = new Vector2(rigid.linearVelocity.x, rigid.linearVelocity.z);
        float Velocity = vector.magnitude/(sprintSpeed-1);
        float CrouchVel = vector.magnitude / (walkSpeed / 2);
        float Yvelocity = VelocityY / jumpPower;
       
        animators[0].SetBool("Walk", isWalk);
        animators[0].SetBool("Run", isSprint);
        animators[0].SetBool("Aim", aiming);
        animators[0].SetBool("Reloading", isReload);
        animators[0].SetBool("Crouch", isCrouching);
        animators[0].SetBool("OnGround", isGround);

        animators[1].SetBool("Run", isSprint);
        animators[1].SetBool("Crouch", isCrouching);
        animators[1].SetBool("Reloading", isReload);
        animators[1].SetBool("OnGround", isGround);
        animators[1].SetFloat("Velocity", Velocity);
        animators[1].SetFloat("CrouchVelocity", CrouchVel);
        animators[1].SetFloat("VelocityY", Yvelocity);
        animators[1].SetFloat("DirX", moveInputVec.x);
        animators[1].SetFloat("DirZ", moveInputVec.y);

        animators[2].SetBool("Run", isSprint);
        animators[2].SetBool("Crouch", isCrouching);
        animators[2].SetBool("Reloading", isReload);
        animators[2].SetBool("OnGround", isGround); 
        animators[2].SetFloat("Velocity", Velocity);
        animators[2].SetFloat("CrouchVelocity", CrouchVel);
        animators[2].SetFloat("VelocityY", Yvelocity);
        animators[2].SetFloat("DirX", moveInputVec.x);
        animators[2].SetFloat("DirZ", moveInputVec.y);
    }

    void View()
    {
        float mouseX;
        float mouseY;
        if (aiming)
        {
            mouseX = mouseInputVec.x * (mouseSensitivity * 0.7f);
            mouseY = -mouseInputVec.y * (mouseSensitivity * 0.7f);
            fov.Lens.FieldOfView = Mathf.Lerp(fov.Lens.FieldOfView, aimFov, 10 * Time.deltaTime);
        }
        else
        {
            mouseX = mouseInputVec.x * mouseSensitivity;
            mouseY = -mouseInputVec.y * mouseSensitivity;
            fov.Lens.FieldOfView = Mathf.Lerp(fov.Lens.FieldOfView, defaultFov, 10 * Time.deltaTime);
        }
        

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
        if (!isGround)
            return;

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

    void GroundCheck()
    {
        CapsuleCollider coll = GetComponent<CapsuleCollider>();
        Vector3 origin = transform.position;
        Vector3 rayOrigin = transform.position + Vector3.down * (coll.height / 2f - coll.radius + 0.01f);
        int mask = LayerMask.GetMask("Terrain");

        isGround = Physics.Raycast(rayOrigin, Vector3.down, 0.25f,mask);

        rigid.useGravity = !isGround;
        
        
        Debug.DrawRay(rayOrigin, Vector3.down * 0.25f, isGround ? Color.green : Color.red);
    }

    void Crouching()
    {
        float offset = isCrouching ? -0.5f : 0;
        Vector3 pos = body.localPosition;
        pos.y = Mathf.Lerp(pos.y, offset,1);
        body.localPosition = pos;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseInputVec = context.ReadValue<Vector2>();
        
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
            isReload = false;
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
        if (!context.performed || aiming || isReload)
            return;

        animators[0].SetTrigger("Reload");
        animators[1].SetTrigger("Reload");
        animators[2].SetTrigger("Reload");
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
            if(isCrouching)
            {
                isSprint = false;
            }
        }
    }
}
