using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the Main Menu UI: Play, Settings, Quit buttons.
/// Attach this to the Canvas in the MainMenu scene.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button closeSettingsButton;  // Tombol "X" atau "Sluiten" di dalam SettingsPanel

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;

    private void Start()
    {
        // Make sure settings panel is hidden at start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Wire up button listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(OnCloseSettingsClicked);

        // Set title
        if (titleText != null)
            titleText.text = "Recreatie Rush";
    }

    private void OnPlayClicked()
    {
        Debug.Log("[MainMenuUI] Play clicked!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            // Fallback: load scene directly
            SceneController.LoadScene("MainGame");
        }
    }

    private void OnSettingsClicked()
    {
        Debug.Log("[MainMenuUI] Settings clicked!");
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void OnCloseSettingsClicked()
    {
        Debug.Log("[MainMenuUI] Close settings clicked!");
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuUI] Quit clicked!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayClicked);
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClicked);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.RemoveListener(OnCloseSettingsClicked);
    }
}

