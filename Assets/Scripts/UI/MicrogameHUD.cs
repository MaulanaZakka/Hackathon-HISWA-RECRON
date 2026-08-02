using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD overlay shown during microgame play.
/// Displays: timer bar, instruction text, and microgame progress (e.g. "2/3").
/// </summary>
public class MicrogameHUD : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Image timerFillBar;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Instruction")]
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private CanvasGroup instructionGroup;  // For fading

    [Header("Progress")]
    [SerializeField] private TextMeshProUGUI progressText;  // e.g. "Taak 2/3"
    [SerializeField] private TextMeshProUGUI profileNameText;

    [Header("Result Flash")]
    [SerializeField] private GameObject successFlash;       // "GOED!" flash
    [SerializeField] private GameObject failFlash;          // "HELAAS!" flash

    private MicrogameBase currentMicrogame;

    private void Awake()
    {
        // Hide flashes on start
        if (successFlash != null) successFlash.SetActive(false);
        if (failFlash != null) failFlash.SetActive(false);
    }

    /// <summary>
    /// Bind to a microgame's events.
    /// </summary>
    public void BindToMicrogame(MicrogameBase microgame)
    {
        // Unbind previous
        if (currentMicrogame != null)
        {
            currentMicrogame.OnTimerUpdated -= UpdateTimer;
            currentMicrogame.OnInstructionShow -= ShowInstruction;
            currentMicrogame.OnGameCompleted -= ShowResultFlash;
        }

        currentMicrogame = microgame;

        if (microgame != null)
        {
            microgame.OnTimerUpdated += UpdateTimer;
            microgame.OnInstructionShow += ShowInstruction;
            microgame.OnGameCompleted += ShowResultFlash;
        }

        // Reset visuals
        if (timerFillBar != null) timerFillBar.fillAmount = 1f;
        if (successFlash != null) successFlash.SetActive(false);
        if (failFlash != null) failFlash.SetActive(false);
    }

    /// <summary>
    /// Update the progress indicator (e.g. "Taak 2/3").
    /// </summary>
    public void SetProgress(int current, int total, string profileName)
    {
        if (progressText != null)
            progressText.text = $"Taak {current + 1}/{total}";

        if (profileNameText != null)
            profileNameText.text = profileName;
    }

    private void UpdateTimer(float remaining, float total)
    {
        if (timerFillBar != null)
            timerFillBar.fillAmount = remaining / total;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(remaining).ToString();

        // Change color as time runs out
        if (timerFillBar != null)
        {
            float ratio = remaining / total;
            if (ratio > 0.5f)
                timerFillBar.color = Color.green;
            else if (ratio > 0.25f)
                timerFillBar.color = Color.yellow;
            else
                timerFillBar.color = Color.red;
        }
    }

    private void ShowInstruction(string text)
    {
        if (instructionText != null)
            instructionText.text = text;

        if (instructionGroup != null)
        {
            instructionGroup.alpha = 1f;
            // Teks instruksi akan tetap tampil (tidak di-fade out)
            // sesuai permintaan agar pemain bisa terus membaca task-nya.
        }
    }

    private void ShowResultFlash(bool success, int score)
    {
        if (success && successFlash != null)
        {
            successFlash.SetActive(true);
        }
        else if (!success && failFlash != null)
        {
            failFlash.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (currentMicrogame != null)
        {
            currentMicrogame.OnTimerUpdated -= UpdateTimer;
            currentMicrogame.OnInstructionShow -= ShowInstruction;
            currentMicrogame.OnGameCompleted -= ShowResultFlash;
        }
    }
}
