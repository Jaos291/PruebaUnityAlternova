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

        _levelCompletedMessage.text = "LEVEL COMPLETED!";

        yield return new WaitForSeconds(3);
        GameController.Instance.saveScore.SetActive(true);
        GameController.Instance.mainThemeAudioSource.clip = _EndingTheme;
        GameController.Instance.mainThemeAudioSource.Play();
    }


    public void SaveResultsJSON()
    {
        SaveResults();

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
        string playerName = _inputField.text; // Reemplaza esto con el método adecuado para obtener el nombre del jugador

        if (playerName.Equals("") || playerName == null)
        {
            playerName = "Player";
        }

        // Añadir la entrada al leaderboard
        LeaderBoardManager leaderboardManager = FindObjectOfType<LeaderBoardManager>();
        leaderboardManager.AddEntry(playerName, score);
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

}
