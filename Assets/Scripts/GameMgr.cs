using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;
    
    [Header("遊戲時長")]
    private float gameTime = 0f;
    [SerializeField] private float maxGameTime = 120f;
    [SerializeField] private Slider gameTimeSlider;
    [SerializeField] private GameObject bG;
    [SerializeField] private Color bGDayTimeColor = Color.white,bGNightTimeColor = Color.red;

    private SpriteRenderer bgSprite;
    
    [Header("懷疑值0/3")]
    [SerializeField] private int damageLevel = 0;
    [SerializeField] private Image[] damageImgs;
    [SerializeField] private Sprite damagedSprite;
    [SerializeField] private Sprite normalSprite;
    
    [Header("遊戲結束")]
    [SerializeField] private TMP_Text resultResonTxt;
    [SerializeField] private Button continueBtn;

    private void Awake()
    {
        instance = this;
        bgSprite = bG.GetComponent<SpriteRenderer>();

    }
    

    private void Start()
    {
        continueBtn.onClick.AddListener(BackToStart);
        Reset();
        AudioMgr.instance.PlayBGM("Game-BGM",3f);
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
        float t = gameTime / maxGameTime; 

        if (t > 0.5f)
        {
            float lerpValue = (t - 0.5f) / 0.5f;  // 0 → 1 （代表後半段）
            bgSprite.color = Color.Lerp(bGDayTimeColor, bGNightTimeColor, lerpValue);
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

        AudioMgr.instance.PlaySFX("Increased suspicion");

        for (int i = 0; i < damageImgs.Length; i++)
        {
            if (i < damageLevel)
            {
                damageImgs[i].sprite = damagedSprite;
            }
            else
            {
                damageImgs[i].sprite = normalSprite;
            }
        }

        // --- 只有最新那一格播放放大動畫 ---
        int index = damageLevel - 1;

        if (index >= 0 && index < damageImgs.Length)
        {
            var img = damageImgs[index].transform;

            img.localScale = Vector3.one;
            
            img.DOScale(1.8f, 0.1f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    // 左右搖晃 1 秒（只在 X 軸晃動）
                    img.DOShakePosition(
                            duration: 1f,
                            strength: new Vector2(30f, 0f), // 左右搖動幅度
                            vibrato: 10,
                            randomness: 0
                        )
                        .OnComplete(() =>
                        {
                            // 回到原大小
                            img.DOScale(1f, 0.25f).SetEase(Ease.OutQuad);
                        });
                });
        }
    }


    public void Reset()
    {
        gameTime = 0f;
        damageLevel = 0;
        
        bgSprite.color = bGDayTimeColor;
 
        MoneyMgr.instance.ResetMoney();
        
        if (gameTimeSlider != null)
        {
            gameTimeSlider.minValue = 0f;
            gameTimeSlider.maxValue = maxGameTime;
            gameTimeSlider.value = 0f;
        }
    
        for (int i = 0; i < damageImgs.Length; i++)
        {
            damageImgs[i].sprite = normalSprite;
        }
        
    }
    
    void EndGame()
    {
        GameStateMgr.instance.ChangeState(GameState.GameResult);
        AudioMgr.instance.PlaySFX("Settlement");
    }
    
    void BackToStart()
    {
        Reset();
        GameStateMgr.instance.ChangeState(GameState.StartMenu);
    }
    
}
