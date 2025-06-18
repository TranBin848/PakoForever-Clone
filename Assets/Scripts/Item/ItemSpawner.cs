using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance { get; private set; }
    public GameObject giftPrefab; // Prefab hộp quà
    public Transform playerTransform; // Người chơi
    public float minSpawnDistance; // Khoảng cách tối thiểu từ người chơi
    public float maxSpawnDistance; // Khoảng cách tối đa từ người chơi
    public float spawnCooldown = 15f; // Thời gian cooldown trước khi spawn lại
    private bool canSpawn = false; // Kiểm soát cooldown

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        StartCoroutine(InitialCooldown()); // Đợi 5s trước khi cho phép spawn 
    }
    private IEnumerator InitialCooldown()
    {
        canSpawn = false; // Ngăn chặn spawn ngay lập tức
        yield return new WaitForSeconds(8f); // Đợi 5 giây
        canSpawn = true; // Cho phép spawn
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameover) return;

        if (canSpawn)
        {
            TrySpawnGift();
        }
    }

    private void TrySpawnGift()
    {
        // Nếu đang cooldown hoặc hộp quà vẫn còn trên bản đồ thì không spawn
        if (!canSpawn || GameObject.FindGameObjectWithTag("Gift") != null) return;

        canSpawn = false; // Ngăn chặn spawn liên tục

        // Lấy vị trí spawn hợp lệ
        Vector3 spawnPosition = GetSpawnPosition();

        // Tạo hộp quà tại vị trí spawn hợp lệ
        Instantiate(giftPrefab, spawnPosition, Quaternion.identity);

    }

    private Vector3 GetSpawnPosition()
    {
        // Tạo vị trí spawn ngẫu nhiên trong khoảng cách hợp lý
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        return new Vector3(playerTransform.position.x + randomDirection.x * randomDistance,
                           playerTransform.position.y,
                           playerTransform.position.z + randomDirection.y * randomDistance);
    }

    public void StartGiftCooldown()
    {
        StartCoroutine(SpawnCooldown());
    }

    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(spawnCooldown);
        canSpawn = true;
        TrySpawnGift();
    }
}
