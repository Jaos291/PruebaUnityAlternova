using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContainer;

    private LeaderBoardManager leaderboardManager;

    void Start()
    {
        leaderboardManager = FindObjectOfType<LeaderBoardManager>();
        DisplayLeaderboard();
    }

    public void DisplayLeaderboard()
    {
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        List<LeaderboardEntry> entries = leaderboardManager.GetEntries();
        foreach (LeaderboardEntry entry in entries)
        {
            GameObject entryObject = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            entryObject.transform.Find("NameText").GetComponent<Text>().text = entry.playerName;
            entryObject.transform.Find("ScoreText").GetComponent<Text>().text = entry.score.ToString("F2");
        }
    }

}
