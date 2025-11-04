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

    private float steeringResponse = 8f; // phản ứng chậm hơn → drift trễ
    private float driftInertia = 0.9f;   // quán tính giữ hướng cũ

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
    }

    private void FixedUpdate()
    {
        if (CarController.Instance.isDisabled || isStuck || player == null)
            return;

        HandleSpeed();
        HandleSteering();
        MoveEnemy();
    }

    private void HandleSteering()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Góc thật giữa hướng xe và hướng đến player
        float desiredSteer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up) / 45f;
        desiredSteer = Mathf.Clamp(desiredSteer, -1f, 1f);

        // Thêm độ trễ phản ứng (xe không bẻ lái ngay)
        steerInput = Mathf.Lerp(steerInput, desiredSteer, Time.deltaTime * steeringResponse);
    }

    private void HandleSpeed()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Sử dụng Time.fixedDeltaTime
        CurrentSpeed += data.Acceleration * Time.fixedDeltaTime;
        CurrentSpeed = Mathf.Min(CurrentSpeed, data.MaxSpeed);

        if (distanceToPlayer <= 5f && Mathf.Abs(steerInput) > 0.8f)
        {
            // Giảm tốc mạnh hơn khi cố gắng rẽ gấp (tạo hiệu ứng phanh gấp/hoảng loạn)
            CurrentSpeed -= data.Deceleration * Time.fixedDeltaTime * 2f;
            CurrentSpeed = Mathf.Max(CurrentSpeed, data.MinSpeed);
        }
    }

    private void MoveEnemy()
    {
        // Thêm "lực quán tính" – xe giữ hướng cũ nhiều hơn khi đổi hướng
        Vector3 desiredForward = Vector3.Lerp(transform.forward, transform.forward + transform.right * steerInput, Time.deltaTime * driftInertia).normalized;

        // Giảm tốc nhẹ khi đang đổi hướng để tạo cảm giác trượt
        float driftSlowdown = 1f - Mathf.Abs(steerInput) * 0.2f;

        Vector3 forwardVelocity = desiredForward * CurrentSpeed * driftSlowdown;
        Vector3 sidewaysVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right) * data.DriftFactor;

        rb.linearVelocity = forwardVelocity + sidewaysVelocity;

        float targetTilt = steerInput * data.TiltAngle;
        CurrentTilt = Mathf.Lerp(CurrentTilt, targetTilt, Time.deltaTime * 2f);

        currentSteerAngle += steerInput * CurrentSpeed * data.SteerAngle * Time.deltaTime;

        Quaternion tiltRotation = Quaternion.Euler(0, 0, CurrentTilt);
        Quaternion steerRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        rb.MoveRotation(steerRotation * tiltRotation);
    }

    void StopMovement()
    {
        isStuck = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    // Phương thức xử lý va chạm
    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra nếu va chạm với các đối tượng cần thiết
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Giant")
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
        if(other.gameObject.CompareTag("Blade") || other.gameObject.CompareTag("Laser") || other.gameObject.CompareTag("Pillar") )
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
    public void ResetEnemy()
    {
        isStuck = false;
        // Đảm bảo tốc độ và vận tốc Rigidbody được reset
        CurrentSpeed = data.MinSpeed;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        // Cập nhật góc lái ban đầu từ rotation của transform (vừa được Spawner gán)
        currentSteerAngle = transform.rotation.eulerAngles.y;
    }
}
