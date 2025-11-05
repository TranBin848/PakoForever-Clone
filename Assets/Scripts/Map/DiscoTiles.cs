using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoTiles : MonoBehaviour
{
    public List<Texture2D> textures; // Danh sách các texture
    private MeshRenderer tileRenderer;
    private Texture2D currentTexture; // Lưu texture hiện tại

    private void Start()
    {
        tileRenderer = GetComponent<MeshRenderer>(); // Lấy renderer của tile
        if (tileRenderer != null)
        {
            currentTexture = tileRenderer.material.mainTexture as Texture2D;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy")) // Kiểm tra nếu là xe
        {
            ChangeTexture();
        }
    }

    private void ChangeTexture()
    {
        if (tileRenderer == null || textures.Count == 0)
            return;

        // Tạo danh sách texture khả dụng (không trùng với texture hiện tại)
        List<Texture2D> availableTextures = new List<Texture2D>(textures);
        availableTextures.Remove(currentTexture);

        // Nếu tất cả texture đều giống nhau → thoát
        if (availableTextures.Count == 0)
            return;

        // Chọn texture ngẫu nhiên khác với texture hiện tại
        Texture2D newTexture = availableTextures[Random.Range(0, availableTextures.Count)];

        // Áp dụng texture mới
        tileRenderer.material.mainTexture = newTexture;
        currentTexture = newTexture;
    }
}
