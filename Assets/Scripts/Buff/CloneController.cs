using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra nếu va chạm với vật thể nào đó
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy")) // Thay "Obstacle" bằng tag của vật thể bạn muốn
        {
            Destroy(this.gameObject);   
        }
    }
}
