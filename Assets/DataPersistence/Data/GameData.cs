using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameData
{
    public float bestScore;
    public float bestDist;
    public int bestCrash;
    public int currentMesh;
    public int currentMeshColor;
    public GameData()
    {
        this.bestScore = 0f;
        this.bestDist = 0f;
        this.bestCrash = 0;
        this.currentMesh = 0;
        this.currentMeshColor = 0;
    }
}
