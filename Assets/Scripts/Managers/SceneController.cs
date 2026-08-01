using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene loading/unloading. Static utility class.
/// </summary>
public static class SceneController
{
    /// <summary>
    /// Load a scene by name (async for smooth transitions).
    /// </summary>
    public static void LoadScene(string sceneName)
    {
        Debug.Log($"[SceneController] Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Load a scene asynchronously (for loading screens if needed).
    /// </summary>
    public static AsyncOperation LoadSceneAsync(string sceneName)
    {
        Debug.Log($"[SceneController] Loading scene async: {sceneName}");
        return SceneManager.LoadSceneAsync(sceneName);
    }

    /// <summary>
    /// Get the name of the currently active scene.
    /// </summary>
    public static string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
