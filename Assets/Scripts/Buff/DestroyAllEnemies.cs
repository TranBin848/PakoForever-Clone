using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAllEnemies : MonoBehaviour, IBuff
{
    public ParticleSystem laser;
    public void Apply(GameObject player)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            AudioManager.Instance.playSFX("Thunder"); // Phát âm thanh nổ
            GameManager.Instance.ChangeBuff("Thunder");

            // Instantiate ParticleSystem tại vị trí của enemy
            ParticleSystem ps = Instantiate(laser, enemy.transform.position, Quaternion.identity);
            // Bắt đầu phát hiệu ứng nổ
            ps.Play();
            Destroy(ps.gameObject, 2f); // Xóa hiệu ứng sau 2s
            EnemySpawner.Instance.EnemyDestroyed(enemy); // Giảm số lượng enemy
        }

        if (gameObject.scene.IsValid())
        {
            Destroy(gameObject);
        }
    }
}
