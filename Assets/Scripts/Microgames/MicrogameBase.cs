using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Abstract base class for all microgames. 
/// Each microgame inherits from this and implements its own gameplay logic.
/// Handles: countdown timer, win/lose detection, and reporting results back to GameManager.
/// </summary>
public abstract class MicrogameBase : MonoBehaviour
{
    [Header("Microgame Settings")]
    [SerializeField] protected float timeLimit = 7f;   // seconds per microgame
    [SerializeField] protected string microgameID;
    [SerializeField] protected string instructionText;  // e.g. "PERBAIKI!" shown briefly

    // ── Runtime state ──
    protected float timeRemaining;
    protected bool isPlaying = false;
    protected bool isCompleted = false;
    protected int score = 0;

    // ── Events ──
    public event Action<float, float> OnTimerUpdated;     // (timeRemaining, timeLimit)
    public event Action<string> OnInstructionShow;
    public event Action<bool, int> OnGameCompleted;        // (success, score)

    /// <summary>
    /// Normalized time remaining (1.0 = full, 0.0 = expired).
    /// </summary>
    public float TimeRemainingNormalized => timeRemaining / timeLimit;

    // ═══════════════════════════════════════════
    //  LIFECYCLE
    // ═══════════════════════════════════════════

    /// <summary>
    /// Called to initialize and start the microgame.
    /// </summary>
    public virtual void StartMicrogame()
    {
        timeRemaining = timeLimit;
        isPlaying = false;
        isCompleted = false;
        score = 0;

        // Show instruction briefly, then start playing
        StartCoroutine(ShowInstructionThenPlay());
    }

    /// <summary>
    /// Shows the instruction text (e.g. "PERBAIKI!") for 1.5 seconds, 
    /// then starts the gameplay timer.
    /// </summary>
    private IEnumerator ShowInstructionThenPlay()
    {
        OnInstructionShow?.Invoke(instructionText);
        yield return new WaitForSeconds(1.5f);

        isPlaying = true;
        OnSetup();
    }

    protected virtual void Update()
    {
        if (!isPlaying || isCompleted) return;

        // Countdown
        timeRemaining -= Time.deltaTime;
        OnTimerUpdated?.Invoke(timeRemaining, timeLimit);

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            TimeExpired();
        }
    }

    // ═══════════════════════════════════════════
    //  ABSTRACT METHODS — Override in each microgame
    // ═══════════════════════════════════════════

    /// <summary>
    /// Called right when gameplay starts (after instruction fades).
    /// Set up game objects, spawn items, etc.
    /// </summary>
    protected abstract void OnSetup();

    /// <summary>
    /// Called when time runs out without completing the objective.
    /// Override to customize failure behavior.
    /// </summary>
    protected virtual void TimeExpired()
    {
        CompleteMicrogame(false);
    }

    // ═══════════════════════════════════════════
    //  PROTECTED HELPERS — Call from subclasses
    // ═══════════════════════════════════════════

    /// <summary>
    /// Call this when the player successfully completes the microgame objective.
    /// </summary>
    protected void WinMicrogame(int bonusScore = 100)
    {
        if (isCompleted) return;
        score += bonusScore;
        CompleteMicrogame(true);
    }

    /// <summary>
    /// Call this to immediately fail the microgame.
    /// </summary>
    protected void LoseMicrogame()
    {
        if (isCompleted) return;
        CompleteMicrogame(false);
    }

    /// <summary>
    /// Internal: finalize the microgame and report to GameManager.
    /// </summary>
    private void CompleteMicrogame(bool success)
    {
        isCompleted = true;
        isPlaying = false;

        // Bonus score for remaining time
        if (success)
        {
            score += Mathf.RoundToInt(timeRemaining * 10f);
        }

        OnGameCompleted?.Invoke(success, score);

        // Report to GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteMicrogame(success, timeRemaining, score);
        }

        Debug.Log($"[{microgameID}] Completed! Success: {success}, Score: {score}, Time left: {timeRemaining:F1}s");
    }

    /// <summary>
    /// Clean up when the microgame GameObject is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
    }
}
