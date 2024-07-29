using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Singleton
    public static GameController Instance;

    //Game References
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject[] _gameObjects;

    //Game Variables to access anywhere
    [HideInInspector] public bool canPlay;
    [HideInInspector] public GridManager gridManager;
    [HideInInspector] public States state;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        gridManager = _gridManager;
        canPlay = false;
    }

    public void GameStateChanger(bool enableGame)
    {
        canPlay = enableGame;
    }

    public enum States
    {
        Playing,
        Paused,
        Finished
    }
}
