using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main controller for the MainGame scene. 
/// Manages transitions between ProfileSelection, Playing, and Results panels.
/// Loads microgame scenes ADDITIVELY (each microgame = separate scene).
/// MainGame scene stays active so HUD and UI remain visible.
/// </summary>
public class MainGameController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject profileSelectionPanel;
    [SerializeField] private GameObject microgamePanel;

    [Header("UI Controllers")]
    [SerializeField] private ProfileSelectionUI profileSelectionUI;
    [SerializeField] private MicrogameHUD microgameHUD;

    [Header("Transition")]
    [SerializeField] private float delayBetweenGames = 2.0f; // seconds between microgames

    // Track which microgame scene is currently loaded
    private string currentMicrogameSceneName = "";
    private MicrogameBase currentMicrogame;
    private List<MicrogameResult> sessionResults = new List<MicrogameResult>();

    // ═══════════════════════════════════════════
    //  UNITY LIFECYCLE
    // ═══════════════════════════════════════════

    private void Start()
    {
        // Subscribe to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged += HandleStateChanged;
            GameManager.Instance.OnMicrogameStarted += HandleMicrogameStarted;
            GameManager.Instance.OnMicrogameCompleted += HandleMicrogameCompleted;
            GameManager.Instance.OnAllMicrogamesCompleted += HandleAllMicrogamesCompleted;

            // Start in profile selection
            GameManager.Instance.EnterProfileSelection();
        }
        else
        {
            Debug.LogError("[MainGameController] GameManager.Instance is null! Make sure GameManager exists in the MainMenu scene.");
            // Fallback: show profile selection
            ShowPanel(profileSelectionPanel);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= HandleStateChanged;
            GameManager.Instance.OnMicrogameStarted -= HandleMicrogameStarted;
            GameManager.Instance.OnMicrogameCompleted -= HandleMicrogameCompleted;
            GameManager.Instance.OnAllMicrogamesCompleted -= HandleAllMicrogamesCompleted;
        }
    }

    // ═══════════════════════════════════════════
    //  STATE HANDLERS
    // ═══════════════════════════════════════════

    private void HandleStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.ProfileSelection:
                ShowPanel(profileSelectionPanel);
                StartCoroutine(UnloadCurrentMicrogameScene());
                break;

            case GameManager.GameState.Playing:
                ShowPanel(microgamePanel);
                sessionResults.Clear();
                break;

            // Results state is handled by a separate Results scene
            // GameManager loads it automatically
        }
    }

    private void HandleMicrogameStarted(int index)
    {
        Debug.Log($"[MainGameController] Starting microgame index: {index}");
        StartCoroutine(LoadMicrogameScene(index));
    }

    private void HandleMicrogameCompleted(MicrogameResult result)
    {
        sessionResults.Add(result);
        Debug.Log($"[MainGameController] Microgame completed: {result.microgameID} - Success: {result.success}");
    }

    private void HandleAllMicrogamesCompleted(List<MicrogameResult> results)
    {
        Debug.Log("[MainGameController] All microgames completed! Transitioning to Results scene.");
        // Unload the last microgame scene before GameManager loads Results scene
        StartCoroutine(UnloadCurrentMicrogameScene());
    }

    // ═══════════════════════════════════════════
    //  MICROGAME SCENE LOADING (ADDITIVE)
    // ═══════════════════════════════════════════

    /// <summary>
    /// Unloads the previous microgame scene, then loads the next one additively.
    /// </summary>
    private IEnumerator LoadMicrogameScene(int index)
    {
        // 1) Unload previous microgame scene if any
        yield return StartCoroutine(UnloadCurrentMicrogameScene());

        // 2) Small delay for transition
        yield return new WaitForSeconds(0.3f);

        // 3) Get the scene name from GameManager
        string microgameID = GameManager.Instance.GetCurrentMicrogameID();

        // The scene name matches the microgameID (e.g. "MG_FixEngine")
        string sceneName = microgameID;

        // 4) Check if the scene exists in Build Settings
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            // Load the microgame scene additively
            AsyncOperation loadOp = SceneController.LoadSceneAdditive(sceneName);
            yield return loadOp;

            currentMicrogameSceneName = sceneName;

            // 5) Find the MicrogameBase component in the newly loaded scene
            yield return null; // Wait one frame for scene objects to initialize

            currentMicrogame = FindMicrogameInScene(sceneName);

            if (currentMicrogame != null)
            {
                // Update HUD
                var profile = ProfileDatabase.GetProfile(GameManager.Instance.SelectedProfile);
                if (microgameHUD != null)
                {
                    microgameHUD.BindToMicrogame(currentMicrogame);
                    microgameHUD.SetProgress(index, GameManager.Instance.GetTotalMicrogames(),
                        profile != null ? profile.nameDutch : "");
                }

                // Start the microgame!
                currentMicrogame.StartMicrogame();
            }
            else
            {
                Debug.LogError($"[MainGameController] No MicrogameBase found in scene: {sceneName}");
                GameManager.Instance.CompleteMicrogame(false, 0f, 0);
            }
        }
        else
        {
            Debug.LogWarning($"[MainGameController] Scene '{sceneName}' not found in Build Settings! Skipping...");
            // Auto-complete so the game doesn't get stuck
            GameManager.Instance.CompleteMicrogame(true, 5f, 50);
        }
    }

    /// <summary>
    /// Finds the MicrogameBase component in a loaded scene.
    /// </summary>
    private MicrogameBase FindMicrogameInScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded) return null;

        // Search all root GameObjects in the microgame scene
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            MicrogameBase mg = obj.GetComponentInChildren<MicrogameBase>();
            if (mg != null) return mg;
        }
        return null;
    }

    /// <summary>
    /// Unloads the currently loaded microgame scene.
    /// </summary>
    private IEnumerator UnloadCurrentMicrogameScene()
    {
        if (!string.IsNullOrEmpty(currentMicrogameSceneName) 
            && SceneController.IsSceneLoaded(currentMicrogameSceneName))
        {
            AsyncOperation unloadOp = SceneController.UnloadScene(currentMicrogameSceneName);
            yield return unloadOp;

            Debug.Log($"[MainGameController] Unloaded scene: {currentMicrogameSceneName}");
        }

        currentMicrogameSceneName = "";
        currentMicrogame = null;
    }


    // ═══════════════════════════════════════════
    //  PANEL MANAGEMENT
    // ═══════════════════════════════════════════

    private void ShowPanel(GameObject panelToShow)
    {
        if (profileSelectionPanel != null)
            profileSelectionPanel.SetActive(panelToShow == profileSelectionPanel);

        if (microgamePanel != null)
            microgamePanel.SetActive(panelToShow == microgamePanel);
    }
}
