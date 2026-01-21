using UnityEngine;

public class BallInteraction : MonoBehaviour
{
    [Header("Ayarlar")]
    public float interactionRange = 3f;
    public float throwForce = 35f; // Fırlatma hızı
    public Transform holdingPoint; // Topun duracağı yer
    
    [Header("Referanslar")]
    private GameObject currentBall;
    private bool isHoldingBall = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // E ile topu al
        if (Input.GetKeyDown(KeyCode.E) && !isHoldingBall)
        {
            TryPickUpBall();
        }

        // Mouse Sol Tık ile fırlat
        if (Input.GetMouseButtonDown(0) && isHoldingBall)
        {
            StartThrowSequence();
        }
    }

    void TryPickUpBall()
    {
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

    // 1. Fizikleri Kapat (Topun bağımsız hareket etmesini engeller)
    Rigidbody rb = currentBall.GetComponent<Rigidbody>();
    if (rb != null) 
    {
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    // 2. Ele Sabitleme İşlemi
    // Eğer holdingPoint atanmamışsa hata vermemesi için kontrol ekledik
    if (holdingPoint != null)
    {
        currentBall.transform.SetParent(holdingPoint);
        
        // localPosition = 0,0,0 demek; topun tam olarak holdingPoint'in MERKEZİNE gitmesi demektir.
        currentBall.transform.localPosition = Vector3.zero;
        currentBall.transform.localRotation = Quaternion.identity;
    }
    else
    {
        Debug.LogError("Holding Point atanmamış! Lütfen Inspector'dan bir obje sürükle.");
    }

    // 3. Animasyonu Başlat
    animator.SetLayerWeight(1, 1f);
    animator.Play("BallHoldPose", 1, 0f); 
}

    void StartThrowSequence()
    {
        // Animator'daki fırlatma animasyonunu tetikle
        animator.SetTrigger("ThrowTrigger");
        
        // Gerçek fırlatma işlemini (ThrowBall) birazdan 3. adımda anlatacağım 
        // "Animation Event" ile çağıracağız. Ama şimdilik buradan çağırıp test edebilirsin:
        // ThrowBall(); 
    }

    // BU FONKSİYON TOPU ELDEN ÇIKARIR
    public void ThrowBall()
    {
        if (currentBall == null) return;

        isHoldingBall = false;
        
        // Bağlantıyı kopar
        currentBall.transform.SetParent(null);
        
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // Kameranın tam merkezine doğru fırlat
            Vector3 throwDir = Camera.main.transform.forward;
            rb.AddForce(throwDir * throwForce, ForceMode.Impulse);
        }

        currentBall = null;
        
        // Katman ağırlığını zamanla veya anında sıfırla
        animator.SetLayerWeight(1, 0f);
    }
}