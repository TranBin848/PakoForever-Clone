using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour, IBuff
{
    public GameObject ghostPrefab; // Prefab của ngoại hình ma quái
    public float duration = 10f; // Thời gian tồn tại của buff

    private GameObject ghostInstance; // Lưu ghost object
    private GameObject originalModel; // Lưu model gốc của player

    public void Apply(GameObject player)
    {
        AudioManager.Instance.playSFX("Ghost");
        player.tag = "Ghost";

        // Tìm model hiện tại của player (giả sử nó là con của Player)
        originalModel = player.transform.GetChild(0).gameObject;
        // Tạo ghostPrefab làm model mới

        if (originalModel != null)
        {
            // Lưu vị trí và xoay của model gốc
            Vector3 originalLocalPosition = originalModel.transform.localPosition;
            Quaternion originalLocalRotation = originalModel.transform.localRotation;
            Vector3 originalLocalScale = originalModel.transform.localScale;

            originalModel.SetActive(false); // Ẩn model gốc

            // Tạo ghostPrefab làm model mới
            ghostInstance = Instantiate(ghostPrefab, player.transform);
            ghostInstance.transform.localPosition = originalLocalPosition; // Đặt về đúng vị trí
            ghostInstance.transform.localRotation = originalLocalRotation; // Giữ nguyên xoay
            ghostInstance.transform.localScale = originalLocalScale; // Giữ nguyên kích thước
        }

        // Bắt đầu Coroutine để reset sau khi hết thời gian
        CarController.Instance.cantDestroy = true;  
        player.GetComponent<CarController>().StartCoroutine(ResetGhost(player));
    }

    private IEnumerator ResetGhost(GameObject player)
    {
        yield return new WaitForSeconds(duration);

        if (ghostInstance != null)
        {
            Destroy(ghostInstance); // Xóa model ma quái
        }

        if (originalModel != null)
        {
            originalModel.SetActive(true); // Hiện lại model gốc
        }
        CarController.Instance.cantDestroy = false;
        player.tag = "Player";
    }


}
