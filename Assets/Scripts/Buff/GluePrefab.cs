using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GluePrefab : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject); // Hủy GluePrefab khi chạm vào Enemy
        }
    }
}
