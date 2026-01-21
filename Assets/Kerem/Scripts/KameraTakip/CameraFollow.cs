using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;      
    [Header("Hız Ayarları")]
    public float sensitivity = 5f;      // Daha küçük, insani rakamlar kullanabilirsin (1-10 arası)
    public float smoothing = 10f;       // Dönüşün yumuşaklığı (Daha yüksek = Daha sert/anlık)

    private float rotationX = 0f;
    private float rotationY = 0f;
    private float currentX = 0f;
    private float currentY = 0f;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationY = rot.y;
        rotationX = rot.x;
        currentX = rotationX;
        currentY = rotationY;
    }

    void LateUpdate()
    {
        if (player == null) return;

        // 1. Karakteri takip et
        transform.position = player.position;

        // 2. Sağ tık kontrolü
        if (Input.GetMouseButton(1))
        {
            // GetAxisRaw kullanarak farenin saf hareketini alıyoruz (Hız hissi için daha iyidir)
            rotationY += Input.GetAxisRaw("Mouse X") * sensitivity;
            rotationX -= Input.GetAxisRaw("Mouse Y") * sensitivity;

            rotationX = Mathf.Clamp(rotationX, -20f, 45f);
        }

        // 3. Smooth (Yumuşatma) İşlemi
        // Lerp kullanarak anlık dönüşü yumuşatıyoruz, böylece "smooth" hissi oluşuyor
        currentX = Mathf.Lerp(currentX, rotationX, Time.deltaTime * smoothing);
        currentY = Mathf.Lerp(currentY, rotationY, Time.deltaTime * smoothing);

        transform.localRotation = Quaternion.Euler(currentX, currentY, 0);
    }
}