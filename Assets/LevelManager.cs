using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static System.Action OnCollectiblesChanged;
    public static LevelManager Instance;

    [Header("Level")]
    public int levelNumber = 1;

    [Header("Collectibles")]
    public int coinsCollected;
    public int cashCollected;
    public int gemsCollected;

    [Header("Values")]
    public int coinValue = 1;
    public int cashValue = 5;
    public int gemValue = 10;

    [Header("Panel Animation")]
    public float panelFadeDuration = 0.45f;
    public float panelMoveDuration = 0.45f;
    public float panelStartOffsetY = 80f;
    public float failedPanelDelay = 0.35f;

    private CanvasGroup completeCanvasGroup;
    private CanvasGroup failedCanvasGroup;
    private RectTransform completeRect;
    private RectTransform failedRect;
    private Vector2 completeTargetPos;
    private Vector2 failedTargetPos;

    [Header("Time")]
    public float levelTimer;
    public float maxLevelTime = 300f;
    public bool timerRunning = true;

    [Header("State")]
    public bool isPaused;
    public bool missionEnded;
    public bool missionSuccess;

    [Header("Stars")]
    public int earnedStars;
    public int oneStarScore = 12;
    public int twoStarScore = 22;
    public int threeStarScore = 32;

    [Header("UI")]
    public GameObject pauseBlurPanel;
    public GameObject pauseOverlay;
    public GameObject pauseMenu;
    public GameObject missionCompletePanel;
    public GameObject missionFailedPanel;
    public FailOverlayFX failOverlayFX;
    public PauseBlurController pauseBlurController;

    [Header("Player Control")]
    public MonoBehaviour[] playerMovementScripts;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (pauseBlurPanel != null) pauseBlurPanel.SetActive(false);
        if (pauseOverlay != null) pauseOverlay.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        SetupPanel(missionCompletePanel, out completeCanvasGroup, out completeRect, out completeTargetPos);
        SetupPanel(missionFailedPanel, out failedCanvasGroup, out failedRect, out failedTargetPos);
    }

    private void SetupPanel(GameObject panel, out CanvasGroup canvasGroup, out RectTransform rect, out Vector2 targetPos)
    {
        canvasGroup = null;
        rect = null;
        targetPos = Vector2.zero;

        if (panel == null) return;

        canvasGroup = panel.GetComponent<CanvasGroup>();
        rect = panel.GetComponent<RectTransform>();

        if (rect != null)
            targetPos = rect.anchoredPosition;

        panel.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    private void Update()
    {
        if (!missionEnded && !isPaused && timerRunning)
        {
            levelTimer += Time.unscaledDeltaTime;

            if (levelTimer >= maxLevelTime)
                FailMission("Time ran out");
        }

        if (!missionEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void CollectCoin()
    {
        if (missionEnded) return;
        coinsCollected++;
        OnCollectiblesChanged?.Invoke();
    }

    public void CollectCash()
    {
        if (missionEnded) return;
        cashCollected++;
        OnCollectiblesChanged?.Invoke();
    }

    public void CollectGem()
    {
        if (missionEnded) return;
        gemsCollected++;
        OnCollectiblesChanged?.Invoke();
    }

    public int GetTimeBonus()
    {
        if (levelTimer <= 120f) return 10;
        if (levelTimer <= 180f) return 8;
        if (levelTimer <= 240f) return 6;
        if (levelTimer <= 300f) return 4;
        return 0;
    }

    public int GetCollectibleScore()
    {
        return (coinsCollected * coinValue) +
               (cashCollected * cashValue) +
               (gemsCollected * gemValue);
    }

    public int GetTotalScore()
    {
        return GetCollectibleScore() + GetTimeBonus();
    }

    public int CalculateStars()
    {
        int score = GetTotalScore();

        if (score >= threeStarScore) return 3;
        if (score >= twoStarScore) return 2;
        if (score >= oneStarScore) return 1;
        return 0;
    }

    public void CompleteMission()
    {
        if (missionEnded) return;

        missionEnded = true;
        missionSuccess = true;
        timerRunning = false;
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseBlurPanel != null) pauseBlurPanel.SetActive(false);
        if (pauseOverlay != null) pauseOverlay.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        earnedStars = CalculateStars();
        SetPlayerControl(false);

        if (missionCompletePanel != null)
            StartCoroutine(AnimatePanelIn(missionCompletePanel, completeCanvasGroup, completeRect, completeTargetPos));

        SaveLevelProgress();
    }

    public void FailMission(string reason)
    {
        if (missionEnded) return;

        missionEnded = true;
        missionSuccess = false;
        timerRunning = false;
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseBlurPanel != null) pauseBlurPanel.SetActive(false);
        if (pauseOverlay != null) pauseOverlay.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        SetPlayerControl(false);

        if (failOverlayFX != null)
            failOverlayFX.PlayFailOverlay();

        StartCoroutine(ShowFailedPanelAfterDelay());
        Debug.Log("Mission Failed: " + reason);
    }

    private IEnumerator ShowFailedPanelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(failedPanelDelay);

        if (missionFailedPanel != null)
            StartCoroutine(AnimatePanelIn(missionFailedPanel, failedCanvasGroup, failedRect, failedTargetPos));
    }

    private IEnumerator AnimatePanelIn(GameObject panel, CanvasGroup canvasGroup, RectTransform rect, Vector2 targetPos)
    {
        if (panel == null) yield break;

        panel.SetActive(true);

        if (canvasGroup == null || rect == null)
            yield break;

        float time = 0f;
        float duration = Mathf.Max(panelFadeDuration, panelMoveDuration);

        Vector2 startPos = targetPos + new Vector2(0f, panelStartOffsetY);
        Vector3 startScale = new Vector3(0.96f, 0.96f, 1f);
        Vector3 endScale = Vector3.one;

        rect.anchoredPosition = startPos;
        rect.localScale = startScale;
        canvasGroup.alpha = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            float eased = EaseOutCubic(t);

            rect.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, eased);
            rect.localScale = Vector3.LerpUnclamped(startScale, endScale, eased);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, eased);

            yield return null;
        }

        rect.anchoredPosition = targetPos;
        rect.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private void SaveLevelProgress()
    {
        string prefix = "Level" + levelNumber;

        int totalScore = GetTotalScore();
        int bestStars = PlayerPrefs.GetInt(prefix + "_stars", 0);
        int bestScore = PlayerPrefs.GetInt(prefix + "_score", 0);
        float bestTime = PlayerPrefs.GetFloat(prefix + "_bestTime", float.MaxValue);

        if (earnedStars > bestStars)
            PlayerPrefs.SetInt(prefix + "_stars", earnedStars);

        if (totalScore > bestScore)
            PlayerPrefs.SetInt(prefix + "_score", totalScore);

        if (levelTimer < bestTime)
            PlayerPrefs.SetFloat(prefix + "_bestTime", levelTimer);

        PlayerPrefs.SetInt(prefix + "_coins", coinsCollected);
        PlayerPrefs.SetInt(prefix + "_cash", cashCollected);
        PlayerPrefs.SetInt(prefix + "_gems", gemsCollected);
        PlayerPrefs.SetInt(prefix + "_completed", 1);

        PlayerPrefs.Save();
    }

    public int GetCurrentLevelNumber()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string digits = "";

        foreach (char c in sceneName)
        {
            if (char.IsDigit(c))
                digits += c;
        }

        return string.IsNullOrEmpty(digits) ? 1 : int.Parse(digits);
    }

    public void PauseGame()
    {
        if (missionEnded) return;

        isPaused = true;
        timerRunning = false;

        if (pauseBlurController != null)
            pauseBlurController.CaptureAndShowBlur();

        if (pauseBlurPanel != null) pauseBlurPanel.SetActive(true);
        if (pauseOverlay != null) pauseOverlay.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        SetPlayerControl(false);
    }

    public void ResumeGame()
    {
        if (missionEnded) return;

        isPaused = false;
        timerRunning = true;
        Time.timeScale = 1f;

        if (pauseBlurPanel != null) pauseBlurPanel.SetActive(false);
        if (pauseOverlay != null) pauseOverlay.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);

        SetPlayerControl(true);
    }

    private void SetPlayerControl(bool enabledState)
    {
        if (playerMovementScripts == null) return;

        foreach (MonoBehaviour movementScript in playerMovementScripts)
        {
            if (movementScript != null)
                movementScript.enabled = enabledState;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithTransition(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithTransition("LevelSelect");
        else
            SceneManager.LoadScene("LevelSelect");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithTransition("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithTransition(current.buildIndex + 1);
        else
            SceneManager.LoadScene(current.buildIndex + 1);
    }
}