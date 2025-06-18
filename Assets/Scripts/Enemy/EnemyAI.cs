using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public PlayerData data;
    public Transform player; // Tham chiếu đến vị trí của người chơi
    private Vector3 MoveForce;
    public float CurrentSpeed;
    public float CurrentTilt;
    public float currentSteerAngle;
    private Rigidbody rb;
    public GameObject explosionEffectPrefab; // Prefab hiệu ứng nổ
    public bool isStuck = false;
    private float steerInput = 0; // Biến lưu hướng di chuyển
    private float distanceToPlayer = 0; // Khoảng cách tới player

    private void Start()
    {
        if (CarController.Instance != null)
        {
            player = CarController.Instance.transform;
        }
        else
        {
            Debug.LogError("Player instance not found!");
        }

        CurrentSpeed = data.MinSpeed;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (CarController.Instance.isDisabled || isStuck || player == null)
            return;

        HandleSteering();
        HandleSpeed();
    }

    private void FixedUpdate()
    {
        if (CarController.Instance.isDisabled || isStuck || player == null)
            return;
        MoveEnemy();
    }

    private void HandleSteering()
    {
        // Hướng enemy về phía người chơi
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        steerInput = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up) / 45f; // Điều chỉnh steerInput theo hướng đến player
        steerInput = Mathf.Clamp(steerInput, -1f, 1f);
    }

    private void HandleSpeed()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= 3 && Mathf.Abs(steerInput) > 0f) // Giảm tốc khi gần
        {
            CurrentSpeed -= data.Deceleration * Time.deltaTime;
            CurrentSpeed = Mathf.Max(CurrentSpeed, data.MinSpeed);
        }
        else
        {
            CurrentSpeed += data.Acceleration * Time.deltaTime;
            CurrentSpeed = Mathf.Min(CurrentSpeed, data.MaxSpeed);
        }
        
    }

    private void MoveEnemy()
    {
        // Tính toán tốc độ di chuyển
        Vector3 forwardVelocity = transform.forward * CurrentSpeed;
        Vector3 sidewaysVelocity = transform.right * Vector3.Dot(rb.velocity, transform.right) * data.DriftFactor;

        // Kết hợp chuyển động thẳng và drift
        rb.velocity = forwardVelocity + sidewaysVelocity;

        // Tính toán góc nghiêng
        float targetTilt = steerInput * data.TiltAngle;
        CurrentTilt = Mathf.Lerp(CurrentTilt, targetTilt, Time.deltaTime * 2f);

        // Tính toán góc xoay
        currentSteerAngle += steerInput * CurrentSpeed * data.SteerAngle * Time.deltaTime;

        // Kết hợp xoay và nghiêng
        Quaternion tiltRotation = Quaternion.Euler(0, 0, CurrentTilt);
        Quaternion steerRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        rb.MoveRotation(steerRotation * tiltRotation); // Sử dụng MoveRotation để làm mềm chuyển động
    }

    void StopMovement()
    {
        isStuck = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    // Phương thức xử lý va chạm
    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra nếu va chạm với các đối tượng cần thiết
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy")
            || collision.gameObject.CompareTag("Player"))
        {
            // Kích hoạt hiệu ứng nổ
            PlayExplosionEffect();
            // Gọi hàm để vô hiệu hóa Enemy
            AudioManager.Instance.playSFX("Explosion");
            GameManager.Instance.CrashCar += 1;
            EnemySpawner.Instance.EnemyDestroyed(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Glue"))
        {
            StopMovement(); 
        }
        if(other.gameObject.CompareTag("Blade") || other.gameObject.CompareTag("Laser") || other.gameObject.CompareTag("Pillar"))
        {
            // Kích hoạt hiệu ứng nổ
            PlayExplosionEffect();
            
            // Gọi hàm để vô hiệu hóa Enemy
            AudioManager.Instance.playSFX("Explosion");
            GameManager.Instance.CrashCar += 1;
            EnemySpawner.Instance.EnemyDestroyed(this.gameObject);
        }

    }
    void PlayExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            // Tạo hiệu ứng nổ tại vị trí của Enemy
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            // Hủy hiệu ứng sau 3 giây
            Destroy(explosion, 3f);

        }
    }
    
}
