using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyCat : MonoBehaviour, IBuff
{
    public GameObject playerPrefab;
    public int numClones; // Số lượng clone
    public float radius; // Khoảng cách giữa các bản sao
    private GameObject pl;
    private List<GameObject> clones = new List<GameObject>();
    
    public void Apply(GameObject player)
    {
        AudioManager.Instance.playSFX("Clone");
        GameManager.Instance.ChangeBuff("Clone");

        pl = player;
        SpawnClones(player);
        StartCoroutine(DestroyAfterTime(12f));
    }

    private void SpawnClones(GameObject player)
    {
        if (playerPrefab == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found!");
            return;
        }
        float angleStep = 360f / numClones; // Chia đều góc quanh player

        for (int i = 0; i < numClones; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad; // Đổi sang radian
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            GameObject copy = Instantiate(playerPrefab, player.transform.position + offset, player.transform.rotation);
            copy.tag = "Player";
            clones.Add(copy);
        }
    }

    private void FixedUpdate()
    {
        float angleStep = 360f / numClones; // Đảm bảo khoảng cách đều nhau
        for (int i = 0; i < clones.Count; i++)
        {
            GameObject copy = clones[i];
            if (copy == null) continue;

            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 targetPosition = pl.transform.position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            
            copy.transform.position = Vector3.Lerp(copy.transform.position, targetPosition, Time.deltaTime * 10f);
            copy.transform.rotation = pl.transform.rotation;

            if (copy.transform.position.y < -2)
            {
                Destroy(copy);
                clones.RemoveAt(i);
                i--; // Tránh lỗi danh sách bị thay đổi trong vòng lặp
            }
        }
    }
    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
