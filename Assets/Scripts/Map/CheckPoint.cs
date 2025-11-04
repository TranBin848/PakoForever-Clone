using System.Collections;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Renderer lightRenderer; // Đèn trên checkpoint (gán trong Inspector)
    public Material activatedMaterial; // Material khi nháy
    public float blinkInterval = 0.2f; // Khoảng thời gian giữa mỗi lần nháy

    private Material originalMaterial; // Lưu material ban đầu
    private bool isBlinking = false; // Tránh nháy nhiều lần

    private void Start()
    {
        if (lightRenderer != null)
        {
            originalMaterial = lightRenderer.material; // Lưu material gốc
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Giant")) && !isBlinking)
        {
            AudioManager.Instance.playSFX("CheckPoint");
            StartCoroutine(BlinkMaterial());
        }
    }

    private IEnumerator BlinkMaterial()
    {
        isBlinking = true;
        float elapsedTime = 0f;

        while (elapsedTime < 3f) // Chạy hiệu ứng trong 2 giây
        {
            lightRenderer.material = (lightRenderer.material == originalMaterial) ? activatedMaterial : originalMaterial;
            yield return new WaitForSeconds(blinkInterval); // Chờ một khoảng thời gian
            elapsedTime += blinkInterval;
        }

        lightRenderer.material = originalMaterial; // Quay lại material gốc
        isBlinking = false;
    }
}
