using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class IncreasePlayerSize : MonoBehaviour, IBuff
{
    public float duration = 10f;
    public void Apply(GameObject player)
    {
        AudioManager.Instance.playSFX("Giant");
        GameManager.Instance.ChangeBuff("Giant");
        player.transform.localScale *= 2f;
        CarController.Instance.cantDestroy = true;
        // Bắt đầu Coroutine để reset sau 10s
        player.GetComponent<CarController>().StartCoroutine(ResetSize(player));
        if (gameObject.scene.IsValid())
        {
            Destroy(gameObject);
        }
    }
    private System.Collections.IEnumerator ResetSize(GameObject player)
    {
        yield return new WaitForSeconds(duration);

        if (player != null)
        {
            // Khôi phục kích thước ban đầu
            player.transform.localScale /= 2f;

            CarController.Instance.cantDestroy = false;
        }
    }
}
