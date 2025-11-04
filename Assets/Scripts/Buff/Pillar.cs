using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour, IBuff
{
    public GameObject pillarPrefab; // Prefab của cọc gỗ
    public ParticleSystem groundExplosionPrefab; // Hiệu ứng nổ đất
    public float duration = 5.5f; // Thời gian tồn tại của buff
    public float spawnInterval = 0.2f; // Thời gian giữa các lần triệu hồi
    public float spawnDistance = 2f; // Khoảng cách phía sau player
    public float riseSpeed = 30f; // Tốc độ trồi lên từ đất
    private float currentTargetY = -1.5f; // Mức cao ban đầu

    public void Apply(GameObject player)
    {
        if (player == null) return;

        AudioManager.Instance.playSFX("Pillar");
        GameManager.Instance.ChangeBuff("Pillar");
        // Bắt đầu triệu hồi cọc gỗ mỗi 0.5s trong 7s
        player.GetComponent<CarController>().StartCoroutine(SpawnPillars(player));
        
    }

    private IEnumerator SpawnPillars(GameObject player)
    {
        AudioManager.Instance.playSFX("Pillar");

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (player == null || CarController.Instance.isDisabled) yield break;

            // Tính toán vị trí spawn ngay sau lưng player
            Vector3 spawnPos = player.transform.position - player.transform.forward * spawnDistance;
            spawnPos.y = -3f; // Đặt dưới đất

            // Triệu hồi cọc gỗ
            GameObject pillar = Instantiate(pillarPrefab, spawnPos, Quaternion.identity);
            // **Triệu hồi hiệu ứng nổ đất**
            if (groundExplosionPrefab != null)
            {
                ParticleSystem explosion = Instantiate(groundExplosionPrefab,  new Vector3(spawnPos.x, 0f, spawnPos.z), Quaternion.identity);
                explosion.Play();
                Destroy(explosion.gameObject, 2f); // Xóa hiệu ứng sau 2s
            }
            // Làm cọc trồi lên từ đất
            StartCoroutine(RisePillar(pillar, currentTargetY));

            // **Tăng dần chiều cao tối đa của cọc gỗ**
            if (currentTargetY < 3f)
                currentTargetY += 0.5f;

            elapsedTime += spawnInterval;
            yield return new WaitForSeconds(spawnInterval);
        }
        if (gameObject.scene.IsValid())
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator RisePillar(GameObject pillar, float targetY)
    {
        while (pillar.transform.position.y < targetY)
        {
            pillar.transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            yield return null;
        }
    }

}
