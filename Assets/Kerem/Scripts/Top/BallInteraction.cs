using UnityEngine;

public class BallInteraction : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform ballSocket;        // Inspector'dan BallSocket objesini buraya sürükle
    public float interactionRange = 2f; // Topu alma mesafesi

    [Header("Referanslar")]
    private Animator animator;          // Animator referansı (Hata almamak için gerekli)
    private GameObject currentBall;
    private bool isHoldingBall = false;

    void Start()
    {
        // Scriptin çalıştığı objedeki (karakter) Animator'ı otomatik bulur
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // E tuşuna basıldığında
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHoldingBall)
            {
                TryPickUpBall();
            }
            else
            {
                DropBall();
            }
        }
    }

    void TryPickUpBall()
    {
        // Çevredeki tüm objeleri kontrol et
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Ball"))
            {
                PickUp(hit.gameObject);
                break;
            }
        }
    }

    void PickUp(GameObject ball)
    {
        currentBall = ball;
        isHoldingBall = true;

        // Topu eldeki sokete bağla
        ball.transform.SetParent(ballSocket);
        
        // Pozisyonu ve dönüşü sokete göre sıfırla
        ball.transform.localPosition = Vector3.zero;
        ball.transform.localRotation = Quaternion.identity;

        // Topun fiziğini kapat (elden düşmemesi için)
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // --- ANIMASYON KATMANI KONTROLÜ ---
        // HoldingLayer'ın indexi 1'dir. Topu alınca katmanı aktif et.
        if (animator != null)
        {
            animator.SetLayerWeight(1, 1f);
        }
    }

    void DropBall()
    {
        if (currentBall != null)
        {
            currentBall.transform.SetParent(null); // Bağlantıyı kopar
            
            Rigidbody rb = currentBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                // Bırakırken hafifçe ileri fırlat (Metin2 tarzı pürüzsüz bırakış)
                rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
            }

            currentBall = null;
            isHoldingBall = false;

            // --- ANIMASYON KATMANI KONTROLÜ ---
            // Topu bırakınca katmanı kapat, kol normal yürüme animasyonuna dönsün.
            if (animator != null)
            {
                animator.SetLayerWeight(1, 0f);
            }
        }
    }

    // Menzili sahnede sarı bir küre olarak görmek için
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}