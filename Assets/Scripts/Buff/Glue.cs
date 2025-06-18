using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glue : MonoBehaviour, IBuff
{
    public GameObject gluePrefab; // Prefab của keo dán
    public float duration = 7f; // Thời gian hiệu ứng kéo dài
    public float spawnInterval = 0.5f; // Khoảng thời gian giữa các lần rải keo
    public float spacing = 1f; // Khoảng cách giữa các vết keo

    public void Apply(GameObject player)
    {
        AudioManager.Instance.playSFX("Glue");
        GameManager.Instance.ChangeBuff("Glue");
        StartCoroutine(SpawnGlue(player));
    }

    private IEnumerator SpawnGlue(GameObject player)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            if (CarController.Instance.isDisabled) break;
            Vector3 gluePosition = player.transform.position - player.transform.forward * spacing;
            gluePosition.y = 0f;
            Instantiate(gluePrefab, gluePosition, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;
        }
        if (gameObject.scene.IsValid())
        {
            Destroy(gameObject);
        }
    }
    
}
