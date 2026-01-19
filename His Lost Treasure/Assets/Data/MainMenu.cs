using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    [Header("Volume Settings")]
    public TMP_Text volumeTextValue;
    public Slider volumeSlider;

    [Header("Gameplay Settings")]
    public TMP_Text mouseSenTextValue;
    public Slider mouseSenSlider;
    public Toggle invertYToggle;

    [Header("Graphics Settings")]
    public TMP_Text brightnessTextValue;
    public Slider brightnessSlider;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;
    private float mainMouseSen = 4f;

    Resolution[] resolutions;

    void Start()
    {
        // Setup resolution options
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currentResIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;

        LoadSettings();
    }

    // ===================== Settings Changes =====================
    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
        volumeTextValue.text = vol.ToString("0.0");
        SaveSettings();
    }

    public void SetMouseSen(float sen)
    {
        mainMouseSen = Mathf.RoundToInt(sen);
        mouseSenTextValue.text = mainMouseSen.ToString("0");
        SaveSettings();
    }

    public void SetInvertY(bool invert)
    {
        invertYToggle.isOn = invert;
        SaveSettings();
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
        SaveSettings();
    }

    public void SetFullScreen(bool isFullscreen)
    {
        _isFullScreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
        SaveSettings();
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        SaveSettings();
    }

    public void SetResolution(int resIndex)
    {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, _isFullScreen);
        resolutionDropdown.value = resIndex;
        SaveSettings();
    }

    // ===================== Save & Load =====================
    void SaveSettings()
    {
        MenuSaveData data = new MenuSaveData
        {
            volume = AudioListener.volume,
            playerMouseSensitivity = mainMouseSen,
            playerInvertY = invertYToggle.isOn,
            brightness = _brightnessLevel,
            quality = _qualityLevel,
            fullscreen = _isFullScreen,
            resolutionIndex = resolutionDropdown.value
        };

        SavePlayerData.Instance.SaveMenu(data);
    }

    void LoadSettings()
    {
        MenuSaveData data = SavePlayerData.Instance.LoadMenu();

        // Volume
        AudioListener.volume = data.volume;
        volumeSlider.value = data.volume;
        volumeTextValue.text = data.volume.ToString("0.0");

        // Mouse Sensitivity
        mainMouseSen = data.playerMouseSensitivity;
        mouseSenSlider.value = mainMouseSen;
        mouseSenTextValue.text = mainMouseSen.ToString("0");

        // Invert Y
        invertYToggle.isOn = data.playerInvertY;

        // Graphics
        _brightnessLevel = data.brightness;
        brightnessSlider.value = _brightnessLevel;
        brightnessTextValue.text = _brightnessLevel.ToString("0.0");

        _qualityLevel = data.quality;
        qualityDropdown.value = _qualityLevel;
        QualitySettings.SetQualityLevel(_qualityLevel);

        _isFullScreen = data.fullscreen;
        fullscreenToggle.isOn = _isFullScreen;
        Screen.fullScreen = _isFullScreen;

        resolutionDropdown.value = data.resolutionIndex;
        SetResolution(data.resolutionIndex);
    }
}
