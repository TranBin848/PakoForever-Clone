using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    public List<GameObject> enemyPrefabs; // Danh sách các loại enemy
    public Transform player; // Tham chiếu tới người chơi
    public float spawnDistance; // Khoảng cách spawn từ người chơi
    public float spawnInterval; // Thời gian giữa mỗi lần spawn
    public int maxEnemies; // Số lượng enemy tối đa ban đầu
    public float difficultyIncreaseRate; // Mỗi X giây sẽ tăng độ khó

    public int currentEnemyCount = 0; // Biến đếm số lượng kẻ thù hiện tại
    private float elapsedTime = 0f; // Thời gian đã trôi qua

    public Queue<GameObject> enemyPool = new Queue<GameObject>(); // Object Pooling

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
        // Bắt đầu spawn enemy liên tục
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
        StartCoroutine(IncreaseDifficultyOverTime());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime; // Cập nhật thời gian trôi qua
        Debug.Log(enemyPool.Count);
    }

    private void SpawnEnemy()
    {
        if (currentEnemyCount >= maxEnemies || CarController.Instance.isDisabled)
            return; // Đạt giới hạn số lượng enemy

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = GetEnemyFromPool();
        if (enemy == null) return;
        enemy.transform.position = spawnPosition;
        enemy.SetActive(true);
        currentEnemyCount++;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return player.position + new Vector3(
            Mathf.Cos(randomAngle) * spawnDistance,
            0,
            Mathf.Sin(randomAngle) * spawnDistance
        );
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
            yield return new WaitForSeconds(difficultyIncreaseRate); // Đợi X giây để tăng độ khó

            spawnInterval = Mathf.Max(0.1f, spawnInterval * 0.8f); // Giảm thời gian spawn (giới hạn tối thiểu 0.5s)

            Debug.Log($"[Difficulty Up] maxEnemies: {maxEnemies}, spawnInterval: {spawnInterval}");
        }
    }
}
