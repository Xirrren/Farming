using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;
    [Header("遊戲時長")]
    private float gameTime = 0f;
    [SerializeField] private float maxGameTime = 120f;
    [SerializeField] private Slider gameTimeSlider;
    
    [Header("懷疑值0/3")]
    [SerializeField] private int damageLevel = 0;
    [SerializeField] private Image[] damageImgs;
    
    [Header("遊戲結束")]
    [SerializeField] private TMP_Text resultResonTxt;
    [SerializeField] private Button continueBtn;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Reset();
        continueBtn.onClick.AddListener(BackToStart);
    }

    void Update()
    {
        if (GameStateMgr.instance.currentState != GameState.InGame)
        {return;}
        if (gameTime < maxGameTime)
        {
            gameTime += Time.deltaTime;

            if (gameTimeSlider != null)
            {
                gameTimeSlider.value = gameTime;
            }

            if (gameTime >= maxGameTime)
            {
                gameTime = maxGameTime;
                resultResonTxt.text = "Time up!";
                EndGame();
            }
        }

        if (damageLevel >= 3)
        {
            Invoke("EndGame",.5f);
            resultResonTxt.text = "Suspicion level is full.";
        }
    }
    
    

    public void AddDamageLevel()
    {
        damageLevel++;
        damageLevel = Mathf.Clamp(damageLevel, 0, damageImgs.Length);

        for (int i = 0; i < damageImgs.Length; i++)
        {
            if (i < damageLevel)
            {
                damageImgs[i].gameObject.SetActive(true);
            }
            else
            {
                damageImgs[i].gameObject.SetActive(false);
            }
        }
    }

    public void Reset()
    {
        gameTime = 0f;
        damageLevel = 0;
        
        MoneyMgr.instance.ResetMoney();
        
        if (gameTimeSlider != null)
        {
            gameTimeSlider.minValue = 0f;
            gameTimeSlider.maxValue = maxGameTime;
            gameTimeSlider.value = 0f;
        }
    
        for (int i = 0; i < damageImgs.Length; i++)
        {
            damageImgs[i].gameObject.SetActive(false);
        }
    }
    
    void EndGame()
    {
        GameStateMgr.instance.ChangeState(GameState.GameResult);
    }
    
    void BackToStart()
    {
        Reset();
        GameStateMgr.instance.ChangeState(GameState.StartMenu);
    }
    
}
