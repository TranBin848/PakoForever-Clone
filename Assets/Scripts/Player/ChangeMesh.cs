using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WheelPositions
{
    public Vector3 frontLeftWheel;
    public Vector3 frontRightWheel;
    public Vector3 backLeftWheel;
    public Vector3 backRightWheel;
}

public class ChangeMesh : MonoBehaviour, IDataPersistence
{
    public static ChangeMesh Instance { get; private set; }
    public Mesh[] carMeshes; // Danh sách các mesh khung xe
    public Material[] carMaterials; // Danh sách các vật liệu màu xe
    public WheelPositions[] wheelPositionsForEachMesh; // Vị trí bánh xe tương ứng với mỗi mesh

    [Header("Wheel References")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform backLeftWheel;
    public Transform backRightWheel;

    private int currentMeshIndex; // Chỉ số của mesh hiện tại
    private int currentMeshColorIndex; // Chỉ số của màu hiện tại
    private MeshFilter meshFilter; // Thành phần MeshFilter để thay đổi mesh
    private MeshRenderer meshRenderer; // Thành phần MeshRenderer để thay đổi vật liệu
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void LoadData(GameData data)
    {
        this.currentMeshIndex = data.currentMesh;
        this.currentMeshColorIndex = data.currentMeshColor;
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = carMeshes[currentMeshIndex];
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = carMaterials[currentMeshColorIndex];
        UpdateWheelPositions(); // Cập nhật vị trí bánh xe khi load
    }
    public void SaveData(ref GameData data)
    {
        data.currentMesh = this.currentMeshIndex;
        data.currentMeshColor = this.currentMeshColorIndex;
    }
    void Start()
    {
        //meshFilter = GetComponent<MeshFilter>();
        //if (carMeshes.Length > 0)
        //{
        //    meshFilter.mesh = carMeshes[currentMeshIndex]; // Gán mesh ban đầu
        //}
    }
    public void NextMesh()
    {
        if (carMeshes.Length == 0) return;
        AudioManager.Instance.playSFX("Activate");
        currentMeshIndex = (currentMeshIndex + 1) % carMeshes.Length; // Chuyển đến mesh tiếp theo, quay lại đầu nếu hết
        meshFilter.mesh = carMeshes[currentMeshIndex]; // Cập nhật mesh
        UpdateWheelPositions(); // Cập nhật vị trí bánh xe
    }
    public void PreviousMesh()
    {
        if (carMeshes.Length == 0) return;
        AudioManager.Instance.playSFX("Activate");
        currentMeshIndex = (currentMeshIndex - 1 + carMeshes.Length) % carMeshes.Length; // Chuyển đến mesh trước, quay lại cuối nếu về đầu
        meshFilter.mesh = carMeshes[currentMeshIndex]; // Cập nhật mesh
        UpdateWheelPositions(); // Cập nhật vị trí bánh xe
    }
    public void NextColor()
    {
        if (carMaterials.Length == 0) return;
        AudioManager.Instance.playSFX("Activate");
        currentMeshColorIndex = (currentMeshColorIndex + 1) % carMaterials.Length; // Chuyển đến mesh tiếp theo, quay lại đầu nếu hết
        meshRenderer.material = carMaterials[currentMeshColorIndex];
    }
    public void PreviousColor()
    {
        if (carMaterials.Length == 0) return;
        AudioManager.Instance.playSFX("Activate");
        currentMeshColorIndex = (currentMeshColorIndex - 1 + carMaterials.Length) % carMaterials.Length; // Chuyển đến mesh trước, quay lại cuối nếu về đầu
        meshRenderer.material = carMaterials[currentMeshColorIndex]; // Cập nhật mesh
    }

    private void UpdateWheelPositions()
    {
        if (wheelPositionsForEachMesh == null || currentMeshIndex >= wheelPositionsForEachMesh.Length)
        {
            Debug.LogWarning("Wheel positions not configured for mesh index: " + currentMeshIndex);
            return;
        }

        WheelPositions positions = wheelPositionsForEachMesh[currentMeshIndex];

        if (frontLeftWheel != null)
            frontLeftWheel.localPosition = positions.frontLeftWheel;
        if (frontRightWheel != null)
            frontRightWheel.localPosition = positions.frontRightWheel;
        if (backLeftWheel != null)
            backLeftWheel.localPosition = positions.backLeftWheel;
        if (backRightWheel != null)
            backRightWheel.localPosition = positions.backRightWheel;
    }
}
