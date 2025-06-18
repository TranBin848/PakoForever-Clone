using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoTiles : MonoBehaviour
{
    public List<Texture> textures; // Danh sách các texture
    private Renderer tileRenderer;

    private void Start()
    {
        tileRenderer = GetComponent<Renderer>(); // Lấy renderer của tile
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
        if (textures.Count == 0) return;

        // Chọn texture ngẫu nhiên
        Texture randomTexture = textures[Random.Range(0, textures.Count)];

        // Gán texture mới cho material của tile
        tileRenderer.material.mainTexture = randomTexture;
    }
}
