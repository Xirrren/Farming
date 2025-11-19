using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    StartMenu,
    InGame,
    GameResult
}
public class GameStateMgr : MonoBehaviour
{
    public static GameStateMgr instance;
    
    public GameState currentState;

    [Header("UI")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject resultScreen;
    
    
    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        ChangeState(GameState.StartMenu);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // 關閉所有 UI
        startScreen?.SetActive(false);
        gameScreen?.SetActive(false);
        resultScreen?.SetActive(false);

        switch (newState)
        {
            case GameState.StartMenu:
                if (startScreen != null)
                {
                    startScreen.SetActive(true);
                }
                Time.timeScale = 0f;
                break;

            case GameState.InGame:
                if (gameScreen != null)
                {
                    gameScreen.SetActive(true);
                }
                Time.timeScale = 1f;
                break;

            case GameState.GameResult:
                if (resultScreen != null)
                {
                    resultScreen.SetActive(true);
                }
                Time.timeScale = 0f;
                break;
        }
    }
    
    public void StartGame()
    {
        ChangeState(GameState.InGame);
    }


}
