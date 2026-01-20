using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;
    public float jumpHeight = 3f;
    public float gravity = -35f;

    [Header("Referanslar")]
    public Transform kameraTakipNoktasi;
    private CharacterController controller;
    private Animator animator;
    private Camera mainCamera;

    private Vector3 velocity;
    private bool isGrounded;
    private float groundCheckDelay = 0.2f; // Zıplama sonrası kısa bir süre yer kontrolünü yoksay
    private float lastGroundedTime; // En son ne zaman yerdaydik?

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 1. GELİŞMİŞ YER KONTROLÜ
        if (controller.isGrounded)
        {
            lastGroundedTime = Time.time;
            isGrounded = true;
        }
        else
        {
            // Eğer 0.1 saniye içinde hala yerdeysek zıplamaya izin ver (Tolerans)
            isGrounded = (Time.time - lastGroundedTime < 0.1f);
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        // 2. GİRDİLER
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        // 3. ZIPLAMA (Anında Tepki)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJump", true);
            lastGroundedTime = 0; // Zıpladığı an yer kontrolünü sıfırla
        }
        else if (controller.isGrounded)
        {
            animator.SetBool("IsJump", false);
        }

        // 4. HAREKET VE DÖNÜŞ
        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 moveDir = targetRotation * Vector3.forward;
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }

        // 5. YERÇEKİMİ
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 6. GÖRSEL VE KAMERA
        animator.SetFloat("Vert", inputDir.magnitude, 0.1f, Time.deltaTime);

        if (kameraTakipNoktasi != null)
        {
            kameraTakipNoktasi.position = transform.position + Vector3.up * 1.5f;
        }
    }
}