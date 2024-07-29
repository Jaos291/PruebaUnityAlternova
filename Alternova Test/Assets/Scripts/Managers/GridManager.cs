using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class Block
{
    public int R;
    public int C;
    public int number;
}

[System.Serializable]
public class BlockList
{
    public List<Block> blocks;
}

public class GridManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public Sprite[] images;
    public int rows = 4;
    public int columns = 3;
    public float blockSpacing = 1.5f;

    private BlockList blockList;
    private List<GameObject> blocks = new List<GameObject>();

    private BlockBehaviour firstRevealed;
    private BlockBehaviour secondRevealed;
    private int totalClicks;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
        LoadBlocksFromJson();
        ShuffleBlocks();
        GenerateGrid();
    }

    void LoadBlocksFromJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "blocks.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            blockList = JsonUtility.FromJson<BlockList>(json);
        }
        else
        {
            Debug.LogError("Cannot find blocks.json file!");
        }
    }

    void ShuffleBlocks()
    {
        List<int> numbers = new List<int>();
        foreach (Block block in blockList.blocks)
        {
            numbers.Add(block.number);
        }


        for (int i = 0; i < numbers.Count; i++)
        {
            int temp = numbers[i];
            int randomIndex = Random.Range(i, numbers.Count);
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }


        for (int i = 0; i < blockList.blocks.Count; i++)
        {
            blockList.blocks[i].number = numbers[i];
        }
    }

    void GenerateGrid()
    {
        foreach (Block block in blockList.blocks)
        {
            GameObject blockObject = Instantiate(blockPrefab);
            blockObject.transform.SetParent(this.gameObject.transform);
            blockObject.GetComponent<BlockBehaviour>().image.sprite = images[block.number];
            blockObject.GetComponent<BlockBehaviour>().image.enabled = false;
            blockObject.GetComponent<BlockBehaviour>().Setup(block.number);
            blocks.Add(blockObject);
        }
    }

    public void BlockRevealed(BlockBehaviour block)
    {
        totalClicks++;

        if (firstRevealed == null)
        {
            firstRevealed = block;
        }
        else if (secondRevealed == null)
        {
            secondRevealed = block;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        GameController.Instance.canPlay = false;

        yield return new WaitForSeconds(1);

        if (firstRevealed.number == secondRevealed.number)
        {
            firstRevealed.button.image.color = Color.green;
            secondRevealed.button.image.color = Color.green;
        }
        else
        {
            firstRevealed.Hide();
            secondRevealed.Hide();
        }

        firstRevealed = null;
        secondRevealed = null;

        GameController.Instance.canPlay = true;

        // Check if the game is finished and save results
        if (IsGameFinished())
        {
            Debug.Log("Game is Finished!");
            SaveResults();
        }
    }

    private bool IsGameFinished()
    {
        foreach (GameObject blockObject in blocks)
        {
            if (!blockObject.GetComponent<BlockBehaviour>().isRevealed)
            {
                return false;
            }
        }
        return true;
    }

    private void SaveResults()
    {
        float totalTime = Time.time - startTime;
        int pairs = blockList.blocks.Count / 2;

        var result = new
        {
            totalTime = totalTime,
            totalClicks = totalClicks,
            pairs = pairs,
            score = pairs / totalClicks * totalTime 
        };

        string json = JsonUtility.ToJson(result);
        string path = Path.Combine(Application.persistentDataPath, "results.json");
        File.WriteAllText(path, json);
        Debug.Log("Results saved: " + json);
    }

    private void SaveResultsJSON()
    {
        float totalTime = Time.time - startTime;
        int pairs = blockList.blocks.Count / 2;
        float score = pairs / (float)totalClicks * totalTime; 

        var result = new
        {
            totalTime = totalTime,
            totalClicks = totalClicks,
            pairs = pairs,
            score = score
        };

        string json = JsonUtility.ToJson(result);
        string path = Path.Combine(Application.persistentDataPath, "results.json");
        File.WriteAllText(path, json);
        Debug.Log("Results saved: " + json);

        // Solicitar el nombre del jugador (puede ser a través de una ventana de diálogo o un input field)
        string playerName = "Player"; // Reemplaza esto con el método adecuado para obtener el nombre del jugador

        // Añadir la entrada al leaderboard
        LeaderBoardManager leaderboardManager = FindObjectOfType<LeaderBoardManager>();
        leaderboardManager.AddEntry(playerName, score);
    }
}
