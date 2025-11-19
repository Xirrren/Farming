using UnityEngine;

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
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) moveInput.y = 1f;
        if (Input.GetKey(KeyCode.S)) moveInput.y = -1f;
        if (Input.GetKey(KeyCode.A)) moveInput.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput.x = 1f;

        if (Input.GetMouseButtonDown(0))
        {
            AttackNearbyFarm();
        }
        
        // 控制走路動畫
        if (anim != null)
        {
            bool isWalking = moveInput != Vector2.zero;
            anim.SetBool("IsWalking", isWalking);
        }

    }

    void FixedUpdate()
    {
        rb.velocity = moveInput.normalized * moveSpeed;
    }

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}