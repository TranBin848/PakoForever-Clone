using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public static CarController Instance { get; private set; }
    public PlayerData data;
    private Vector3 MoveForce;
    public float CurrentSpeed; // Tốc độ hiện tại của xe
    public float CurrentTilt; // Góc nghiêng hiện tại của xe
    public float currentSteerAngle; // Góc xoay hiện tại
    private Rigidbody rb; // Tham chiếu đến Rigidbody của xe
    public bool isDisabled = false; // Trạng thái xe đã bị vô hiệu hóa
    public GameObject explosionEffectPrefab; // Để lưu prefab của hiệu ứng nổ
    public ParticleSystem smokeEffectPrefab; // Prefab hiệu ứng khói
    private ParticleSystem currentSmokeEffectLeft = null; // Hiệu ứng khói đang được kích hoạt
    private ParticleSystem currentSmokeEffectRight = null; // Hiệu ứng khói đang được kích hoạt
    public Transform tireRightPosition;
    public Transform tireLeftPosition;
    public bool cantDestroy = false;
    public TrailRenderer[] wheelTrails; // Thêm biến chứa Trail Renderer của bánh xe
    private float steerInput = 0; // Biến toàn cục lưu input

   
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ResetCar();
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    
    public void ResetCar()
    {
        // Reset lại các thông số của xe
        CurrentSpeed = data.MinSpeed; // Tốc độ xe về 0
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Kiểm tra nếu va chạm với vật thể nào đó
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy")) // Thay "Obstacle" bằng tag của vật thể bạn muốn
        {
            if (!cantDestroy)
            {
                GameManager.Instance.GameOver(); // Gọi hàm GameOver từ GameManager
                foreach (TrailRenderer trail in wheelTrails)
                {
                    if (trail != null)
                    {
                        trail.enabled = false;
                    }
                }
                // Cố định camera (hoặc chuyển camera qua vị trí cần thiết)
                  
                // Gọi hàm để chơi hiệu ứng nổ
                PlayExplosionEffect();
                FlyAway(); // Gọi hàm để làm xe bay tứ tung
                DisableCar(); // Gọi hàm để vô hiệu hóa xe
            }
        }
    }
    

    void PlayExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            StartCoroutine(ExplosionSequence());
        }
    }

    // 🔥 Coroutine để lặp lại hiệu ứng nổ 3 lần với kích thước nhỏ dần
    IEnumerator ExplosionSequence()
    {
        float scaleFactor = 1f; // Kích thước ban đầu
        float delayBetweenExplosions = 0.7f; // Thời gian giữa các lần nổ

        for (int i = 0; i < 3; i++) // Lặp lại 3 lần
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, transform.rotation);

            // Giảm kích thước mỗi lần nổ (0.8, 0.6, 0.4)
            explosion.transform.localScale *= scaleFactor;

            AudioManager.Instance.playSFX("Explosion"); // Phát âm thanh nổ 

            scaleFactor *= 0.7f; // Giảm kích thước xuống 80% mỗi lần
            yield return new WaitForSeconds(delayBetweenExplosions); // Đợi trước khi tạo nổ tiếp theo
        }
    }
    void FlyAway()
    {
        rb.isKinematic = false; // Bật lại vật lý
        rb.useGravity = true; // Đảm bảo có trọng lực
        rb.constraints = RigidbodyConstraints.None; // Gỡ bỏ mọi ràng buộc

        // ✅ Lực bay lên trời mạnh
        Vector3 launchForce = new Vector3(
            Random.Range(-2f, 2f),  // Một chút lực ngang
            Random.Range(30f, 40f), // Lực hướng lên trời mạnh
            Random.Range(-2f, 2f)   // Một chút lực ngang
        );

        rb.AddForce(launchForce, ForceMode.Impulse);

        // ✅ Xoay mạnh để giật ngửa
        Vector3 flipTorque = new Vector3(
            Random.Range(10f, 20f),  // Xoay quanh trục X
            Random.Range(-6f, 6f),  // Một chút xoay ngang
            Random.Range(-20f, -30f) // Xoay mạnh để giật ngửa về sau
        );

        rb.AddTorque(flipTorque, ForceMode.Impulse);
    }
    void DisableCar()
    {
        
        isDisabled = true; // Đặt trạng thái xe bị vô hiệu hóa
        rb.linearVelocity = Vector3.zero; // Dừng chuyển động
        rb.angularVelocity = Vector3.zero; // Dừng xoay
        CurrentSpeed = 0; // Reset tốc độ
        MoveForce = Vector3.zero; // Dừng mọi lực tác động
    }
    void Update()
    {
        if (isDisabled) return;
        HandleInput();
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDisabled) return;
        if (transform.position.y < -5)
        {
            GameManager.Instance.GameOver();
            DisableCar();
        }   
        // Nếu xe đang quá chậm, tăng nhẹ tốc độ để đảm bảo không bị đứng yên
        if (rb.linearVelocity.magnitude < 1f)
        {
            rb.AddForce(transform.forward * 5f, ForceMode.Acceleration);
        }
        HandleMovement(); // Giờ `steerInput` có thể dùng trong đây
        HandleDrift(); // Thêm drift vào di chuyển
    }
    void HandleInput()
    {
        steerInput = Input.GetAxis("Horizontal"); // Lưu input từ bàn phím

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                {
                    if (touch.position.x < Screen.width / 2)
                    {
                        steerInput = -1;
                    }
                    else
                    {
                        steerInput = 1;
                    
                    }
                }
            }
        }
        if(steerInput != 0) StartSmokeEffect();
        else StopSmokeEffect();
    }

    void HandleMovement()
    {
        if (steerInput != 0)
        {
            CurrentSpeed -= data.Deceleration * Time.deltaTime;
            CurrentSpeed = Mathf.Max(CurrentSpeed, data.MinSpeed);
        }
        else
        {
            CurrentSpeed += data.Acceleration * Time.deltaTime;
            CurrentSpeed = Mathf.Min(CurrentSpeed, data.MaxSpeed);
        }

        Vector3 forwardVelocity = transform.forward * CurrentSpeed;
        Vector3 sidewaysVelocity = transform.right * Vector3.Dot(rb.linearVelocity, transform.right);

        // Giảm dần lực drift theo thời gian để tránh giật ngang
        sidewaysVelocity *= Mathf.Lerp(1f, 0.1f, Time.deltaTime * data.DriftSmooth);

        // Kết hợp chuyển động thẳng và drift
        rb.linearVelocity = forwardVelocity + sidewaysVelocity;

        // Điều chỉnh hướng lái
        float targetTilt = steerInput * data.TiltAngle;
        CurrentTilt = Mathf.Lerp(CurrentTilt, targetTilt, Time.deltaTime * 1f);
        currentSteerAngle += steerInput * CurrentSpeed * data.SteerAngle * Time.deltaTime;

        // Cập nhật rotation
        Quaternion tiltRotation = Quaternion.Euler(0, 0, CurrentTilt);
        Quaternion steerRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        rb.MoveRotation(steerRotation * tiltRotation); // Dùng MoveRotation thay vì thay đổi transform.rotation
    } 
    void HandleDrift()
    {
        if (Mathf.Abs(steerInput) > 0.1f) // Nếu đang bẻ lái
        {
            // Tính hướng drift ngược với hướng của xe
            Vector3 driftDirection = transform.right * -steerInput;

            // Điều chỉnh độ trượt theo tốc độ hiện tại
            float driftStrength = data.DriftFactor * rb.linearVelocity.magnitude * 0.1f;

            // Thêm lực drift vào xe để làm trượt bánh xe
            rb.AddForce(driftDirection * driftStrength, ForceMode.Acceleration);

            // Giảm ma sát ngang để xe trượt mượt hơn
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, transform.forward * CurrentSpeed, data.DriftSmooth * Time.deltaTime);
        }
    }
    public void StartSmokeEffect()
    {
        if (currentSmokeEffectLeft == null) // Kiểm tra nếu chưa có hiệu ứng nào đang chạy
        {
            currentSmokeEffectLeft = Instantiate(smokeEffectPrefab, tireLeftPosition.transform.position, Quaternion.identity);
            currentSmokeEffectLeft.transform.parent = tireLeftPosition.transform; // Để hiệu ứng di chuyển cùng object này
            currentSmokeEffectLeft.Play(); // Bắt đầu hiệu ứng
        }
        if (currentSmokeEffectRight == null) // Kiểm tra nếu chưa có hiệu ứng nào đang chạy
        {
            currentSmokeEffectRight = Instantiate(smokeEffectPrefab, tireRightPosition.transform.position, Quaternion.identity);
            currentSmokeEffectRight.transform.parent = tireRightPosition.transform; // Để hiệu ứng di chuyển cùng object này
            currentSmokeEffectRight.Play(); // Bắt đầu hiệu ứng
        }
    }

    public void StopSmokeEffect()
    {
        if (currentSmokeEffectLeft != null)
        {
            currentSmokeEffectLeft.Stop(); // Dừng hiệu ứng
            Destroy(currentSmokeEffectLeft.gameObject, 2f); // Xóa sau 2 giây để hoàn tất hiệu ứng tắt dần
            currentSmokeEffectLeft = null; // Reset biến
        }
        if (currentSmokeEffectRight != null)
        {
            currentSmokeEffectRight.Stop(); // Dừng hiệu ứng
            Destroy(currentSmokeEffectRight.gameObject, 2f); // Xóa sau 2 giây để hoàn tất hiệu ứng tắt dần
            currentSmokeEffectRight = null; // Reset biến
        }
    }

}