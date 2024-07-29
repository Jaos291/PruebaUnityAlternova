using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LeaderBoardManager : MonoBehaviour
{
    private string leaderboardFilePath;
    private Leaderboard leaderboard;

    void Start()
    {
        leaderboardFilePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        LoadLeaderboard();
    }

    public void AddEntry(string playerName, float score)
    {
        LeaderboardEntry newEntry = new LeaderboardEntry { playerName = playerName, score = score };
        leaderboard.entries.Add(newEntry);
        leaderboard.entries.Sort((a, b) => b.score.CompareTo(a.score));
        SaveLeaderboard();
    }

    private void LoadLeaderboard()
    {
        if (File.Exists(leaderboardFilePath))
        {
            string json = File.ReadAllText(leaderboardFilePath);
            leaderboard = JsonUtility.FromJson<Leaderboard>(json);
        }
        else
        {
            leaderboard = new Leaderboard();
        }
    }

    private void SaveLeaderboard()
    {
        string json = JsonUtility.ToJson(leaderboard, true);
        File.WriteAllText(leaderboardFilePath, json);
    }

    public List<LeaderboardEntry> GetEntries()
    {
        return leaderboard.entries;
    }
}
