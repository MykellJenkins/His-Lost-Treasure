using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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

    float nextSaveTime;
    private MenuSaveData data;
    private Resolution[] resolutions;
    private bool isInitializing = false; // Prevents saving while loading initial values
    public int resolutionIndex;

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
        volumeSlider.SetValueWithoutNotify(data.masterVolume);
        volumeText.text = Mathf.RoundToInt(data.masterVolume * 100f) + "%";
        AudioListener.volume = data.masterVolume;

        // Sensitivity
        sensitivitySlider.SetValueWithoutNotify(data.mouseSensitivity);
        sensitivityText.text = data.mouseSensitivity.ToString("0.0");

        // Toggles
        invertYToggle.SetIsOnWithoutNotify(data.invertY);
        fullscreenToggle.SetIsOnWithoutNotify(data.isFullscreen);

        // Graphics
        brightnessSlider.SetValueWithoutNotify(data.brightness);
        brightnessText.text = data.brightness.ToString("0.0");

        qualityDropdown.SetValueWithoutNotify(data.qualityLevel);
        QualitySettings.SetQualityLevel(data.qualityLevel);

        // Resolution
        if (resolutions != null && data.resolutionIndex < resolutions.Length)
        {
            resolutionDropdown.SetValueWithoutNotify(data.resolutionIndex);
            SetResolution(data.resolutionIndex);
        }
    }

    // ================= UI CALLBACKS =================

    public void SetVolume(float value)
    {
        if (isInitializing) return;

        value = Mathf.Clamp01(value);
        data.masterVolume = value;
        AudioListener.volume = value;

        volumeText.text = Mathf.RoundToInt(value * 100f) + "%";
        AutoSave();
    }

    public void SetSensitivity(float value)
    {
        if (isInitializing) return;

        value = Mathf.Clamp01(value);
        data.mouseSensitivity = value;

        sensitivityText.text = Mathf.RoundToInt(value * 100f) + "%";
        AutoSave();
    }

    public void SetInvertY(bool value)
    {
        data.invertY = value;
        AutoSave();
    }

    public void SetBrightness(float value)
    {
        if (isInitializing) return;

        value = Mathf.Clamp01(value);
        data.brightness = value;

        brightnessText.text = Mathf.RoundToInt(value * 100f) + "%";

        data.brightness = value;
        brightnessText.text = value.ToString("0.0");
        // Typically handled via a Post-Processing Volume weight or Global Shader param
        AutoSave();
    }

    public void SetQuality(int index)
    {
        if (isInitializing) return;

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
        if (isInitializing) return;
        if (resolutions == null || index < 0 || index >= resolutions.Length) return;

        data.resolutionIndex = index;
        Resolution res = resolutions[index];

        Screen.SetResolution(
            res.width,
            res.height,
            data.isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed
        );

        AutoSave();
    }

    public void AutoSave()
    {
        if (isInitializing) return;
        if (Time.unscaledTime < nextSaveTime) return;

        nextSaveTime = Time.unscaledTime + 0.4f;
        SavePlayerData.Instance.SaveMenu(data);
    }

    // ================= RESOLUTIONS =================

    void SetupResolutions()
    {
        var uniqueRes = Screen.resolutions
            .Select(r => (r.width, r.height))
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

            // Auto-detect current resolution
            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                data.resolutionIndex = i;
            }
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