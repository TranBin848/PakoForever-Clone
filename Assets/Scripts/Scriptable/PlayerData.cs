using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    // Settings
    public float Acceleration;  // Tốc độ gia tốc
    public float Deceleration; // Tốc độ giảm tốc
    public float MaxSpeed;     // Tốc độ tối đa
    public float MinSpeed;     // Tốc độ khi quay
    public float TiltAngle;    // Góc nghiêng của xe khi quay
    public float SteerAngle;   // Góc xoay
    public float DriftFactor;  // Điều chỉnh mức drift
    public float DriftSmooth;    // Mức mượt mà khi chuyển từ drift về bám đường
}
