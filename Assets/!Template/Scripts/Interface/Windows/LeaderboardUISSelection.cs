using System.Collections;
using UnityEngine;
using TMPro;

public class LeaderboardSelectWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textRecords;
    [SerializeField] private int count;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < count; i++)
        {
            RecordData recordData = LeaderBoard.GetScore($"score_{i}");
            if(recordData != null)
                textRecords.text += $"LEVEL {i + 1} : {recordData.score}\n";
            else
                textRecords.text += $"<color=grey>LEVEL {i + 1} : NO</color>\n";
        }
    }
}