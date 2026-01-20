using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;

    [Header("Gameplay")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sensitivityText;
    [SerializeField] private Toggle invertYToggle;

    [Header("Graphics")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Text brightnessText;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private MenuSaveData data;
    private Resolution[] resolutions;
    private bool isInitializing = false; // Prevents saving while loading initial values

    void Start()
    {
        isInitializing = true;
        try
        {
            LoadOrCreateData();
            SetupResolutions();
            ApplyAllToUI();
        }
        finally
        {
            isInitializing = false; // Guaranteed to run even if an error occurs above
        }
    }

    void LoadOrCreateData()
    {
        data = SavePlayerData.Instance.LoadMenu();
        if (data == null) data = new MenuSaveData();
    }

    void ApplyAllToUI()
    {
        // Audio
        volumeSlider.value = data.masterVolume;
        volumeText.text = (data.masterVolume * 100).ToString("0") + "%";
        AudioListener.volume = data.masterVolume;

        // Sensitivity
        sensitivitySlider.value = data.mouseSensitivity;
        sensitivityText.text = data.mouseSensitivity.ToString("0.0");

        // Toggles
        invertYToggle.isOn = data.invertY;
        fullscreenToggle.isOn = data.isFullscreen;

        // Graphics
        brightnessSlider.value = data.brightness;
        brightnessText.text = data.brightness.ToString("0.0");

        qualityDropdown.value = data.qualityLevel;
        QualitySettings.SetQualityLevel(data.qualityLevel);

        // Resolution
        resolutionDropdown.value = data.refreshRate;
        // Apply resolution safely
        if (resolutions != null && data.refreshRate < resolutions.Length)
        {
            SetResolution(data.refreshRate);
        }
    }

    // ================= UI CALLBACKS =================

    public void SetVolume(float value)
    {
        data.masterVolume = value;
        AudioListener.volume = value; // In 2026, use an AudioMixer for better results
        volumeText.text = (value * 100).ToString("0") + "%";
        AutoSave();
    }

    public void SetSensitivity(float value)
    {
        data.mouseSensitivity = value;
        sensitivityText.text = value.ToString("0.0");
        AutoSave();
    }

    public void SetInvertY(bool value)
    {
        data.invertY = value;
        AutoSave();
    }

    public void SetBrightness(float value)
    {
        data.brightness = value;
        brightnessText.text = value.ToString("0.0");
        // Typically handled via a Post-Processing Volume weight or Global Shader param
        AutoSave();
    }

    public void SetQuality(int index)
    {
        data.qualityLevel = index;
        QualitySettings.SetQualityLevel(index);
        AutoSave();
    }

    public void SetFullscreen(bool value)
    {
        data.isFullscreen = value;
        Screen.fullScreen = value;
        AutoSave();
    }

    public void SetResolution(int index)
    {
        if (resolutions == null || index < 0 || index >= resolutions.Length) return;

        data.refreshRate = index;
        Resolution res = resolutions[index];
        // Use the modern 2026 RefreshRate API if available
        Screen.SetResolution(res.width, res.height, data.isFullscreen);
        AutoSave();
    }

    public void AutoSave()
    {
        if (isInitializing) return;
        SavePlayerData.Instance.SaveMenu(data);
    }

    // ================= RESOLUTIONS =================

    void SetupResolutions()
    {
        // Fix: Filter by string representation to ensure actual distinct resolutions
        var uniqueRes = Screen.resolutions
            .Select(res => new { res.width, res.height })
            .Distinct()
            .ToList();

        resolutions = new Resolution[uniqueRes.Count];
        resolutionDropdown.ClearOptions();
        List<string> options = new();

        for (int i = 0; i < uniqueRes.Count; i++)
        {
            resolutions[i] = new Resolution
            {
                width = uniqueRes[i].width,
                height = uniqueRes[i].height
            };
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");
        }
        resolutionDropdown.AddOptions(options);
    }

    public void Apply()
    {
        // Ensure data is valid before saving
        if (data == null) LoadOrCreateData();

        // 2026 Standard: Explicitly set FullScreenMode
        FullScreenMode mode = data.isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        if (resolutions != null && data.refreshRate < resolutions.Length)
        {
            Resolution res = resolutions[data.refreshRate];
            Screen.SetResolution(res.width, res.height, mode);
        }

        SavePlayerData.Instance.SaveMenu(data);
        Debug.Log("Settings Applied and Saved Successfully.");
    }

    public void ResetToDefaults()
    {
        data = new MenuSaveData();
        ApplyAllToUI();
        SavePlayerData.Instance.SaveMenu(data);
    }
}