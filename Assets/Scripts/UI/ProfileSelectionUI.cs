using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Displays the 6 LOBX profile cards for the player to choose from.
/// Each card shows the profile emoji, name, description, and number of jobs.
/// </summary>
public class ProfileSelectionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cardContainer;     // Parent for profile cards
    [SerializeField] private GameObject profileCardPrefab; // Prefab for a single profile card

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headerText;

    private List<GameObject> spawnedCards = new List<GameObject>();

    private void OnEnable()
    {
        PopulateProfiles();
    }

    /// <summary>
    /// Creates a button card for each of the 6 LOBX profiles.
    /// </summary>
    public void PopulateProfiles()
    {
        // Clear existing cards
        foreach (var card in spawnedCards)
        {
            if (card != null) Destroy(card);
        }
        spawnedCards.Clear();

        if (headerText != null)
            headerText.text = "Kies jouw type!";

        var allProfiles = ProfileDatabase.GetAllProfiles();
        foreach (var profile in allProfiles)
        {
            CreateProfileCard(profile);
        }
    }

    private void CreateProfileCard(ProfileDatabase.ProfileInfo profile)
    {
        if (profileCardPrefab == null || cardContainer == null)
        {
            Debug.LogWarning("[ProfileSelectionUI] Missing prefab or container reference!");
            return;
        }

        GameObject card = Instantiate(profileCardPrefab, cardContainer);
        spawnedCards.Add(card);

        // Set up card visuals
        // Find child components by name (flexible for different prefab structures)
        var emojiText = card.transform.Find("EmojiText")?.GetComponent<TextMeshProUGUI>();
        var nameText = card.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        var descText = card.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
        var jobCountText = card.transform.Find("JobCountText")?.GetComponent<TextMeshProUGUI>();
        var cardButton = card.GetComponent<Button>();
        var cardImage = card.GetComponent<Image>();

        if (emojiText != null)
            emojiText.text = profile.emoji;

        if (nameText != null)
            nameText.text = profile.nameDutch;

        if (descText != null)
            descText.text = profile.description;

        if (jobCountText != null)
            jobCountText.text = $"{profile.jobs.Count} beroep{(profile.jobs.Count > 1 ? "en" : "")}";

        if (cardImage != null)
        {
            Color bgColor = profile.themeColor;
            bgColor.a = 0.85f;
            cardImage.color = bgColor;
        }

        // Button click → select this profile
        if (cardButton != null)
        {
            ProfileType profileType = profile.type; // capture for closure
            cardButton.onClick.AddListener(() => OnProfileCardClicked(profileType));
        }
    }

    private void OnProfileCardClicked(ProfileType profileType)
    {
        Debug.Log($"[ProfileSelectionUI] Selected profile: {profileType}");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectProfile(profileType);
        }
    }
}
