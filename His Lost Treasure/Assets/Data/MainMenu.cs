using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class MainMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer mixer;
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

        if (data.masterVolume <= 0f)
            data.masterVolume = 1f;
        Debug.Log("Loaded volume: " + data.masterVolume);
    }

    void ApplyAllToUI()
    {
        // Audio
        volumeSlider.SetValueWithoutNotify(data.masterVolume);
        volumeText.text = Mathf.RoundToInt(data.masterVolume * 100f) + "%";
        ApplyVolumeToMixer();

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

        // Convert linear [0,1] to decibels
        float dB = (value > 0f) ? 20f * Mathf.Log10(value) : -80f;

        mixer.SetFloat("Master", dB); // <-- Exposed parameter in AudioMixer

        // Update UI
        volumeText.text = Mathf.RoundToInt(value * 100f) + "%";

        AutoSave();
    }

    public void SetVolumeFromSlider()
    {
        SetVolume(volumeSlider.value);
    }

    // Apply saved volume to the mixer (instead of AudioListener)
    void ApplyVolumeToMixer()
    {
        float dB = (data.masterVolume > 0f) ? 20f * Mathf.Log10(data.masterVolume) : -80f;
        mixer.SetFloat("Master", dB);
    }

    public void SetSensitivity(float value)
    {
        if (isInitializing) return;

        value = Mathf.Clamp01(value);
        data.mouseSensitivity = value;

        sensitivityText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void SetInvertY(bool value)
    {
        data.invertY = value;
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
    }

    public void SetQuality(int index)
    {
        if (isInitializing) return;

        data.qualityLevel = index;
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool value)
    {
        data.isFullscreen = value;
        Screen.fullScreen = value;
        
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
        if (data == null)
            LoadOrCreateData();

        // ================= AUDIO =================
        volumeSlider.SetValueWithoutNotify(data.masterVolume);
        AudioListener.volume = data.masterVolume;

        // ================= GAMEPLAY =================
        data.mouseSensitivity = Mathf.Clamp(data.mouseSensitivity, 0.1f, 20f);
        // invertY is already stored

        // ================= GRAPHICS =================
        QualitySettings.SetQualityLevel(data.qualityLevel);

        FullScreenMode mode = data.isFullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        if (resolutions != null &&
            data.resolutionIndex >= 0 &&
            data.resolutionIndex < resolutions.Length)
        {
            Resolution res = resolutions[data.resolutionIndex];
            Screen.SetResolution(res.width, res.height, mode);
        }

        // ================= BRIGHTNESS =================
        ApplyBrightness(); // IMPORTANT

        // ================= SAVE (NO THROTTLE) =================
        SavePlayerData.Instance.SaveMenu(data);

        Debug.Log("All settings applied and saved.");
    }

    void ApplyBrightness()
    {
    // Replace with Post-Processing if using URP/HDRP
    Shader.SetGlobalFloat("_GlobalBrightness", data.brightness);
    }

    public void ResetToDefaults()
    {
        data = new MenuSaveData();
        ApplyAllToUI();
        SavePlayerData.Instance.SaveMenu(data);
    }
}