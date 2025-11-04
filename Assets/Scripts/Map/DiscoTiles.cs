using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoTiles : MonoBehaviour
{
    public List<Texture2D> textures; // Danh sách các texture
    private MeshRenderer tileRenderer;

    private void Start()
    {
        tileRenderer = GetComponent<MeshRenderer>(); // Lấy renderer của tile
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
        if (tileRenderer == null) return;

        if (textures.Count == 0) return;

        Texture2D randomTexture = textures[Random.Range(0, textures.Count)];

        if (randomTexture == null) return;

        tileRenderer.material.mainTexture = randomTexture;
    }
}
