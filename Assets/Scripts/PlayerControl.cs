using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 5f;

    [Header("互動設定")]
    public float interactDistance = 1.5f;

    [Header("治療設置")]
    public float healDuration = 3f;
    public int healAmount = 1;
    public float searchInterval = 1f;
    
    [Header("UI")]
    public Slider attackChargeSlider; 

    [Header("垃圾圖示")]
    [Tooltip("在 Inspector 直接指派玩家身上的垃圾圖示 Sprite")]
    [SerializeField] private Sprite trashIconSpriteAsset;
    [SerializeField] private Vector2 trashIconLocalOffset = new Vector2(0f, 0.75f);
    [SerializeField] private int trashIconSortingOrder = 10;
    private SpriteRenderer trashIconSprite;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Animator anim;

    // -------- 撿拾垃圾 & 攻擊蓄力 --------
    private bool hasTrash = false;
    private bool isChargingAttack = false;
    private float attackChargeTimer = 0f;
    public float attackChargeTime = 3f; // 3 秒蓄力

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (attackChargeSlider != null)
            attackChargeSlider.value = 0;

        InitializeTrashIconSprite();
    }

    void Update()
    {
        // ----- 移動輸入 -----
        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) moveInput.y = 1f;
        if (Input.GetKey(KeyCode.S)) moveInput.y = -1f;
        if (Input.GetKey(KeyCode.A)) moveInput.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput.x = 1f;

        // 動畫控制
        if (anim != null)
        {
            anim.SetBool("IsWalking", moveInput != Vector2.zero);
        }

        // 左鍵按下 → 拾取 或 開始攻擊
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickUpTrashOrStartAttack();
        }

        // 左鍵持續按住 → 若在蓄力，就計時並更新 Slider
        if (isChargingAttack && Input.GetKey(KeyCode.E))
        {
            attackChargeTimer += Time.deltaTime;

            if (attackChargeSlider != null)
            {
                float progress = attackChargeTimer / attackChargeTime;
                attackChargeSlider.value = Mathf.Clamp01(progress);
            }

            if (attackChargeTimer >= attackChargeTime)
            {
                FinishAttack();
            }
        }

        // 取消狀態：鬆開左鍵 → 蓄力失敗
        if (Input.GetMouseButtonUp(0))
        {
            if (isChargingAttack)
            {
                CancelAttack();
            }
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    // -----------------------------------
    // ① 按下左鍵 → 先看是否拾取垃圾 → 否則才開始攻擊
    // -----------------------------------
    void TryPickUpTrashOrStartAttack()
    {
        // 如果還沒有垃圾 → 試著撿垃圾
        if (!hasTrash)
        {
            if (TryPickUpTrash())
            {
                return; // 成功撿到垃圾 → 不進入攻擊
            }
        }

        // 如果有垃圾 → 才能開始蓄力攻擊
        if (hasTrash)
        {
            StartAttackCharge();
        }
    }

    // -----------------------------------
    // ② 撿垃圾：尋找 GarbageItem（檢查附近 Collider）
    // -----------------------------------
    bool TryPickUpTrash()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactDistance);

        foreach (Collider2D hit in hits)
        {
            GarbageItem trash = hit.GetComponent<GarbageItem>();
            if (trash != null)
            {
                hasTrash = true;

                // ★ 撿到垃圾 → 顯示垃圾圖示
                if (trashIconSprite != null)
                    trashIconSprite.enabled = true;

                Destroy(trash.gameObject);
                Debug.Log("撿到垃圾！");
                return true;
            }
        }

        return false;
    }

    // -----------------------------------
    // ③ 開始蓄力攻擊
    // -----------------------------------
    void StartAttackCharge()
    {
        isChargingAttack = true;
        attackChargeTimer = 0f;

        // 確保 slider 初始為 0（可選）
        if (attackChargeSlider != null)
            attackChargeSlider.value = 0f;

        Debug.Log("開始蓄力攻擊（需 " + attackChargeTime + " 秒）");
    }

    // -----------------------------------
    // ④ 蓄力完成 → 造成傷害
    // -----------------------------------
    void FinishAttack()
    {
        isChargingAttack = false;
        attackChargeTimer = 0f;

        // 重置蓄力 Slider
        if (attackChargeSlider != null)
            attackChargeSlider.value = 0f;

        // 成功攻擊後消耗垃圾 → 隱藏垃圾圖示
        hasTrash = false;
        if (trashIconSprite != null)
            trashIconSprite.enabled = false;

        AttackNearbyFarm();

        Debug.Log("攻擊完成並造成傷害，垃圾已消耗。");
    }

    // -----------------------------------
    // ⑤ 蓄力取消
    // -----------------------------------
    void CancelAttack()
    {
        isChargingAttack = false;
        attackChargeTimer = 0f;

        // 清空蓄力 UI（Slider）
        if (attackChargeSlider != null)
            attackChargeSlider.value = 0f;

        Debug.Log("蓄力取消");
    }

    // -----------------------------------
    // （原本就有的）攻擊農田
    // -----------------------------------
    void AttackNearbyFarm()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactDistance);

        foreach (Collider2D hit in hits)
        {
            FarmTile farm = hit.GetComponent<FarmTile>();
            if (farm != null)
            {
                farm.TakeDamage(farm.farmData.damagePerHit);
                break;
            }
        }
    }

    public bool IsChargingAttack
    {
        get { return isChargingAttack; }
    }

    void InitializeTrashIconSprite()
    {
        if (trashIconSpriteAsset == null)
        {
            Debug.LogWarning("PlayerControl: 尚未在 Inspector 指派垃圾圖示 Sprite。");
        }

        if (trashIconSprite == null)
        {
            GameObject iconObj = new GameObject("TrashIconSprite");
            iconObj.transform.SetParent(transform);
            iconObj.transform.localPosition = trashIconLocalOffset;
            trashIconSprite = iconObj.AddComponent<SpriteRenderer>();
            trashIconSprite.sortingOrder = trashIconSortingOrder;
        }

        trashIconSprite.sprite = trashIconSpriteAsset;
        trashIconSprite.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
