using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyMgr : MonoBehaviour
{
    public static MoneyMgr instance;
    [SerializeField] private int money = 0;
    [SerializeField] private TMP_Text moneyTxt,resultMoneyTxt;
    [SerializeField] private Image bossImg;

    private Animator bossAnimator;
    private bool isPlayingAnimation = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        bossAnimator = bossImg.GetComponent<Animator>();
    }

    void Update()
    {
        UpdateMoneyUI();
    }
    
    void UpdateMoneyUI()
    {
        moneyTxt.text = money.ToString();
        resultMoneyTxt.text = $"Money : " + money;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        PlayAni("happy");
    }
    public void DeductMoney(int amount)
    {
        money -= amount;
        PlayAni("angry");
    }
    
    public void PlayAni(string paramName)
    {
        if (isPlayingAnimation) {return;}
        isPlayingAnimation = true;
        bossAnimator.SetBool(paramName, true);
        StartCoroutine(DelayTurnOff(paramName));
    }

    IEnumerator DelayTurnOff(string paramName)
    {
        yield return new WaitForSeconds(2f);
        bossAnimator.SetBool(paramName, false);
        isPlayingAnimation = false;
    }

    public void ResetMoney()
    {
        money = 0;
        isPlayingAnimation = false;
    }
}
