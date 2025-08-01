using UnityEngine;
using System.Collections;

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
    private Vector3 velocity = Vector3.zero; // Kita kembalikan untuk SmoothDamp yang lebih stabil

    // --- VARIABEL BARU UNTUK KONTROL MANUAL & SHAKE ---
    private bool isFollowingPlayer = true;
    private Vector3 shakeOffset = Vector3.zero;

    private Camera cam;
    private float originalOrthoSize;


    void Start()
    {
        // Ambil komponen kamera dan simpan ukuran zoom aslinya
        cam = GetComponent<Camera>();
        originalOrthoSize = cam.orthographicSize;
    }

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
        if (!isFollowingPlayer) return;
        if (player == null) return;
        
        Vector3 desiredPosition = player.position + offset + (Vector3)manualOffset;
        desiredPosition.z = transform.position.z;
        
        // Batasi posisi
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        
        // Gerakkan kamera ke posisi yang diinginkan ditambah efek getar
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition + shakeOffset, ref velocity, smoothSpeed);
    }

    public void FocusOnPoint(Vector3 focusPoint, float zoomLevel, float moveDuration)
    {
        isFollowingPlayer = false;
        StartCoroutine(MoveAndZoomRoutine(focusPoint, zoomLevel, moveDuration));
    }

    public void ReturnToPlayerFocus(float moveDuration)
    {
        StartCoroutine(MoveAndZoomRoutine(player.position + offset, originalOrthoSize, moveDuration));
        isFollowingPlayer = true;
    }

    private IEnumerator MoveAndZoomRoutine(Vector3 targetPosition, float targetSize, float duration)
    {
        float timer = 0f;
        Vector3 startPosition = transform.position;
        float startSize = cam.orthographicSize;

        while (timer < duration)
        {
            // Interpolasi posisi dan zoom secara bersamaan
            transform.position = Vector3.Lerp(startPosition, new Vector3(targetPosition.x, targetPosition.y, transform.position.z), timer / duration);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Pastikan posisi dan zoom tepat di akhir
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        cam.orthographicSize = targetSize;
    }
    
    public void FocusOnTarget(Transform target, float duration)
    {
        StartCoroutine(FocusRoutine(target, duration));
    }

    // Untuk memicu efek getar
    public void TriggerShake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator FocusRoutine(Transform target, float duration)
    {
        isFollowingPlayer = false;
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        
        // Bergerak ke target
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.2f);
            yield return null;
        }

        // Diam di target selama durasi tertentu
        yield return new WaitForSeconds(duration);

        // Kembali mengikuti player
        isFollowingPlayer = true;
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float timer = 0f;
        while (timer < duration)
        {
            shakeOffset = Random.insideUnitSphere * magnitude;
            timer += Time.deltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}