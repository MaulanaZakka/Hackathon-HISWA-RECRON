using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Handles scene loading/unloading. Static utility class.
/// Supports both normal scene loading and additive loading for microgames.
/// </summary>
public static class SceneController
{
    /// <summary>
    /// Load a scene by name (replaces current scene).
    /// Used for: MainMenu → MainGame transitions.
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        Debug.Log($"[SceneController] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a scene ADDITIVELY (on top of the current scene).
    /// Used for: Loading microgame scenes while MainGame stays active.
    /// The HUD and UI panels remain visible.
    /// </summary>
    public static AsyncOperation LoadSceneAdditive(string sceneName)
    {
        Debug.Log($"[SceneController] Loading scene additive: {sceneName}");
        return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    /// <summary>
    /// Unload an additively loaded scene.
    /// Used for: Removing a microgame scene when it's done.
    /// </summary>
    public static AsyncOperation UnloadScene(string sceneName)
    {
        Debug.Log($"[SceneController] Unloading scene: {sceneName}");
        return SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// Check if a scene is currently loaded.
    /// </summary>
    public static bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }

    /// <summary>
    /// Get the name of the currently active scene.
    /// </summary>
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
