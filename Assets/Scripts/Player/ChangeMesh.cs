using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMesh : MonoBehaviour, IDataPersistence
{
    public static ChangeMesh Instance { get; private set; } 
    public Mesh[] carMeshes; // Danh sách các mesh khung xe
    private int currentMeshIndex; // Chỉ số của mesh hiện tại
    private MeshFilter meshFilter; // Thành phần MeshFilter để thay đổi mesh
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
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = carMeshes[currentMeshIndex];
    }
    public void SaveData(ref GameData data)
    {
        data.currentMesh = this.currentMeshIndex;
    }
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (carMeshes.Length > 0)
        {
            meshFilter.mesh = carMeshes[currentMeshIndex]; // Gán mesh ban đầu
        }
    }
    public void NextMesh()
    {
        if (carMeshes.Length == 0) return;
        AudioManager.Instance.playSFX("Activate");
        currentMeshIndex = (currentMeshIndex + 1) % carMeshes.Length; // Chuyển đến mesh tiếp theo, quay lại đầu nếu hết
        meshFilter.mesh = carMeshes[currentMeshIndex]; // Cập nhật mesh
    }
    //public void SaveMesh()
    //{
    //    if (DataPersistenceManager.instance == null)
    //    {
    //        Debug.LogWarning("DataPersistenceManager chưa sẵn sàng, hoãn SaveMesh!");
    //        return;
    //    }

    //    // Nếu DataPersistenceManager đã sẵn sàng
    //    DataPersistenceManager.instance.SaveGame();
    //}
    public void PreviousMesh()
    {
        if (carMeshes.Length == 0) return;
        AudioManager.Instance.playSFX("Activate");
        currentMeshIndex = (currentMeshIndex - 1 + carMeshes.Length) % carMeshes.Length; // Chuyển đến mesh trước, quay lại cuối nếu về đầu
        meshFilter.mesh = carMeshes[currentMeshIndex]; // Cập nhật mesh
    }

}
