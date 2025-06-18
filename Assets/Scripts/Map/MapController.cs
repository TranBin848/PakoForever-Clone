using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks; // Danh sách prefab chunk
    private List<bool> isActiveList = new List<bool>(); // Danh sách trạng thái chunk

    public GameObject player; // Nhân vật di chuyển
    public float checkerRadius; // Bán kính kiểm tra
    public float chunkSize; // Kích thước chunk cố định
    public LayerMask terrainMask; // Mask để kiểm tra chunk

    [Header("Optimization")]
    public int poolSize; // Số lượng chunk trong pool
    public float maxOpDist; // Khoảng cách tối đa để hiển thị chunk

    private Queue<GameObject> chunkPool = new Queue<GameObject>(); // Pool chunk
    private HashSet<Vector3> spawnedPositions = new HashSet<Vector3>(); // Đánh dấu các vị trí đã spawn
    private List<GameObject> activeChunks = new List<GameObject>(); // Danh sách chunk đang hoạt động


    void Start()
    {
        foreach (var obj in terrainChunks)
        {
            isActiveList.Add(true); // Ban đầu tất cả đều được sử dụng
        }
        // Tạo pool chunk ban đầu
        GameObject chunk = Instantiate(terrainChunks[5]);
        isActiveList[5] = false;
        chunk.SetActive(false); // Vô hiệu hóa chunk
        chunkPool.Enqueue(chunk); // Thêm vào pool
        for (int i = 1; i < poolSize; i++)
        {
            int rand = Random.Range(0, terrainChunks.Count);
            
            if (isActiveList[rand] == false)
            {
                i--;
                continue;
            }
            if (rand == 6) isActiveList[6] = false;
            if (rand == 8) isActiveList[8] = false;
            if (rand == 9) isActiveList[9] = false;
            if (rand == 10) isActiveList[10] = false;
            if (rand == 11) isActiveList[11] = false;
            chunk = Instantiate(terrainChunks[rand]);
            chunk.SetActive(false); // Vô hiệu hóa chunk
            chunkPool.Enqueue(chunk); // Thêm vào pool
        }

        // Spawn chunk đầu tiên tại vị trí nhân vật
        Vector3 initialPosition = new Vector3(
            Mathf.Floor(player.transform.position.x / chunkSize) * chunkSize,
            0,
            Mathf.Floor(player.transform.position.z / chunkSize) * chunkSize
        );
        SpawnChunk(initialPosition);
        ChunkChecker();
    }

    void FixedUpdate()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        Vector3 playerPosition = player.transform.position;

        // Số lượng chunk cần kiểm tra xung quanh nhân vật
        int chunksInRadius = Mathf.CeilToInt(checkerRadius / chunkSize);

        // Kiểm tra từng vị trí trong phạm vi
        for (int x = -chunksInRadius; x <= chunksInRadius; x++)
        {
            for (int z = -chunksInRadius; z <= chunksInRadius; z++)
            {
                // Tính tọa độ chunk mới
                Vector3 targetPosition = new Vector3(
                    Mathf.Floor(playerPosition.x / chunkSize) * chunkSize + x * chunkSize,
                    0,
                    Mathf.Floor(playerPosition.z / chunkSize) * chunkSize + z * chunkSize
                );

                Collider[] colliders = Physics.OverlapSphere(targetPosition, chunkSize * 0.5f, terrainMask);
                if (!spawnedPositions.Contains(targetPosition) && colliders.Length == 0)
                {
                    SpawnChunk(targetPosition);
                }
            }
        }
    }

    void SpawnChunk(Vector3 position)
    {
        if (chunkPool.Count > 0)
        {
            GameObject chunk = chunkPool.Dequeue(); // Lấy chunk từ pool
            chunk.transform.position = position; // Đặt lại vị trí
            chunk.SetActive(true); // Kích hoạt chunk
            spawnedPositions.Add(position); // Đánh dấu vị trí đã spawn
            activeChunks.Add(chunk); // Thêm vào danh sách chunk đang hoạt động
        }
        else
        {
            Debug.LogWarning("Pool đã hết chunk!");
        }
    }

    void ChunkOptimizer()
    {
        for (int i = activeChunks.Count - 1; i >= 0; i--)
        {
            GameObject chunk = activeChunks[i];

            // Tính khoảng cách giữa player và chunk
            float opDist = Vector3.Distance(player.transform.position, chunk.transform.position);

            if (opDist > maxOpDist)
            {
                DespawnChunk(chunk);
                activeChunks.RemoveAt(i);
            }
        }
    }

    void DespawnChunk(GameObject chunk)
    {
        chunk.SetActive(false); // Vô hiệu hóa chunk
        chunkPool.Enqueue(chunk); // Đưa chunk trở lại pool
        spawnedPositions.Remove(chunk.transform.position); // Gỡ vị trí khỏi danh sách
    }
}
