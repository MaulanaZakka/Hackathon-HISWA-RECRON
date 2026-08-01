using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

/// <summary>
/// Central game manager (Singleton). Controls the game state machine:
/// ProfileSelection → Playing microgames → Results panel.
/// Persists across scenes via DontDestroyOnLoad.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ── Game States ──
    public enum GameState
    {
        MainMenu,
        ProfileSelection,
        Playing,
        Results
    }

    [Header("Current State")]
    [SerializeField] private GameState currentState = GameState.MainMenu;
    public GameState CurrentState => currentState;

    // ── Selected Profile ──
    private ProfileType selectedProfile;
    public ProfileType SelectedProfile => selectedProfile;

    // ── Microgame Progress ──
    private int currentMicrogameIndex = 0;
    private List<MicrogameResult> microgameResults = new List<MicrogameResult>();

    // ── Events ──
    public event Action<GameState> OnStateChanged;
    public event Action<ProfileType> OnProfileSelected;
    public event Action<int> OnMicrogameStarted;       // index
    public event Action<MicrogameResult> OnMicrogameCompleted;
    public event Action<List<MicrogameResult>> OnAllMicrogamesCompleted;

    // ── Profile Data ──
    // Each profile has a list of microgame scene/prefab identifiers
    private Dictionary<ProfileType, List<string>> profileMicrogames;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeProfileMicrogames();
    }

    /// <summary>
    /// Maps each LOBX profile to its microgame identifiers.
    /// </summary>
    private void InitializeProfileMicrogames()
    {
        profileMicrogames = new Dictionary<ProfileType, List<string>>
        {
            {
                ProfileType.DeDoener, new List<string>
                {
                    "MG_FixEngine",       // Monteur Motoren
                    "MG_ConnectCables",   // Elektromonteur
                    "MG_FixPipes"         // Technische Dienst
                }
            },
            {
                ProfileType.DeOrganisator, new List<string>
                {
                    "MG_ManageHarbor",    // Havenmeester
                    "MG_CheckInGuest"     // Front Office & Shop
                }
            },
            {
                ProfileType.DeCreatieveling, new List<string>
                {
                    "MG_EntertainGuests"  // Animatiemedewerker
                }
            },
            {
                ProfileType.DeHelper, new List<string>
                {
                    "MG_TeachSailing",    // Zeil Instructeur
                    "MG_GuideOutbound"    // Sport & Activiteiten
                }
            },
            {
                ProfileType.DeOndernemer, new List<string>
                {
                    "MG_BeautifyResort"   // Groenvoorziening
                }
            },
            {
                ProfileType.DeOnderzoeker, new List<string>
                {
                    "MG_SewSail"          // Zeilmaker
                }
            }
        };
    }

    // ═══════════════════════════════════════════
    //  PUBLIC API — Called by UI scripts
    // ═══════════════════════════════════════════

    /// <summary>
    /// Transition from MainMenu to MainGame scene.
    /// </summary>
    public void StartGame()
    {
        SceneController.LoadScene("MainGame");
    }

    /// <summary>
    /// Called when the MainGame scene is loaded. Shows profile selection.
    /// </summary>
    public void EnterProfileSelection()
    {
        SetState(GameState.ProfileSelection);
    }

    /// <summary>
    /// Player picks a LOBX profile. Start playing its microgames.
    /// </summary>
    public void SelectProfile(ProfileType profile)
    {
        selectedProfile = profile;
        currentMicrogameIndex = 0;
        microgameResults.Clear();

        OnProfileSelected?.Invoke(profile);
        SetState(GameState.Playing);

        // Start the first microgame
        OnMicrogameStarted?.Invoke(currentMicrogameIndex);
    }

    /// <summary>
    /// Returns the microgame IDs for the currently selected profile.
    /// </summary>
    public List<string> GetCurrentMicrogameList()
    {
        if (profileMicrogames.ContainsKey(selectedProfile))
            return profileMicrogames[selectedProfile];
        return new List<string>();
    }

    /// <summary>
    /// Returns the total number of microgames for the selected profile.
    /// </summary>
    public int GetTotalMicrogames()
    {
        return GetCurrentMicrogameList().Count;
    }

    /// <summary>
    /// Returns the current microgame ID.
    /// </summary>
    public string GetCurrentMicrogameID()
    {
        var list = GetCurrentMicrogameList();
        if (currentMicrogameIndex < list.Count)
            return list[currentMicrogameIndex];
        return "";
    }

    /// <summary>
    /// Called by a microgame when completed. Advances to the next game or results.
    /// </summary>
    public void CompleteMicrogame(bool success, float timeRemaining, int score)
    {
        var result = new MicrogameResult
        {
            microgameID = GetCurrentMicrogameID(),
            profileType = selectedProfile,
            success = success,
            timeRemaining = timeRemaining,
            score = score
        };
        microgameResults.Add(result);
        OnMicrogameCompleted?.Invoke(result);

        currentMicrogameIndex++;

        if (currentMicrogameIndex >= GetTotalMicrogames())
        {
            // All microgames for this profile are done → show results
            SetState(GameState.Results);
            OnAllMicrogamesCompleted?.Invoke(microgameResults);
        }
        else
        {
            // Next microgame
            OnMicrogameStarted?.Invoke(currentMicrogameIndex);
        }
    }

    /// <summary>
    /// Returns to profile selection (from Results panel).
    /// </summary>
    public void ReturnToProfileSelection()
    {
        currentMicrogameIndex = 0;
        microgameResults.Clear();
        SetState(GameState.ProfileSelection);
    }

    /// <summary>
    /// Returns to the main menu scene.
    /// </summary>
    public void ReturnToMainMenu()
    {
        currentMicrogameIndex = 0;
        microgameResults.Clear();
        SetState(GameState.MainMenu);
        SceneController.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ═══════════════════════════════════════════
    //  PRIVATE
    // ═══════════════════════════════════════════

    private void SetState(GameState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(newState);
        Debug.Log($"[GameManager] State changed to: {newState}");
    }
}

// ═══════════════════════════════════════════
//  Enums & Data Structures
// ═══════════════════════════════════════════

/// <summary>
/// The 6 LOBX personality profiles.
/// </summary>
public enum ProfileType
{
    DeDoener,           // The Doer
    DeOrganisator,      // The Organizer
    DeCreatieveling,    // The Creative
    DeHelper,           // The Helper
    DeOndernemer,       // The Entrepreneur
    DeOnderzoeker       // The Investigator
}

/// <summary>
/// Result data for a single microgame round.
/// </summary>
[System.Serializable]
public struct MicrogameResult
{
    public string microgameID;
    public ProfileType profileType;
    public bool success;
    public float timeRemaining;
    public int score;
}
