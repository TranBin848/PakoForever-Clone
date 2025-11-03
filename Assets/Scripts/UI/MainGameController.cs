using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    public static MainGameController Instance { get; private set; }
    public GameObject inGamePanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject homePanel;
    public GameObject settingsPanel;
    public GameObject chooseCharacterPanel;
    public Image musicIconGame;  // Icon âm thanh
    public Image sfxIconGame;    // Icon hiệu ứng âm thanh
    public Image musicIconHome;  // Icon âm thanh
    public Image sfxIconHome;    // Icon hiệu ứng âm thanh
    private bool isMusicOn = true;
    private bool isSFXOn = true;
    private bool canRestart = false; // Biến kiểm tra có thể restart không
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        Time.timeScale = 0;
        if (PlayerPrefs.GetInt("isRestarting", 0) == 1) // Nếu đang restart
        {
            PlayerPrefs.SetInt("isRestarting", 0); // Reset lại trạng thái
            ShowInGamePanel(); // Vào game luôn
        }
        else
        {
            ShowHomePanel(); // Bình thường thì vào màn hình chính
        }
    }
    void Update()
    {
        if (GameManager.Instance.isGameover && !canRestart)
        {
            StartCoroutine(EnableRestartAfterDelay(3f)); // Chạy Coroutine chờ 3 giây
        }

        if (GameManager.Instance.isGameover && canRestart && Input.GetMouseButtonDown(0)) // Hoặc Input.touchCount > 0
        {
            PlayerPrefs.SetInt("isRestarting", 1);
            PlayerPrefs.SetInt("ZoomOut", 1);
            PlayerPrefs.Save();
            GameManager.Instance.RestartGame();
        }
    }
    private IEnumerator EnableRestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canRestart = true; // Cho phép restart sau 3 giây
    }

    public void ShowInGamePanel()
    {
        Time.timeScale = 1;
        int randomTheme = Random.Range(1, 4); // Tạo số ngẫu nhiên từ 1 đến 3
                                              // Phát nhạc nền dựa trên số ngẫu nhiên
        switch (randomTheme)
        {
            case 1:
                AudioManager.Instance.PlayMusic("Theme1");
                break;
            case 2:
                AudioManager.Instance.PlayMusic("Theme2");
                break;
            case 3:
                AudioManager.Instance.PlayMusic("Theme3");
                break;
        }
        AudioManager.Instance.PlaySFXWithDuration("Police", 6f);
        GameManager.Instance.AdjustCameraDistance(85f);
        homePanel.SetActive(false);
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        chooseCharacterPanel.SetActive(false);
        inGamePanel.SetActive(true);
        //ChangeMesh.Instance.SaveMesh();
    }
    public void ShowGameOverPanel()
    {
        homePanel.SetActive(false);
        settingsPanel.SetActive(false);
        inGamePanel.SetActive(false);
        pausePanel.SetActive(false);
        chooseCharacterPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
    public void ShowPausePanel()
    {
        Time.timeScale = 0;
        homePanel.SetActive(false);
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        chooseCharacterPanel.SetActive(false);
        inGamePanel.SetActive(false);
        pausePanel.SetActive(true);
    }
    public void ShowSettingPanel()
    {
        Time.timeScale = 0;
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        inGamePanel.SetActive(false);
        // Ẩn panel chính
        homePanel.SetActive(false);
        // Hiện SettingPanel
        chooseCharacterPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Hàm để quay lại màn hình chính
    public void ShowHomePanel()
    {
        AudioManager.Instance.PlayMusic("Intro");
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        inGamePanel.SetActive(false);
        // Ẩn SettingPanel
        settingsPanel.SetActive(false);
        // Hiện panel chính
        chooseCharacterPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    public void ShowChoooseCharacterPanel()
    {
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        inGamePanel.SetActive(false);
        // Ẩn SettingPanel
        settingsPanel.SetActive(false);
        // Hiện panel chính
        homePanel.SetActive(false);
        chooseCharacterPanel.SetActive(true);
    }    
    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        AudioManager.Instance.ToggleMusic();
        UpdateMusicIconHome();
        UpdateMusicIconGame();
    }
    public void ToggleSFX()
    {
        isSFXOn = !isSFXOn;
        AudioManager.Instance.ToggleSFX();
        UpdateSFXIconHome();
        UpdateSFXIconGame();
    }
    private void UpdateMusicIconGame()
    {
        Color iconColor = musicIconGame.color;
        iconColor.a = isMusicOn ? 1f : 0.5f;  // Nếu tắt, icon bị mờ đi
        musicIconGame.color = iconColor;
    }
    private void UpdateMusicIconHome()
    {
        Color iconColor = musicIconHome.color;
        iconColor.a = isMusicOn ? 1f : 0.5f;  // Nếu tắt, icon bị mờ đi
        musicIconHome.color = iconColor;
    }

    private void UpdateSFXIconGame()
    {
        Color iconColor = sfxIconGame.color;
        iconColor.a = isSFXOn ? 1f : 0.5f;  // Nếu tắt, icon bị mờ đi
        sfxIconGame.color = iconColor;
    }
    private void UpdateSFXIconHome()
    {
        Color iconColor = sfxIconHome.color;
        iconColor.a = isSFXOn ? 1f : 0.5f;  // Nếu tắt, icon bị mờ đi
        sfxIconHome.color = iconColor;
    }
    public void Play()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayMusic("Theme");
        //ChangeMesh.Instance.SaveMesh();
        ShowInGamePanel();
    }
    public void Home()
    {
        GameManager.Instance.RestartGame();
    }
}
