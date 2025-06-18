using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public Image musicIcon;  // Icon âm thanh
    public Image sfxIcon;    // Icon hiệu ứng âm thanh
    private bool isMusicOn = true;
    private bool isSFXOn = true;
    // Hàm được gọi khi ấn vào nút Setting

    private void Start()
    {
        AudioManager.Instance.PlayMusic("Intro");
        HideSettingPanel();    
    }
    public void ShowSettingPanel()
    {
        // Ẩn panel chính
        mainPanel.SetActive(false);
        // Hiện SettingPanel
        settingsPanel.SetActive(true);
    }

    // Hàm để quay lại màn hình chính
    public void HideSettingPanel()
    {
        // Hiện panel chính
        mainPanel.SetActive(true);
        // Ẩn SettingPanel
        settingsPanel.SetActive(false);
    }
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        AudioManager.Instance.ToggleMusic();
        UpdateMusicIcon();
    }
    public void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        AudioManager.Instance.ToggleSFX();
        UpdateSFXIcon();
    }
    private void UpdateMusicIcon()
    {
        Color iconColor = musicIcon.color;
        iconColor.a = isMusicOn ? 1f : 0.5f;  // Nếu tắt, icon bị mờ đi
        musicIcon.color = iconColor;
    }

    private void UpdateSFXIcon()
    {
        Color iconColor = sfxIcon.color;
        iconColor.a = isSFXOn ? 1f : 0.5f;  // Nếu tắt, icon bị mờ đi
        sfxIcon.color = iconColor;
    }
}
