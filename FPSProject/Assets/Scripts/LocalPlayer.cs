using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LocalPlayer : MonoBehaviour
{
    // ¾ÉÀ» ½Ã body YÁÂÇ¥ -0.5
    [SerializeField] Transform body;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform firePoint;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] ParticleSystem spark;
    [SerializeField] Light muzzleLight;
    [SerializeField] CinemachineCamera fov;
    [SerializeField] Camera cam;
    [SerializeField] Rigidbody rigid;
    [SerializeField] Transform vertical;
    [SerializeField] Animator[] animators;
    [SerializeField] Magazine magazine;
    [SerializeField] AudioClip shootSound;
    [SerializeField] AudioClip walkSound;
    [SerializeField] AudioClip runSound;
    [SerializeField] AudioClip aimingSound;
    [SerializeField] AudioClip reloadSound1;
    [SerializeField] AudioClip reloadSound2;
    [SerializeField] AudioClip holsterIn;
    [SerializeField] AudioClip holsterOut;
    [SerializeField] Image crosshair;


    [SerializeField] Vector2 mouseInputVec;
    [SerializeField] Vector2 moveInputVec;
    [SerializeField] Vector3 moveDir;
    [SerializeField] LayerMask shootAble;

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
    [SerializeField] float fireRate;
    [SerializeField] float recoilForce;
    [SerializeField] float recoilVerticalOffset;
    [SerializeField] float recoilHorizontalOffset;

    [SerializeField] int bulletCount;
    [SerializeField] int fullMagazine;

    [SerializeField] bool isWalk;
    [SerializeField] bool isSprint;
    [SerializeField] bool isReload;
    [SerializeField] bool aiming;
    [SerializeField] bool isGround;
    [SerializeField] bool isCrouching;
    [SerializeField] bool isFiring;

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
        isGround = false;
        isCrouching = false;
        isFiring = false;
    }

    
    void Start()
    {
        
    }

   
    void Update()
    {
        VelocityY = rigid.linearVelocity.y;
        RecoilRecovery();
        AnimationControler();
        MoveSoundControler();
    }

    private void FixedUpdate()
    {
        View();
        Move();
        Crouching();
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
    
    void MoveSoundControler()
    {
        if (!isGround)
        {
            PlayerMoveAudio.audioSource.Stop();
            return;
        }
            

        if(isWalk)
        {
            if(isSprint && !aiming)
            {
                PlayerMoveAudio.audioSource.clip = runSound;
                PlayerMoveAudio.audioSource.loop = true;
            }
            else
            {
                if(aiming)
                {
                    PlayerMoveAudio.audioSource.pitch = 0.5f;
                    PlayerMoveAudio.audioSource.clip = walkSound;
                    PlayerMoveAudio.audioSource.loop = true;
                }
                else
                {
                    PlayerMoveAudio.audioSource.pitch = 1f;
                    PlayerMoveAudio.audioSource.clip = walkSound;
                    PlayerMoveAudio.audioSource.loop = true;
                }
                
            }

            if (!PlayerMoveAudio.audioSource.isPlaying)
            {
                PlayerMoveAudio.audioSource.Play();
            }
        }
        else
        {
            PlayerMoveAudio.audioSource.Stop();
        }

        

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
            Color color = crosshair.color;
            color.a = Mathf.Lerp(color.a, 0, 10);
            crosshair.color = color;
        }
        else
        {
            mouseX = mouseInputVec.x * mouseSensitivity;
            mouseY = -mouseInputVec.y * mouseSensitivity;
            fov.Lens.FieldOfView = Mathf.Lerp(fov.Lens.FieldOfView, defaultFov, 10 * Time.deltaTime);
            Color color = crosshair.color;
            color.a = Mathf.Lerp(color.a, 255, 10);
            crosshair.color = color;
        }
        

        verticalRotation += mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);

        vertical.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up, mouseX);


    }
    #region AnimationEventFunc
    public void Reloading()
    {
        isReload = !isReload;
    }

    public void FillUp()
    {
        bulletCount = fullMagazine;
    }

    public void MuzzleFlashOn()
    {
        muzzleFlash.Play();
        spark.Play();
        StartCoroutine(MuzzleFlashLight());
    }

    public void MuzzleFlashOff()
    {
        muzzleFlash.Stop();
        spark.Stop();
    }
    public void Launch()
    {
        if (bulletCount <= 0)
            return;
        bulletCount--;
        Ray ray;
        RaycastHit hit;
        if (!aiming)
        {
            ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Vector2 spread = Random.insideUnitCircle * 0.0521f;
            ray.direction = (ray.direction + cam.transform.right * spread.x + cam.transform.up * spread.y).normalized;
        }
        else
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        Vector3 targetPoint = (Physics.Raycast(ray, out hit, 100f, shootAble)) ? hit.point : ray.origin + ray.direction * 100f;

        Vector3 dir = (targetPoint - firePoint.position).normalized;

        Vector3 firePos = firePoint.position;

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

        magazine.Fire(dir, firePos);
        PlayerShotAudio.PlaySound(shootSound);

        StartCoroutine(Recoil());
    }

    public void HolsterInSound()
    {
        PlayerActAudio.PlaySound(holsterIn);
    }

    public void HolsterOutSound()
    {
        PlayerActAudio.PlaySound(holsterOut);
    }

    #endregion
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
        float radius = coll.radius * 0.9f;
        float checkDist = 0.25f;
        Vector3 rayOrigin = transform.position + Vector3.down * (coll.height / 2f - coll.radius + 0.01f);
        int mask = LayerMask.GetMask("Terrain");

        isGround = Physics.SphereCast(rayOrigin, radius, Vector3.down, out RaycastHit hit, checkDist, mask); 

        rigid.useGravity = !isGround;
        
        
        Debug.DrawRay(rayOrigin, Vector3.down * checkDist, isGround ? Color.green : Color.red);
    }

    void Crouching()
    {
        float offset = isCrouching ? -0.5f : 0;
        Vector3 pos = body.localPosition;
        pos.y = Mathf.Lerp(pos.y, offset,1);
        body.localPosition = pos;
    }

    

    void RecoilRecovery()
    {
        recoilVerticalOffset = Mathf.Lerp(recoilVerticalOffset, 0f, 0.5f);
        recoilHorizontalOffset = Mathf.Lerp(recoilHorizontalOffset, 0f, 0.5f);

        verticalRotation -= recoilVerticalOffset;
        transform.Rotate(Vector3.up, recoilHorizontalOffset);
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
            if (bulletCount <= 0)
                return;
            isFiring = true;
            isReload = false;
            StartCoroutine(Fire());
        }
        else if(context.canceled)
        {
            isFiring = false;
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            aiming = true;
            PlayerActAudio.PlaySound(aimingSound);
        }
        else if(context.canceled)
        {
            aiming = false;
            PlayerActAudio.PlaySound(aimingSound);
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (!context.performed || aiming || isReload)
            return;


        if(bulletCount <= 0)
        {
            animators[0].SetTrigger("ReloadEmpty");
            PlayerActAudio.PlaySound(reloadSound2);
        }
        else
        {
            animators[0].SetTrigger("ReloadLeft");
            PlayerActAudio.PlaySound(reloadSound1);
        }
        
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
                isReload = false;
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

    IEnumerator MuzzleFlashLight()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
    }

    IEnumerator Fire()
    {
        while(isFiring)
        {
            if (bulletCount <= 0)
                yield break;

            if (!aiming)
            {
                animators[0].Play("Fire",0,0f);
            }
            else
            {
                animators[0].Play("Aim Fire",0,0f);
            }
            yield return CoroutineCasher.Wait(fireRate);
        }
    }

    IEnumerator Recoil()
    {
        if(!aiming)
        {
            yield break;
        }
        recoilVerticalOffset += recoilForce;

        float horizontalRecoil = 0.5f;
        float randomHoriReco = Random.Range(-0.03f, 0.03f);

        recoilHorizontalOffset += horizontalRecoil + randomHoriReco;


        yield return null;
    }
}
