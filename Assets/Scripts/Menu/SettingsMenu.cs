using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.UI;


public class SettingsMenu : MonoBehaviour { 

    public Dropdown resolutionDropdown;

    public enum TypeSound
    {
        General,
        FX,
        Music
    }

    TypeSound actualTypeSound;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && // faut-il ajouter Screen.currentResolution.width ?
                resolutions[i].height == Screen.currentResolution.height) // faut-il ajouter Screen.currentResolution.height ?
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetTypeSound(string str)
    {
        switch(str)
        {
            case "General":
                actualTypeSound = TypeSound.General;
                break;
            case "FX":
                actualTypeSound = TypeSound.FX;
                break;
            case "Music":
                actualTypeSound = TypeSound.Music;
                break;
            default:
                actualTypeSound = TypeSound.General;
                break;
        }
    }

    public void SetVolume (float volume)
    {
        switch (actualTypeSound)
        {
            case (TypeSound.General):
                AkSoundEngine.SetRTPCValue("Var_Master_Audio", (int)volume);
                break;
            case (TypeSound.FX):
                AkSoundEngine.SetRTPCValue("Var_SFX_Volume", (int)volume);
                break;
            case (TypeSound.Music):
                AkSoundEngine.SetRTPCValue("Var_Music_Volume", (int)volume);
                break;
            default:
                break;
        }
    }
    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
