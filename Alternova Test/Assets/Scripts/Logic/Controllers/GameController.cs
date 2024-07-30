using UnityEngine;

public class GameController : MonoBehaviour
{
    // Singleton instance
    public static GameController Instance { get; private set; }

    // Game References
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject[] _gameObjects;

    // Game Variables accessible anywhere
    [HideInInspector] public bool canPlay;
    [HideInInspector] public GridManager gridManager;

    public AudioSource mainThemeAudioSource;
    public GameObject saveScore;
    public GameObject leaderboard;

    private void Awake()
    {
        SetupSingleton();
    }

    private void Start()
    {
        InitializeGame();
    }

    public void GameStateChanger(bool enableGame)
    {
        canPlay = enableGame;
    }

    public void SaveScore()
    {
        gridManager.SaveResultsJSON();
    }

    private void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        gridManager = _gridManager;
        canPlay = false;
    }
}
