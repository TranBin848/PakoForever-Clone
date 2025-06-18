using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour, IBuff
{
    public GameObject laserPrefab;  // Prefab của laser
    public float rotationSpeed; // Tốc độ quay (độ/giây)
    public float offsetDistance; // Khoảng cách laser với player
    public float lifetime; // Thời gian tồn tại
    private GameObject spawnedLaser; // Biến lưu laser đã spawn
    private GameObject rotationPivot; // Trục xoay
    private GameObject pl;
    public void Apply(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("Không tìm thấy Player!");
            return;
        }
        pl = player;
        AudioManager.Instance.playSFX("Laser"); // Phát âm thanh
        GameManager.Instance.ChangeBuff("Laser");

        rotationPivot = new GameObject("LaserPivot");
        rotationPivot.transform.position = player.transform.position;
        rotationPivot.transform.SetParent(player.transform); // Gán pivot vào Player để nó đi theo Player

        // Spawn laser và gán nó vào pivot
        spawnedLaser = Instantiate(laserPrefab, rotationPivot.transform);
        spawnedLaser.transform.localPosition = new Vector3(offsetDistance, 1f, 0); // Đặt cách Player một khoảng
        StartCoroutine(DestroyAfterTime());
    }
    public void FixedUpdate()
    {
        if (rotationPivot == null) return;

        rotationPivot.transform.rotation = Quaternion.Euler(0, rotationPivot.transform.rotation.eulerAngles.y, 0);
        rotationPivot.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(spawnedLaser); // Xóa lưỡi cưa
        if (gameObject.scene.IsValid())
        {
            Destroy(gameObject);
        }
    }
}
