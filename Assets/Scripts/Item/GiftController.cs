using UnityEngine;

public class GiftController : MonoBehaviour
{
    public float chaseSpeed; // Tốc độ di chuyển về phía player
    public float bounceHeight; // Độ cao của hiệu ứng nhún lên xuống
    public float bounceSpeed; // Tốc độ nhún

    private Transform playerTransform;
    private Vector3 initialPosition;

    public GameObject smokeExplosionPrefab; // Kéo SmokeExplosion vào trong Inspector

    private void Start()
    {
        if (GameManager.Instance.isGameover)
        {
            Destroy(gameObject); // Xóa hộp quà nếu game over (người chơi chết
            return;
        }
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        initialPosition = transform.position; // Lưu vị trí ban đầu của hộp quà
    }

    private void FixedUpdate()
    {
        if (playerTransform != null)
        {
            // Di chuyển về phía người chơi
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, 0, direction.z) * chaseSpeed * Time.deltaTime;

            // Hiệu ứng nhún lên xuống
            float bounceOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            float newY = initialPosition.y + bounceOffset;

            // Giới hạn giá trị Y không được dưới 0
            newY = Mathf.Max(newY, 0f);

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // Hiệu ứng xoay hộp quà
            transform.Rotate(Vector3.up, 100f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (CarController.Instance.isDisabled) return;

        // Áp dụng buff
        GameObject buffObject = BuffManager.Instance.GetRandomBuff();
        IBuff buff = buffObject.GetComponent<IBuff>();
        if (buff != null)
            buff.Apply(other.gameObject);

        AudioManager.Instance.playSFX("Gift");

        // 🎇 Tạo hiệu ứng khói nổ mới
        if (smokeExplosionPrefab != null)
        {
            GameObject smoke = Instantiate(smokeExplosionPrefab, other.transform.position, Quaternion.identity);
        }

        // Cooldown & hủy quà
        ItemSpawner.Instance.StartGiftCooldown();
        Destroy(gameObject);
    }

}
