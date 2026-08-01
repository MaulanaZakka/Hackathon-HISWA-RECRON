using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Displays the results panel after all microgames in a profile are completed.
/// Shows: profile name, all jobs within that profile, their descriptions, 
/// and the player's scores. This is the "educational payload" — 
/// the moment where the player learns what HISWA-RECRON actually does.
/// </summary>
public class ResultsPanelUI : MonoBehaviour
{
    [Header("Profile Info")]
    [SerializeField] private TextMeshProUGUI profileNameText;
    [SerializeField] private TextMeshProUGUI profileEmojiText;
    [SerializeField] private TextMeshProUGUI profileDescriptionText;
    [SerializeField] private Image profileBanner;

    [Header("Job List")]
    [SerializeField] private Transform jobListContainer;
    [SerializeField] private GameObject jobCardPrefab;

    [Header("Score Summary")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI completionText;   // e.g. "3/3 taken voltooid!"

    [Header("Buttons")]
    [SerializeField] private Button backToProfilesButton;
    [SerializeField] private Button backToMenuButton;

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headerText;

    private List<GameObject> spawnedJobCards = new List<GameObject>();

    private void OnEnable()
    {
        if (backToProfilesButton != null)
            backToProfilesButton.onClick.AddListener(OnBackToProfiles);

        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(OnBackToMenu);
    }

    private void OnDisable()
    {
        if (backToProfilesButton != null)
            backToProfilesButton.onClick.RemoveListener(OnBackToProfiles);

        if (backToMenuButton != null)
            backToMenuButton.onClick.RemoveListener(OnBackToMenu);
    }

    /// <summary>
    /// Populate the results panel with data from the completed microgames.
    /// </summary>
    public void ShowResults(ProfileType profileType, List<MicrogameResult> results)
    {
        var profile = ProfileDatabase.GetProfile(profileType);
        if (profile == null) return;

        // Set header
        if (headerText != null)
            headerText.text = "Dit is jouw wereld!";

        // Profile info
        if (profileNameText != null)
            profileNameText.text = profile.nameDutch;

        if (profileEmojiText != null)
            profileEmojiText.text = profile.emoji;

        if (profileDescriptionText != null)
            profileDescriptionText.text = profile.description;

        if (profileBanner != null)
        {
            Color bgColor = profile.themeColor;
            bgColor.a = 0.9f;
            profileBanner.color = bgColor;
        }

        // Score summary
        int totalScore = 0;
        int successes = 0;
        foreach (var r in results)
        {
            totalScore += r.score;
            if (r.success) successes++;
        }

        if (totalScoreText != null)
            totalScoreText.text = $"Score: {totalScore}";

        if (completionText != null)
            completionText.text = $"{successes}/{results.Count} taken voltooid!";

        // Job cards
        PopulateJobCards(profile, results);
    }

    private void PopulateJobCards(ProfileDatabase.ProfileInfo profile, List<MicrogameResult> results)
    {
        // Clear existing
        foreach (var card in spawnedJobCards)
        {
            if (card != null) Destroy(card);
        }
        spawnedJobCards.Clear();

        if (jobCardPrefab == null || jobListContainer == null) return;

        foreach (var job in profile.jobs)
        {
            GameObject card = Instantiate(jobCardPrefab, jobListContainer);
            spawnedJobCards.Add(card);

            // Find components
            var titleText = card.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            var sectorText = card.transform.Find("SectorText")?.GetComponent<TextMeshProUGUI>();
            var descText = card.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
            var scoreText = card.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            var statusIcon = card.transform.Find("StatusIcon")?.GetComponent<Image>();

            if (titleText != null)
                titleText.text = job.jobTitleDutch;

            if (sectorText != null)
                sectorText.text = job.sector;

            if (descText != null)
                descText.text = job.shortDescription;

            // Find matching result
            MicrogameResult? matchingResult = null;
            foreach (var r in results)
            {
                if (r.microgameID == job.microgameID)
                {
                    matchingResult = r;
                    break;
                }
            }

            if (matchingResult.HasValue)
            {
                if (scoreText != null)
                    scoreText.text = $"Score: {matchingResult.Value.score}";

                if (statusIcon != null)
                    statusIcon.color = matchingResult.Value.success ? Color.green : Color.red;
            }
        }
    }

    private void OnBackToProfiles()
    {
        Debug.Log("[ResultsPanelUI] Back to profiles");
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToProfileSelection();
    }

    private void OnBackToMenu()
    {
        Debug.Log("[ResultsPanelUI] Back to main menu");
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenu();
    }
}
