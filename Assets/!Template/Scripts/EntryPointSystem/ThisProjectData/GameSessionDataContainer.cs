using System.Collections;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "GameSessionDataContainer", menuName = "SGames/GameSessionDataContainer")]
public class GameSessionDataContainer : ScriptableObject
{
    public IntContainer HealthContainer;
    public float SpeedGame = 1f;

    [Space]
    public string StandartLevelData = "Level_2";

    public GameSessionDataContainer(IntContainer healthContainer, float speedGame)
    {
        HealthContainer = healthContainer;
        SpeedGame = speedGame;
    }

    public GameSessionDataContainer Clone()
    {
        return Instantiate(this);
    }
}