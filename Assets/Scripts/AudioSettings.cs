using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private Image musicToogleImage;
    [SerializeField] private Image sfxToogleImage;
    [SerializeField] private Image vibroToogleImage;
    [SerializeField] private Sprite[] toogleSprites;

    private bool vibrateOn = true;
    private bool musicOn = true;
    private bool sfxOn = true;

    private void Start()
    {
        ToogleSettingsMenu(false);
    }

    public void ToogleSettingsMenu(bool isOpen)
    {
        settingsMenu.SetActive(isOpen);
        musicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        vibrateOn = PlayerPrefs.GetInt("Vibrate", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("Sfx", 1) == 1;

        UpdateSettings();
    }

    private void UpdateSettings()
    {
        musicToogleImage.sprite = musicOn ? toogleSprites[1] : toogleSprites[0];
        PlayerPrefs.SetInt("Music", musicOn ? 1 : 0);

        sfxToogleImage.sprite = sfxOn ? toogleSprites[1] : toogleSprites[0];
        PlayerPrefs.SetInt("Sfx", sfxOn ? 1 : 0);

        vibroToogleImage.sprite = vibrateOn ? toogleSprites[1] : toogleSprites[0];
        PlayerPrefs.SetInt("Vibrate", vibrateOn ? 1 : 0);

        PlayerPrefs.Save();

        if (!musicOn)
            SoundManager.Instance.TurnOffMusic();
        else
            SoundManager.Instance.TurnOnMusic();

        if (!sfxOn)
            SoundManager.Instance.TurnOffSfx();
        else
            SoundManager.Instance.TurnOnSfx();
    }

    public void ToogleMusic()
    {
        musicOn = !musicOn;
        UpdateSettings();
    }

    public void ToogleSfx()
    {
        sfxOn = !sfxOn;
        UpdateSettings();
    }

    public void ToogleVibro()
    {
        vibrateOn = !vibrateOn;
        if (vibrateOn)
            Handheld.Vibrate();
        UpdateSettings();
    }
}
