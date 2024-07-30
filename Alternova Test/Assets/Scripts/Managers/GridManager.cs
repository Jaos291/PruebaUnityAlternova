using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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
    [SerializeField] private AudioClip _incorrectPair;
    [SerializeField] private AudioClip _correctPair;
    [SerializeField] private AudioClip _EndingTheme;
    [SerializeField] private GameTimer _gameTimer;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI _levelCompletedMessage;
    [SerializeField] private TextMeshProUGUI _finalScore;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Sprite[] images;
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 3;
    [SerializeField] private float blockSpacing = 1.5f;

    private BlockList blockList;
    private List<GameObject> blocks = new List<GameObject>();
    private BlockBehaviour firstRevealed;
    private BlockBehaviour secondRevealed;
    private int totalClicks;
    private float startTime;
    private float finalScore;

    private void Start()
    {
        startTime = Time.time;
        LoadBlocksFromJson();

        if (ValidateBlocks())
        {
            ShuffleBlocks();
            GenerateGrid();
        }
        else
        {
            Debug.LogError("Invalid data in JSON field, please verify fields");
        }
    }

    private void LoadBlocksFromJson()
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

    private bool ValidateBlocks()
    {
        if (blockList == null || blockList.blocks == null || blockList.blocks.Count == 0)
        {
            return false;
        }

        foreach (Block block in blockList.blocks)
        {
            if (block == null || block.R < 1 || block.C < 1 || block.number < 0 || block.number > 9)
            {
                return false;
            }
        }
        return true;
    }

    private void ShuffleBlocks()
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

    private void GenerateGrid()
    {
        foreach (Block block in blockList.blocks)
        {
            GameObject blockObject = Instantiate(blockPrefab);
            blockObject.transform.SetParent(this.gameObject.transform);
            BlockBehaviour blockBehaviour = blockObject.GetComponent<BlockBehaviour>();
            blockBehaviour.image.sprite = images[block.number];
            blockBehaviour.image.enabled = false;
            blockBehaviour.Setup(block.number);
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
            SFXManager.Instance.PlaySFXClip(_correctPair);
        }
        else
        {
            firstRevealed.Hide();
            secondRevealed.Hide();
            SFXManager.Instance.PlaySFXClip(_incorrectPair);
        }

        firstRevealed = null;
        secondRevealed = null;

        GameController.Instance.canPlay = true;

        // Check if the game is finished and save results
        if (IsGameFinished())
        {
            GameController.Instance.mainThemeAudioSource.clip = _EndingTheme;
            GameController.Instance.mainThemeAudioSource.Play();
            _gameTimer.isPaused = true;
            StartCoroutine(FinishSequence());
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

    private IEnumerator FinishSequence()
    {
        yield return new WaitForSeconds(1);

        foreach (Transform child in this.gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }

        finalScore = CalculateScore();

        _finalScore.text = "Your Score: " + finalScore.ToString();

        _levelCompletedMessage.text = "LEVEL COMPLETED!";

        yield return new WaitForSeconds(3);

        _levelCompletedMessage.text = "";

        GameController.Instance.leaderboard.SetActive(true);
        GameController.Instance.saveScore.SetActive(true);
        GameController.Instance.mainThemeAudioSource.clip = _EndingTheme;
        GameController.Instance.mainThemeAudioSource.Play();
    }

    private float CalculateScore()
    {
        float totalTime = Time.time - startTime;
        int pairs = blockList.blocks.Count / 2;
        float score = pairs / (float)totalClicks * totalTime;

        return score;
    }

    public void SaveResultsJSON()
    {
        float totalTime = Time.time - startTime;
        int pairs = blockList.blocks.Count / 2;
        float score = finalScore;

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

        // Request player name through input field
        string playerName = _inputField.text; // Replace this with the appropriate method to get the player name

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Player";
        }

        // Add entry to leaderboard
        LeaderBoardManager leaderboardManager = FindObjectOfType<LeaderBoardManager>();
        leaderboardManager.AddEntry(playerName, score);
    }

    //---------------------------------
    public void UnitTesting()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "blocksTest.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            blockList = JsonUtility.FromJson<BlockList>(json);
        }
        else
        {
            Debug.LogError("Cannot find blocksTest.json file!");
        }
    }
}
