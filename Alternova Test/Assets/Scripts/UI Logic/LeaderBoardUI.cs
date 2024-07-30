using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            entryObject.GetComponent<TextMeshProUGUI>().text = entry.playerName + " - " + entry.score; 
        }
    }

}
