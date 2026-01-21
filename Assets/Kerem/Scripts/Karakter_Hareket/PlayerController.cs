using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float walkSpeed = 5f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f; 
    public float jumpHeight = 3f;
    public float gravity = -35f;

    [Header("Referanslar")]
    public Transform kameraTakipNoktasi;
    private CharacterController controller;
    private Animator animator;
    private Camera mainCamera;

    private Vector3 velocity;
    private bool isGrounded;
    private float lastGroundedTime; 
    private float jumpCooldown = 0.2f; // Zıplama animasyonunun başlaması için süre
    private float lastJumpTime;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. GELİŞMİŞ YER KONTROLÜ
        bool isTouchingGround = controller.isGrounded;

        // Eğer yeni zıpladıysak, yer kontrolünü kısa süre geçersiz say
        if (Time.time - lastJumpTime < jumpCooldown)
        {
            isTouchingGround = false;
        }

        if (isTouchingGround)
        {
            lastGroundedTime = Time.time;
            isGrounded = true;
        }
        else
        {
            isGrounded = (Time.time - lastGroundedTime < 0.1f);
        }

        if (isTouchingGround && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. GİRDİLER
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isMoving = (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving; 
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // 3. BAKIŞ YÖNÜ
        float cameraYRotation = mainCamera.transform.eulerAngles.y;
        Quaternion targetRot = Quaternion.Euler(0, cameraYRotation, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        // 4. HAREKET
        Vector3 moveDir = transform.forward * v + transform.right * h;
        controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

        // 5. ZIPLAMA VE YERÇEKİMİ
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJump", true);
            lastJumpTime = Time.time; // Zıplama zamanını kaydet
            lastGroundedTime = 0;
        }
        
        // Sadece zıplama üzerinden belli bir süre geçtikten sonra yere inişi kontrol et
        if (isTouchingGround && Time.time - lastJumpTime > jumpCooldown)
        {
            animator.SetBool("IsJump", false);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 6. ANIMASYON KONTROLÜ
        float stateValue = isRunning ? 1f : 0f;
        animator.SetFloat("State", stateValue, 0.1f, Time.deltaTime);

        if (isMoving)
        {
            animator.SetFloat("Vert", v, 0.1f, Time.deltaTime);
            animator.SetFloat("Hor", h, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Vert", 0, 0.1f, Time.deltaTime);
            animator.SetFloat("Hor", 0, 0.1f, Time.deltaTime);
        }

        // 7. KAMERA TAKİP NOKTASI
        if (kameraTakipNoktasi != null)
        {
            kameraTakipNoktasi.position = transform.position + Vector3.up * 1.5f;
        }
    }
}