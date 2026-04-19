using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Level")]
    public int levelNumber;
    public string sceneName;

    [Header("Button")]
    public Button levelButton;

    [Header("Stars")]
    public Image[] stars;
    public Sprite activeStarSprite;
    public Sprite inactiveStarSprite;

    [Header("Lock")]
    public GameObject lockIcon;

    [Header("Debug")]
    public int currentStarCount;
    public int previousLevelStarCount;
    public bool unlocked;

    private bool isLoading = false;

    private void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        UpdateStars();
        UpdateLockState();
    }

    public void LoadLevel()
    {
        if (isLoading) return;
        if (!IsUnlocked()) return;

        isLoading = true;

        if (levelButton != null)
            levelButton.interactable = false;

        Time.timeScale = 1f;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadSceneWithTransition(sceneName);
        else
            SceneManager.LoadScene(sceneName);
    }

    public void UpdateStars()
    {
        currentStarCount = PlayerPrefs.GetInt(GetStarsKey(levelNumber), 0);

        Debug.Log("Level " + levelNumber + " current stars = " + currentStarCount);

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].sprite = (i < currentStarCount) ? activeStarSprite : inactiveStarSprite;
        }
    }

    public void UpdateLockState()
    {
        unlocked = IsUnlocked();

        if (levelButton != null)
            levelButton.interactable = unlocked && !isLoading;

        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);
    }

    public bool IsUnlocked()
    {
        if (levelNumber == 1)
        {
            previousLevelStarCount = -1;
            return true;
        }

        previousLevelStarCount = PlayerPrefs.GetInt(GetStarsKey(levelNumber - 1), 0);
        return previousLevelStarCount >= 2;
    }

    private string GetStarsKey(int level)
    {
        return "Level" + level + "_stars";
    }
}