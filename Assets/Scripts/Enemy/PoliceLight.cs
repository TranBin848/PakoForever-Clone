using System.Collections;
using UnityEngine;

public class PoliceLight : MonoBehaviour
{
    public float scaleUpSize = 2.5f; // Kích thước khi phóng to
    public float scaleDownSize = 1.5f; // Kích thước ban đầu
    public float speed = 0.5f; // Thời gian đổi kích thước

    private Coroutine blinkCoroutine;

    private void OnEnable()
    {
        // Đảm bảo Coroutine không chạy trùng
        if (blinkCoroutine == null)
        {
            blinkCoroutine = StartCoroutine(BlinkSize());
        }
    }

    private void OnDisable()
    {
        // Dừng Coroutine nếu GameObject bị tắt
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }

    IEnumerator BlinkSize()
    {
        while (true)
        {
            transform.localScale = Vector3.one * scaleUpSize;
            yield return new WaitForSeconds(speed);

            transform.localScale = Vector3.one * scaleDownSize;
            yield return new WaitForSeconds(speed);
        }
    }
}
