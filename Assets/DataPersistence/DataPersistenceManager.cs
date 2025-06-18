using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    private GameData gameData;
    public List<IDataPersistence> dataPersistencesObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Another instance of DataPersistenceManager already exists");
        }
    }
    private void Start()
    {
        Debug.Log("Data directory: " + Application.persistentDataPath);
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        this.gameData = this.dataHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("Game data was not found");
            NewGame();
        }
        foreach(IDataPersistence dataPersistence in this.dataPersistencesObjects)
        {
            dataPersistence.LoadData(this.gameData);
        }
    }
    public void SaveGame()
    {
        Debug.Log("Saving game with " + dataPersistencesObjects.Count + " persistence objects.");

        foreach (IDataPersistence dataPersistence in dataPersistencesObjects)
        {
            dataPersistence.SaveData(ref this.gameData);
            Debug.Log("Saved data: " + (gameData != null ? "Valid" : "Null"));
        }

        dataHandler.Save(gameData);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
