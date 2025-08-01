using UnityEngine;
using System.Collections;
using System.Linq;

public enum GuideType { Move, Interact, Jump, DropDown, Aim, Camera }

[System.Serializable]
public class Guide
{
    public string guideName;
    public GuideType type;
    public GameObject guideObject;
    [HideInInspector]
    public CanvasGroup canvasGroup;
    [HideInInspector]
    public bool hasBeenShown = false;
}

public class GuideManager : MonoBehaviour
{
    public static GuideManager Instance;

    [SerializeField] private Guide[] guides;
    [SerializeField] private float defaultDisplayTime = 5f;
    [SerializeField] private float fadeDuration = 0.3f; // Kecepatan fade

    private Guide currentlyActiveGuide = null;
    private Coroutine displayCoroutine = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var guide in guides)
        {
            if (guide.guideObject != null)
            {
                guide.canvasGroup = guide.guideObject.GetComponent<CanvasGroup>();
                if (guide.canvasGroup == null)
                {
                    guide.canvasGroup = guide.guideObject.AddComponent<CanvasGroup>();
                }
                
                guide.canvasGroup.alpha = 0;
                guide.guideObject.SetActive(false);
            }
        }
    }

    public void ShowTimedGuide(GuideType type)
    {
        Guide guideToShow = FindGuide(type);
        if (guideToShow == null || guideToShow.hasBeenShown) return;

        if (currentlyActiveGuide != null && currentlyActiveGuide != guideToShow)
        {
            ForceHide(currentlyActiveGuide);
        }

        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(ShowAndHideRoutine(guideToShow));
        
        guideToShow.hasBeenShown = true;
    }

    public void ShowSituationalGuide(GuideType type)
    {
        Guide guideToShow = FindGuide(type);
        if (guideToShow == null || currentlyActiveGuide == guideToShow) return;

        if (currentlyActiveGuide != null)
        {
            ForceHide(currentlyActiveGuide);
        }
        
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(FadeGuideRoutine(guideToShow, true));
        currentlyActiveGuide = guideToShow;
    }

    public void HideSituationalGuide(GuideType type)
    {
        Guide guideToHide = FindGuide(type);
        if (guideToHide == null || currentlyActiveGuide != guideToHide) return;
        
        if (displayCoroutine != null) StopCoroutine(displayCoroutine);
        displayCoroutine = StartCoroutine(FadeGuideRoutine(guideToHide, false));
        currentlyActiveGuide = null;
    }

    private IEnumerator ShowAndHideRoutine(Guide guide)
    {
        currentlyActiveGuide = guide;
        yield return StartCoroutine(FadeGuideRoutine(guide, true));
        yield return new WaitForSeconds(defaultDisplayTime);
        yield return StartCoroutine(FadeGuideRoutine(guide, false));
        currentlyActiveGuide = null;
    }

    // --- PERBAIKAN ADA DI DALAM COROUTINE INI ---
    private IEnumerator FadeGuideRoutine(Guide guide, bool fadeIn)
    {
        if (fadeIn)
        {
            guide.guideObject.SetActive(true);
        }

        float startAlpha = guide.canvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;
        float timer = 0f;

        // Pastikan menggunakan nama variabel yang benar: "fadeDuration"
        while (timer < fadeDuration)
        {
            // Pastikan menggunakan nama variabel yang benar: "fadeDuration"
            guide.canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        guide.canvasGroup.alpha = endAlpha;

        if (!fadeIn)
        {
            guide.guideObject.SetActive(false);
        }
    }

    private void ForceHide(Guide guide)
    {
        if(displayCoroutine != null) StopCoroutine(displayCoroutine);
        if (guide != null && guide.guideObject != null)
        {
            guide.canvasGroup.alpha = 0;
            guide.guideObject.SetActive(false);
        }
        if(currentlyActiveGuide == guide)
        {
            currentlyActiveGuide = null;
        }
    }

    private Guide FindGuide(GuideType type) => guides.FirstOrDefault(g => g.type == type);
}