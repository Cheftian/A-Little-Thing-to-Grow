using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Kecepatan")]
    [Tooltip("Seret objek Player ke sini.")]
    public Transform player; 
    [Tooltip("Seberapa mulus kamera mengikuti player. Untuk Lerp, coba angka lebih besar seperti 5 atau 10.")]
    [SerializeField] private float smoothSpeed = 5f; // Nilai disesuaikan untuk Lerp

    [Header("Offset Kamera")]
    [Tooltip("Posisi default kamera relatif terhadap player.")]
    [SerializeField] private Vector3 offset; 
    
    [Header("Kontrol Manual Kamera")]
    [Tooltip("Kecepatan kamera bergerak saat tombol arah ditekan.")]
    [SerializeField] private float panSpeed = 4f;
    [Tooltip("Jarak maksimal kamera bisa bergerak dari posisi defaultnya.")]
    [SerializeField] private Vector2 panLimit; 
    [Tooltip("Kecepatan kamera kembali ke tengah saat tombol arah dilepas.")]
    [SerializeField] private float returnSpeed = 3f; 

    [Header("Batas Dunia Kamera")]
    [Tooltip("Posisi X minimal yang bisa dijangkau kamera.")]
    [SerializeField] private float minX;
    [Tooltip("Posisi X maksimal yang bisa dijangkau kamera.")]
    [SerializeField] private float maxX;
    [Tooltip("Posisi Y minimal yang bisa dijangkau kamera.")]
    [SerializeField] private float minY;
    [Tooltip("Posisi Y maksimal yang bisa dijangkau kamera.")]
    [SerializeField] private float maxY;

    private Vector2 manualOffset;
    void Update()
    {
        Vector2 panInput = Vector2.zero;
        if (Input.GetKey(KeyCode.RightArrow)) panInput.x = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) panInput.x = -1;
        if (Input.GetKey(KeyCode.UpArrow)) panInput.y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) panInput.y = -1;

        if (panInput.sqrMagnitude > 0.1f)
        {
            manualOffset += panInput * panSpeed * Time.deltaTime;
        }
        else
        {
            manualOffset = Vector2.Lerp(manualOffset, Vector2.zero, returnSpeed * Time.deltaTime);
        }

        manualOffset.x = Mathf.Clamp(manualOffset.x, -panLimit.x, panLimit.x);
        manualOffset.y = Mathf.Clamp(manualOffset.y, -panLimit.y, panLimit.y);
    }
    
    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset + (Vector3)manualOffset;
        
        desiredPosition.z = transform.position.z;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}