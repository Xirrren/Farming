using UnityEngine;

public class FarmTile : MonoBehaviour
{
    [Header("農田配置")]
    public FarmData farmData;
    
    [Header("當前狀態")]
    public int currentStage = 0;
    public int currentHealth;
    
    private SpriteRenderer spriteRenderer;
    private float timer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        currentHealth = farmData.maxHealth;
        UpdateSprite();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= farmData.stageDuration)
        {
            timer = 0f;
            currentStage++;

            if (currentStage >= farmData.stageSprites.Length)
            {
                currentStage = 0;
                currentHealth = farmData.maxHealth;
                switch (currentHealth)
                {
                    case 0:
                        MoneyMgr.instance.DeductMoney(3);
                        break;
                    case 1:
                        MoneyMgr.instance.DeductMoney(1);
                        break;
                    case 2:
                        MoneyMgr.instance.AddMoney(0);
                        break;
                    case 3:
                        MoneyMgr.instance.AddMoney(1);
                        break;
                }
            }

            UpdateSprite();
            UpdateColor();
        }
    }

    void UpdateSprite()
    {
        spriteRenderer.sprite = farmData.stageSprites[currentStage];
    }

    void UpdateColor()
    {
        // 取得原本的顏色（不動 Hue）
        Color.RGBToHSV(spriteRenderer.color, out float h, out float s, out float v);

        // 根據血量直接指定亮度
        float brightness = 1f;  // 預設滿血亮度

        if (currentHealth == 2)
            brightness = 0.8f;    // 降低 20%
        else if (currentHealth == 1)
            brightness = 0.6f;    // 降低 40%
        else if (currentHealth == 0)
            brightness = 0.4f;    // 降低 60%

        // 套用亮度變化
        spriteRenderer.color = Color.HSVToRGB(h, s, brightness);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        
        UpdateColor();
    }
    public void RestoreHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > farmData.maxHealth)
        {
            currentHealth = farmData.maxHealth;
        }
    
        UpdateColor();
    }


}