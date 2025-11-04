using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    public List<GameObject> enemyPrefabs; // Danh sách các loại enemy
    public Transform player; // Tham chiếu tới người chơi

    [Header("Game Flow")]
    public float initialDelay = 3f;

    [Header("Spawn Settings")]
    public float baseSpawnDistance = 30f; // Khoảng cách spawn cơ bản từ người chơi (30m)
    public float spawnInterval = 3f; // Thời gian giữa mỗi lần spawn cơ bản
    public int maxEnemies = 5; // Số lượng enemy tối đa ban đầu

    [Header("Difficulty Settings")]
    public float difficultyIncreaseRate = 10f; // Mỗi X giây sẽ tính toán lại độ khó
    public float maxSpeedForDynamicCalc = 50f; // Tốc độ tối đa của Player để tính toán (Thay bằng data.MaxSpeed của Player)

    public int currentEnemyCount = 0;
    public Queue<GameObject> enemyPool = new Queue<GameObject>();

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
    void Start()
    {
        for(int i = 0; i < 25; i++)  
        {
            GameObject selectedEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject newEnemy = Instantiate(selectedEnemy);
            newEnemy.SetActive(false);
            enemyPool.Enqueue(newEnemy);
        }
        StartCoroutine(DelayedGameStart());
    }


    private void SpawnEnemy()
    {
        if (currentEnemyCount >= maxEnemies || CarController.Instance.isDisabled)
            return;

        // 🔥 1. Tính toán khoảng cách spawn động dựa trên tốc độ Player
        float dynamicSpawnDistance = GetDynamicSpawnDistance();

        Vector3 spawnPosition;
        Quaternion initialRotation;

        // 2. Lấy vị trí và góc nhìn ban đầu
        GetRandomSpawnPosition(out spawnPosition, out initialRotation, dynamicSpawnDistance);

        GameObject enemy = GetEnemyFromPool();
        if (enemy == null) return;

        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = initialRotation;

        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            // ResetEnemy() phải được thêm vào EnemyAI.cs (đã đề cập ở trên)
            enemyAI.ResetEnemy();
        }

        enemy.SetActive(true);
        currentEnemyCount++;
    }

    private float GetDynamicSpawnDistance()
    {
        if (CarController.Instance == null) return baseSpawnDistance;

        float playerSpeed = CarController.Instance.CurrentSpeed;

        // Tỷ lệ tốc độ (giới hạn từ 0 đến 1)
        float speedRatio = Mathf.Clamp01(playerSpeed / maxSpeedForDynamicCalc);

        // Tăng khoảng cách từ 1.0x (tốc độ thấp) lên 1.5x (max speed)
        // Điều này đảm bảo Enemy không spawn quá gần khi Player chạy nhanh
        float distanceMultiplier = 1.0f + speedRatio * 0.5f;

        return baseSpawnDistance * distanceMultiplier;
    }

    // ----------------------------------------------------
    // 🔥 Cập nhật GetRandomSpawnPosition để nhận tham số khoảng cách
    // ----------------------------------------------------
    private void GetRandomSpawnPosition(out Vector3 position, out Quaternion rotation, float distance)
    {
        // Vị trí ngẫu nhiên 360 độ
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        position = player.position + new Vector3(
            Mathf.Cos(randomAngle) * distance,
            0,
            Mathf.Sin(randomAngle) * distance
        );

        // Hướng quay: nhìn vào Player
        Vector3 lookDirection = player.position - position;
        lookDirection.y = 0;
        rotation = Quaternion.LookRotation(lookDirection);
    }

    private GameObject GetEnemyFromPool()
    {
        if (enemyPool.Count > 0)
        {
            return enemyPool.Dequeue(); // Lấy enemy từ pool
        }

        if (enemyPrefabs.Count > 0)
        {
            GameObject selectedEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject newEnemy = Instantiate(selectedEnemy);
            enemyPool.Enqueue(newEnemy); // Đưa vào pool ngay lập tức
            return newEnemy;
        }

        return null;
    }
    public void EnemyDestroyed(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy); // Đưa enemy vào pool thay vì hủy
        currentEnemyCount--;
    }

    IEnumerator IncreaseDifficultyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(difficultyIncreaseRate);

            // Lấy dữ liệu tốc độ Player
            float playerSpeed = CarController.Instance != null ? CarController.Instance.CurrentSpeed : 0f;
            float speedRatio = Mathf.Clamp01(playerSpeed / maxSpeedForDynamicCalc);

            // Tỷ lệ giảm: Giảm nhanh hơn khi Player chạy nhanh 
            float baseDecrease = 0.8f;
            float dynamicDecrease = baseDecrease - (speedRatio * 0.1f);

            float newSpawnInterval = Mathf.Max(0.1f, spawnInterval * dynamicDecrease);

            // Nếu có thay đổi, hủy và gọi lại InvokeRepeating
            if (Mathf.Abs(newSpawnInterval - spawnInterval) > 0.01f)
            {
                spawnInterval = newSpawnInterval;
                CancelInvoke(nameof(SpawnEnemy));
                InvokeRepeating(nameof(SpawnEnemy), spawnInterval, spawnInterval);
            }

            Debug.Log($"[Difficulty Up] interval: {spawnInterval:F2}s, distance: {GetDynamicSpawnDistance():F1}m");
        }
    }

    IEnumerator DelayedGameStart()
    {
        // Đợi 3 giây trước khi bắt đầu bất cứ thứ gì liên quan đến gameplay
        yield return new WaitForSeconds(initialDelay);

        Debug.Log("Game Start Delayed! Bắt đầu Spawn Enemy và tăng độ khó.");

        // Bắt đầu spawn enemy liên tục
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);

        // Bắt đầu tăng độ khó
        StartCoroutine(IncreaseDifficultyOverTime());
    }
}
