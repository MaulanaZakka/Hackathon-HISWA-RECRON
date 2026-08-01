using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main controller for the MainGame scene. 
/// Manages transitions between ProfileSelection, Playing, and Results panels.
/// Instantiates microgame prefabs and passes them to the HUD.
/// </summary>
public class MainGameController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject profileSelectionPanel;
    [SerializeField] private GameObject microgamePanel;
    [SerializeField] private GameObject resultsPanel;

    [Header("UI Controllers")]
    [SerializeField] private ProfileSelectionUI profileSelectionUI;
    [SerializeField] private MicrogameHUD microgameHUD;
    [SerializeField] private ResultsPanelUI resultsPanelUI;

    [Header("Microgame Spawn Point")]
    [SerializeField] private Transform microgameSpawnParent;

    [Header("Microgame Prefabs")]
    [SerializeField] private List<MicrogamePrefabEntry> microgamePrefabs;

    [Header("Transition")]
    [SerializeField] private float delayBetweenGames = 2.0f; // seconds between microgames

    private GameObject currentMicrogameInstance;
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
            Debug.LogError("[MainGameController] GameManager.Instance is null! Make sure GameManager exists in the MainMenu scene or is a prefab.");
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
                DestroyCurrentMicrogame();
                break;

            case GameManager.GameState.Playing:
                ShowPanel(microgamePanel);
                sessionResults.Clear();
                break;

            case GameManager.GameState.Results:
                ShowPanel(resultsPanel);
                break;
        }
    }

    private void HandleMicrogameStarted(int index)
    {
        Debug.Log($"[MainGameController] Starting microgame index: {index}");
        StartCoroutine(SpawnMicrogame(index));
    }

    private void HandleMicrogameCompleted(MicrogameResult result)
    {
        sessionResults.Add(result);
        Debug.Log($"[MainGameController] Microgame completed: {result.microgameID} - Success: {result.success}");
    }

    private void HandleAllMicrogamesCompleted(List<MicrogameResult> results)
    {
        Debug.Log($"[MainGameController] All microgames completed! Showing results.");

        // Clean up microgame
        StartCoroutine(TransitionToResults(results));
    }

    // ═══════════════════════════════════════════
    //  MICROGAME SPAWNING
    // ═══════════════════════════════════════════

    private IEnumerator SpawnMicrogame(int index)
    {
        // Destroy previous microgame if any
        DestroyCurrentMicrogame();

        // Small delay for transition
        yield return new WaitForSeconds(0.3f);

        string microgameID = GameManager.Instance.GetCurrentMicrogameID();
        GameObject prefab = FindMicrogamePrefab(microgameID);

        if (prefab == null)
        {
            Debug.LogWarning($"[MainGameController] No prefab found for microgame: {microgameID}. Using placeholder.");
            // TODO: Create a placeholder microgame
            // For now, skip to completion
            GameManager.Instance.CompleteMicrogame(true, 5f, 50);
            yield break;
        }

        // Spawn microgame
        currentMicrogameInstance = Instantiate(prefab, microgameSpawnParent);
        MicrogameBase microgame = currentMicrogameInstance.GetComponent<MicrogameBase>();

        if (microgame == null)
        {
            Debug.LogError($"[MainGameController] Prefab {microgameID} has no MicrogameBase component!");
            yield break;
        }

        // Update HUD
        var profile = ProfileDatabase.GetProfile(GameManager.Instance.SelectedProfile);
        if (microgameHUD != null)
        {
            microgameHUD.BindToMicrogame(microgame);
            microgameHUD.SetProgress(index, GameManager.Instance.GetTotalMicrogames(),
                profile != null ? profile.nameDutch : "");
        }

        // Start the microgame!
        microgame.StartMicrogame();
    }

    private IEnumerator TransitionToResults(List<MicrogameResult> results)
    {
        // Wait a moment before showing results
        yield return new WaitForSeconds(delayBetweenGames);

        DestroyCurrentMicrogame();

        // Show results panel
        if (resultsPanelUI != null)
        {
            resultsPanelUI.ShowResults(GameManager.Instance.SelectedProfile, results);
        }
    }

    private GameObject FindMicrogamePrefab(string microgameID)
    {
        if (microgamePrefabs == null) return null;

        foreach (var entry in microgamePrefabs)
        {
            if (entry.microgameID == microgameID)
                return entry.prefab;
        }
        return null;
    }

    private void DestroyCurrentMicrogame()
    {
        if (currentMicrogameInstance != null)
        {
            Destroy(currentMicrogameInstance);
            currentMicrogameInstance = null;
        }
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

        if (resultsPanel != null)
            resultsPanel.SetActive(panelToShow == resultsPanel);
    }
}

/// <summary>
/// Maps a microgame ID string to its prefab for the Inspector.
/// </summary>
[System.Serializable]
public class MicrogamePrefabEntry
{
    public string microgameID;
    public GameObject prefab;
}
