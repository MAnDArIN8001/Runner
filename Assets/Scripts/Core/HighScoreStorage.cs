using UnityEngine;

/// <summary>
/// Persists best score via PlayerPrefs.
/// </summary>
public static class HighScoreStorage
{
    private const string Key = "Runner.HighScore";

    public static int Load() => PlayerPrefs.GetInt(Key, 0);

    public static void Save(int score)
    {
        PlayerPrefs.SetInt(Key, score);
        PlayerPrefs.Save();
    }
}
