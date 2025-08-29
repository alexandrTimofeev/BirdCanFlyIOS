using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "SGames/LevelData")]
public class LevelData : ScriptableObject
{
    public int ScoreTarget = 100;
    public int Timer = 100;
    public GameObject LevelPrefab;
}