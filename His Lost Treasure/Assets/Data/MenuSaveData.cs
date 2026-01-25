using UnityEngine;
using System;

[Serializable]
public class MenuSaveData
{
    [Header("Audio")]
    [Range(0.0001f, 1f)] public float masterVolume = 1f;
    [Range(0.0001f, 1f)] public float musicVolume = 1f;
    [Range(0.0001f, 1f)] public float sfxVolume = 1f;

    [Header("Controls")]
    [Range(0.1f, 20f)] public float mouseSensitivity = 4f;
    public bool invertY = false;

    [Header("Video")]
    [Range(0.5f, 2f)] public float brightness = 1f;
    public int qualityLevel = 2; // Matches QualitySettings.GetQualityLevel()
    public bool isFullscreen = true;

    // Store actual values rather than just an index for better stability
    public int resWidth = 1920;
    public int resHeight = 1080;
    public int refreshRate = 60;
    public int resolutionIndex;

    /// <summary>
    /// Converts 0-1 volume to Decibels for Unity AudioMixer
    /// Usage: mixer.SetFloat("MasterVol", data.GetPercentageToDb(data.masterVolume));
    /// </summary>
    public float GetPercentageToDb(float volume)
    {
        return Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20f;
    }

    /// <summary>
    /// Applies the saved video settings directly to the engine
    /// </summary>
    public void ApplyVideoSettings()
    {
        QualitySettings.SetQualityLevel(qualityLevel);
        Screen.SetResolution(
            resWidth,
            resHeight,
            isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
            new RefreshRate { numerator = (uint)refreshRate, denominator = 1 }
        );
    }
}
