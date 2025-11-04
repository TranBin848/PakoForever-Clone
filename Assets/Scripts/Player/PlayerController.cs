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
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        ResetCar();
        InitializeSmokeEffects();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            if (!cantDestroy)
            {
                GameManager.Instance.GameOver();
                SetTrailEnabled(false);
                PlayExplosionEffect();
                FlyAway();
                DisableCar();
            }
        }
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
        HandleMovement(); // Giờ `steerInput` có thể dùng trong đây
        HandleDrift(); // Thêm drift vào di chuyển
    }
    void HandleInput()
    {
        steerInput = Input.GetAxis("Horizontal"); // Lưu input từ bàn phím

        if (Input.touchCount > 0)
        {
            steerInput = 0;
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
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
        // Kích hoạt hiệu ứng khói dựa trên input
        if (Mathf.Abs(steerInput) > 0.1f)
        {
            StartSmokeEffect();
            // Kích hoạt Trail Renderer khi xe đang drift (có thể dùng tốc độ ngang/drift)
            SetTrailEnabled(true);
        }
        else
        {
            StopSmokeEffect();
            SetTrailEnabled(false);
        }
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

    // --- UTILITIES & EFFECTS ---
    public void ResetCar()
    {
        CurrentSpeed = data.MinSpeed;
        currentSteerAngle = transform.rotation.eulerAngles.y;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        SetTrailEnabled(false);
    }
    private void SetTrailEnabled(bool enabledState)
    {
        foreach (TrailRenderer trail in wheelTrails)
        {
            if (trail != null)
            {
                trail.enabled = enabledState;
            }
        }
    }
    void InitializeSmokeEffects()
    {
        if (smokeEffectPrefab == null) return;

        // Khởi tạo Particle Systems (Làm con của vị trí bánh xe)
        currentSmokeEffectLeft = Instantiate(smokeEffectPrefab, tireLeftPosition.position, Quaternion.identity, tireLeftPosition);
        currentSmokeEffectRight = Instantiate(smokeEffectPrefab, tireRightPosition.position, Quaternion.identity, tireRightPosition);

        // Dừng tất cả ngay lập tức và đảm bảo không có particle cũ
        currentSmokeEffectLeft.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        currentSmokeEffectRight.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    public void StartSmokeEffect()
    {
        if (currentSmokeEffectLeft != null && !currentSmokeEffectLeft.isPlaying)
        {
            currentSmokeEffectLeft.Play();
        }
        if (currentSmokeEffectRight != null && !currentSmokeEffectRight.isPlaying)
        {
            currentSmokeEffectRight.Play();
        }
    }
    public void StopSmokeEffect()
    {
        if (currentSmokeEffectLeft != null && currentSmokeEffectLeft.isPlaying)
        {
            currentSmokeEffectLeft.Stop();
        }
        if (currentSmokeEffectRight != null && currentSmokeEffectRight.isPlaying)
        {
            currentSmokeEffectRight.Stop();
        }
    }
    void DisableCar()
    {

        isDisabled = true; // Đặt trạng thái xe bị vô hiệu hóa
        rb.linearVelocity = Vector3.zero; // Dừng chuyển động
        rb.angularVelocity = Vector3.zero; // Dừng xoay
        CurrentSpeed = 0; // Reset tốc độ
        MoveForce = Vector3.zero; // Dừng mọi lực tác động
    }
    void PlayExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            StartCoroutine(ExplosionSequence());
        }
    }
    //Coroutine để lặp lại hiệu ứng nổ 3 lần với kích thước nhỏ dần
    IEnumerator ExplosionSequence()
    {
        float scaleFactor = 1f;
        float delayBetweenExplosions = 0.7f;

        for (int i = 0; i < 3; i++)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
            explosion.transform.localScale *= scaleFactor;

            AudioManager.Instance.playSFX("Explosion"); // Uncomment nếu bạn có AudioManager

            scaleFactor *= 0.7f;
            yield return new WaitForSeconds(delayBetweenExplosions);
        }
    }
    void FlyAway()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        Vector3 launchForce = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(30f, 40f),
            Random.Range(-2f, 2f)
        );

        rb.AddForce(launchForce, ForceMode.Impulse);

        Vector3 flipTorque = new Vector3(
            Random.Range(10f, 20f),
            Random.Range(-6f, 6f),
            Random.Range(-20f, -30f)
        );

        rb.AddTorque(flipTorque, ForceMode.Impulse);
    }
}