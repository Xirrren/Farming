using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    [Header("遊戲時長")]
    private float gameTime = 0f;
    [SerializeField] private float maxGameTime = 120f;
    [SerializeField] private Slider gameTimeSlider;
    
    [Header("懷疑值0/3")]
    [SerializeField] private int damageLevel = 0;
    [SerializeField] private Image[] damageImgs;
    private void Start()
    {
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

    void Update()
    {
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
                EndGame();
            }
        }

        if (damageLevel >= 3)
        {
            Invoke("EndGame",.5f);
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
    
    void EndGame()
    {
        Debug.Log("Game Over!");
        GameStateMgr.instance.ChangeState(GameState.GameResult);
    }
    
}
