using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class InGameTopButtons : MonoBehaviour
{
    [Header("Optional")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private AudioClip buttonClickSfx;
    [SerializeField] private float uiSoundVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PauseButton()
    {
        PlayUISound();

        if (LevelManager.Instance != null && !LevelManager.Instance.isPaused)
            LevelManager.Instance.PauseGame();
    }

    public void RestartButton()
    {
        PlayUISound();

        if (LevelManager.Instance != null)
            LevelManager.Instance.RestartLevel();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void HintButton()
    {
        PlayUISound();

        if (hintPanel != null)
            hintPanel.SetActive(true);
    }

    public void CloseHintButton()
    {
        PlayUISound();

        if (hintPanel != null)
            hintPanel.SetActive(false);
    }

    private void PlayUISound()
    {
        if (audioSource != null && buttonClickSfx != null)
            audioSource.PlayOneShot(buttonClickSfx, uiSoundVolume);
    }
}
