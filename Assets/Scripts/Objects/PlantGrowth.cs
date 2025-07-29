using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GrowthStageData
{
    public string stageName;
    public Sprite stemSprite;
    public GameObject[] leavesToActivate;
}

public class PlantGrowth : MonoBehaviour
{
    [Header("Referensi Visual")]
    [SerializeField] private SpriteRenderer stemRenderer;
    [SerializeField] private List<GameObject> allLeaves; // Pastikan semua 14 daun ada di sini
    [SerializeField] private CameraController mainCamera;

    [Header("Pengaturan Pertumbuhan")]
    [SerializeField] private List<GrowthStageData> growthStages;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float leafAppearDuration = 0.4f;
    [SerializeField] private float shakeDuration = 0.4f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    [Header("Pengaturan Sinematik")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private float cameraZoomLevel = 10f;
    [SerializeField] private float cameraMoveDuration = 0.5f;

    // Status Tanaman
    private int currentStage = -1;
    private bool isWatered = false;
    private bool isFertilized = false;
    private bool isGrowing = false;

    void Start()
    {
        // Menonaktifkan semua bagian di awal
        stemRenderer.gameObject.SetActive(false);
        foreach (var leaf in allLeaves)
        {
            leaf.SetActive(false);
        }
    }

    public bool ApplyWater()
    {
        if (isWatered || isGrowing) return false;
        isWatered = true;
        Debug.Log("Tanaman disiram!");
        CheckConditionsAndGrow();
        return true;
    }

    public bool ApplyFertilizer()
    {
        if (isFertilized || isGrowing) return false;
        isFertilized = true;
        Debug.Log("Tanaman dipupuk!");
        CheckConditionsAndGrow();
        return true;
    }

    public void AttemptUpgrade()
    {
        if (currentStage == -1 && !isGrowing)
        {
            StartCoroutine(GrowthSequenceRoutine());
        }
    }

    private void CheckConditionsAndGrow()
    {
        if (isWatered && isFertilized && !isGrowing)
        {
            if (currentStage < growthStages.Count - 1)
            {
                StartCoroutine(GrowthSequenceRoutine());
            }
        }
    }

    private IEnumerator GrowthSequenceRoutine()
    {
        isGrowing = true;

        mainCamera.FocusOnPoint(cameraFocusPoint.position, cameraZoomLevel, cameraMoveDuration);
        yield return new WaitForSeconds(cameraMoveDuration);

        yield return StartCoroutine(FadeRenderers(GetActiveVisuals(), 0f, fadeDuration));

        // Siapkan stage berikutnya, termasuk SEMUA daun dari tahap sebelumnya
        GoToNextStage();

        mainCamera.TriggerShake(shakeDuration, shakeMagnitude);
        yield return new WaitForSeconds(shakeDuration);

        // Munculkan Tangkai
        yield return StartCoroutine(FadeSingleRenderer(stemRenderer, 1f, fadeDuration));
        yield return new WaitForSeconds(0.2f);

        // Munculkan SEMUA daun yang seharusnya aktif
        yield return StartCoroutine(FadeRenderers(GetActiveLeaves(), 1f, leafAppearDuration));
        
        yield return new WaitForSeconds(1f);

        mainCamera.ReturnToPlayerFocus(cameraMoveDuration);
        isGrowing = false;
    }

    private void GoToNextStage()
    {
        if (currentStage >= growthStages.Count - 1) return;

        currentStage++;
        UpdateVisualsToPrepare();
        isWatered = false;
        isFertilized = false;
    }
    
    // --- PERUBAHAN UTAMA ADA DI SINI ---
    private void UpdateVisualsToPrepare()
    {
        GrowthStageData stageData = growthStages[currentStage];

        // Ganti sprite tangkai dan buat transparan
        stemRenderer.gameObject.SetActive(true);
        if (stageData.stemSprite != null) stemRenderer.sprite = stageData.stemSprite;
        stemRenderer.color = new Color(stemRenderer.color.r, stemRenderer.color.g, stemRenderer.color.b, 0);

        // Nonaktifkan semua daun terlebih dahulu untuk memulai dari awal
        foreach(var leaf in allLeaves)
        {
            leaf.SetActive(false);
        }

        // Aktifkan SEMUA daun dari tahap pertama hingga tahap saat ini
        for (int i = 0; i <= currentStage; i++)
        {
            GrowthStageData pastStageData = growthStages[i];
            foreach (var leaf in pastStageData.leavesToActivate)
            {
                leaf.SetActive(true);
                leaf.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); // Buat transparan
            }
        }
        Debug.Log("Menyiapkan tanaman untuk tumbuh ke tahap: " + stageData.stageName);
    }
    
    // --- Kumpulan Fungsi Bantuan (Tidak Berubah) ---

    private List<SpriteRenderer> GetActiveVisuals()
    {
        List<SpriteRenderer> visuals = new List<SpriteRenderer>();
        if (stemRenderer.gameObject.activeSelf) visuals.Add(stemRenderer);
        foreach(var leaf in allLeaves)
        {
            if (leaf.activeSelf) visuals.Add(leaf.GetComponent<SpriteRenderer>());
        }
        return visuals;
    }

    private List<GameObject> GetActiveLeaves()
    {
        List<GameObject> activeLeaves = new List<GameObject>();
        foreach (var leaf in allLeaves)
        {
            if (leaf.activeSelf)
            {
                activeLeaves.Add(leaf);
            }
        }
        return activeLeaves;
    }

    // ... (Coroutine FadeRenderers, FadeSingleRenderer, dll. tetap sama) ...
    private IEnumerator FadeSingleRenderer(SpriteRenderer renderer, float targetAlpha, float duration)
    {
        yield return StartCoroutine(FadeRenderers(new List<SpriteRenderer>{ renderer }, targetAlpha, duration));
    }

    private IEnumerator FadeRenderers(List<GameObject> objects, float targetAlpha, float duration)
    {
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        foreach (var obj in objects)
        {
            if (obj != null) renderers.Add(obj.GetComponent<SpriteRenderer>());
        }
        yield return StartCoroutine(FadeRenderers(renderers, targetAlpha, duration));
    }

    private IEnumerator FadeRenderers(List<SpriteRenderer> renderers, float targetAlpha, float duration)
    {
        float timer = 0f;
        List<Color> startColors = new List<Color>();
        foreach (var r in renderers) if (r != null) startColors.Add(r.color);

        while (timer < duration)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i] == null) continue;
                float newAlpha = Mathf.Lerp(startColors[i].a, targetAlpha, timer / duration);
                renderers[i].color = new Color(startColors[i].r, startColors[i].g, startColors[i].b, newAlpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] == null) continue;
            renderers[i].color = new Color(startColors[i].r, startColors[i].g, startColors[i].b, targetAlpha);
        }
    }
}