using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class PauseMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMainUI;
    [SerializeField] private GameObject howToPlayUI;

    [Header("UI Sound")]
    [SerializeField] private AudioClip buttonClickSfx;
    [SerializeField] private float uiSoundVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ShowPauseMain();
    }

    public void ResumeGame()
    {
        PlayUISound(buttonClickSfx);

        ShowPauseMain();

        if (LevelManager.Instance != null)
            LevelManager.Instance.ResumeGame();
    }

    public void OpenSettings()
    {
        PlayUISound(buttonClickSfx);

        if (pauseMainUI != null) pauseMainUI.SetActive(false);
        if (howToPlayUI != null) howToPlayUI.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        PlayUISound(buttonClickSfx);

        if (pauseMainUI != null) pauseMainUI.SetActive(false);
        if (howToPlayUI != null) howToPlayUI.SetActive(true);
    }

    public void BackToPauseMain()
    {
        PlayUISound(buttonClickSfx);
        ShowPauseMain();
    }

    public void ReturnToMainMenu()
    {
        PlayUISound(buttonClickSfx);

        Time.timeScale = 1f;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.isPaused = false;
            LevelManager.Instance.timerRunning = true;
        }

        SceneManager.LoadScene(mainMenuScene);
    }

    private void ShowPauseMain()
    {
        if (pauseMainUI != null) pauseMainUI.SetActive(true);
        if (howToPlayUI != null) howToPlayUI.SetActive(false);
    }

    public void PlayUISound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip, uiSoundVolume);
    }
}