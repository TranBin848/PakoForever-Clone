using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    public GameObject[] buffPrefabs; // Các buff có thể spawn
    public Transform spawnPoint; // Vị trí spawn buff
    private List<GameObject> availableBuffs; // Danh sách buff chưa được sử dụng

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        ResetBuffs(); // Khởi tạo danh sách buff
        RemoveExistingBuffs(); // Hủy buff cũ khi khởi động
    }
    private void FixedUpdate()
    {
        if(GameManager.Instance.isGameover) RemoveExistingBuffs() ;
    }
    private void RemoveExistingBuffs()
    {
        MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>(); // Lấy tất cả các script trên scene
        foreach (var obj in allObjects)
        {
            if (obj is IBuff) // Kiểm tra nếu script đó implement IBuff
            {
                Destroy(obj.gameObject); // Hủy GameObject chứa buff
            }
        }
    }
    private void ResetBuffs()
    {
        availableBuffs = new List<GameObject>(buffPrefabs);
    }
    public GameObject GetRandomBuff()
    {
        if (availableBuffs.Count == 0)
        {
            ResetBuffs(); // Nếu đã dùng hết buff, reset lại
        }

        // Chọn buff ngẫu nhiên từ danh sách buff chưa dùng
        int randomIndex = Random.Range(0, availableBuffs.Count);
        GameObject selectedBuff = availableBuffs[randomIndex];

        // Loại buff này khỏi danh sách đã sử dụng
        availableBuffs.RemoveAt(randomIndex);

        // Tạo buff tại vị trí spawn
        GameObject buffObject = Instantiate(selectedBuff, spawnPoint.position, Quaternion.identity);

        return buffObject;
    }
}
