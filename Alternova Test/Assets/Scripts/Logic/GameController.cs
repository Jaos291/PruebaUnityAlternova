using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //Singleton
    public static GameController Instance;

    //Game References
    [SerializeField] private GridManager _gridManager;

    //Game Variables to access anywhere
    [HideInInspector] public bool canPlay = true;
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
        canPlay = true;
    }

    private void GameStateChanger()
    {
        if (!canPlay)
        {
            canPlay = true;
        }
        else
        {
            canPlay = false;
        }
    }

    public enum States
    {
        Playing,
        Paused,
        Finished
    }
}
