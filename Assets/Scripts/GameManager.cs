using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Thêm dòng này ở đầu script
using Cinemachine;
using System.Collections;

public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager Instance; // Singleton để truy cập từ mọi nơi
    public TMP_Text timeText; // Hiển thị thời gian trôi qua
    public TMP_Text resultText;
    public TMP_Text distanceText; // Hiển thị quãng đường đã đi
    public TMP_Text crashText;
    public TMP_Text buffText;
    public TMP_Text bestScoreText; // Hiển thị best score
    public TMP_Text bestDistText;
    public TMP_Text bestCrashText;

    public float elapsedTime = 0f; // Thời gian đã trôi qua
    public float distanceTravelled = 0f; // Quãng đường đã chạy
    public int CrashCar = 0;
    public string bText = "";

    private float bestScore = 0f; // Thời gian sống lâu nhất
    private float bestDist = 0f; // Quãng đường đi xa nhất
    private int bestCrash = 0; // Số lần va chạm nhiều nhất

    public bool isGameover = false;


    public CarController carController; // Tham chiếu đến CarController
    public CinemachineVirtualCamera virtualCamera;  // Virtual Camera
    private CinemachineFramingTransposer framingTransposer;  // Để chỉnh khoảng cách camera


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadData(GameData data)
    {
        this.bestScore = data.bestScore;
        this.bestDist = data.bestDist;
        this.bestCrash = data.bestCrash;
    }
    public void SaveData(ref GameData data)
    {
        data.bestScore = this.bestScore;
        data.bestDist = this.bestDist;
        data.bestCrash = this.bestCrash;   
    }
    private void Start()
    {
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        ZoomIn();
        elapsedTime = 0f;
        distanceTravelled = 0f;
        UpdateUI();
        if (PlayerPrefs.GetInt("ZoomOut", 0) == 1) // Nếu đang restart
        {
            PlayerPrefs.SetInt("ZoomOut", 0); // Reset lại trạng thái
            ZoomOut();
        }
    }

    private void Update()
    {
        // Cập nhật thời gian đã trôi qua
        elapsedTime += Time.deltaTime;
        // Cập nhật quãng đường (tính theo tốc độ của xe)
        if (carController != null)
        {
            distanceTravelled += carController.CurrentSpeed / 10 * Time.deltaTime; // Quãng đường = tốc độ * thời gian
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        timeText.text = $"{FormatTime(elapsedTime)}";
        distanceText.text = $"DIST: {distanceTravelled:F1}m";
        bestScoreText.text = $"BEST SCORE {FormatTime(bestScore)}";
        crashText.text = $"CRASH: {CrashCar}";
        buffText.text = bText;
    }
    public void ChangeBuff(string buff)
    {
        bText = buff;
        StartCoroutine(ResetBuffText());
    }

    private IEnumerator ResetBuffText()
    {
        yield return new WaitForSeconds(3f);
        bText = "";
    }
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000); // Lấy 3 chữ số miligiây

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
    public void ZoomIn()
    {
        AdjustCameraDistance(35f);
    }
    public void ZoomOut()
    {
        AdjustCameraDistance(95f);
    }
    public void GameOver()
    {
        isGameover = true;
        // Khi xe chết, kiểm tra best score
        if (elapsedTime > bestScore)
        {
            bestScore = elapsedTime;
        }
        if(distanceTravelled > bestDist)
        {
            bestDist = distanceTravelled;
        }
        if(CrashCar > bestCrash)
        {
            bestCrash = CrashCar;
        }
        bestDistText.text = $"BEST DIST: {bestDist}";
        bestCrashText.text = $"BEST CRASH: {bestCrash}"; 
        DataPersistenceManager.instance.SaveGame();
        resultText.text = $"{FormatTime(elapsedTime)}";
        MainGameController.Instance.ShowGameOverPanel();
        ZoomIn();
    }
    public void AdjustCameraDistance(float distance)
    {
        if (framingTransposer != null)
        {
            framingTransposer.m_CameraDistance = distance;  // Điều chỉnh khoảng cách camera
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
