using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float blinkDuration; // Thời gian nhấp nháy
    public float blinkInterval = 0.1f; // Tốc độ nhấp nháy

    private Renderer obstacleRenderer;
    private Collider obstacleCollider;

    private void Start()
    {
        blinkDuration = 0.7f;
        obstacleRenderer = GetComponent<Renderer>();
        obstacleCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision other)
    {
        // Xử lý va chạm với Player (vì Player có isTrigger = false)
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(BlinkAndDestroy());
        }
        if (other.gameObject.CompareTag("Ghost"))
        {
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Xử lý va chạm với Blade (vì Blade có isTrigger = true)
        if (other.gameObject.CompareTag("Blade") || other.gameObject.CompareTag("Laser"))
        {
            StartCoroutine(BlinkAndDestroy());
        }
        
    }

    private IEnumerator BlinkAndDestroy()
    {
        float endTime = Time.time + blinkDuration;
        bool isVisible = true;

        // Nhấp nháy obstacle trong thời gian blinkDuration
        while (Time.time < endTime)
        {
            isVisible = !isVisible;
            obstacleRenderer.enabled = isVisible;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Tắt Collider để tránh va chạm
        if (obstacleCollider != null)
        {
            obstacleCollider.enabled = false;
        }

        // Xóa obstacle khỏi scene
        Destroy(gameObject);
    }
}
