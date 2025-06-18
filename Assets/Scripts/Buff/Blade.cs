using System.Collections;
using UnityEngine;

public class Blade : MonoBehaviour, IBuff
{
    public GameObject bladePrefab; // Prefab của lưỡi cưa
    public float rotationSpeed = 600f; // Tốc độ xoay của lưỡi cưa
    public float lifetime; // Thời gian tồn tại
    private GameObject pl;
    private GameObject spawnedBlade; // Lưỡi cưa đã spawn

    public void Apply(GameObject player)
    {
        pl = player;
        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player!");
            return;
        }
        AudioManager.Instance.playSFX("Blade"); // Phát âm thanh Blade
        GameManager.Instance.ChangeBuff("Blade");

        // Tạo lưỡi cưa ngay tại vị trí của Player
        spawnedBlade = Instantiate(bladePrefab, new Vector3(player.transform.position.x, 1f, player.transform.position.z), Quaternion.identity);
        spawnedBlade.transform.SetParent(player.transform); // Gắn vào player để nó di chuyển theo

        StartCoroutine(DestroyAfterTime());
    }

    private void FixedUpdate()
    {
        if (spawnedBlade != null)
        {
            // **Giữ cố định Y**
            Vector3 fixedPosition = new Vector3(pl.transform.position.x, 1f, pl.transform.position.z);
            spawnedBlade.transform.position = fixedPosition;
            // **Reset lại rotation để tránh bị nghiêng theo xe**
            spawnedBlade.transform.rotation = Quaternion.Euler(0, spawnedBlade.transform.rotation.eulerAngles.y, 0);

            // Xoay quanh trục Y
            spawnedBlade.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
    
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(spawnedBlade); // Xóa lưỡi cưa
        if (gameObject.scene.IsValid())
        {
            Destroy(gameObject);
        }
    }
}
