using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SearchingGuy : MonoBehaviour
{
    [Header("巡邏路線（順時針 4 點）")]
    [Tooltip("若提供 Transform，會直接使用其位置，否則使用 Local 偏移建立方形路線。")]
    [SerializeField] private Transform[] patrolPoints = new Transform[4];
    [SerializeField] private bool useLocalSquareOffsets = true;
    [SerializeField] private Vector2[] squareOffsets = new Vector2[4]
    {
        new Vector2(-2f, 2f),
        new Vector2(2f, 2f),
        new Vector2(2f, -2f),
        new Vector2(-2f, -2f)
    };

    [Header("移動參數")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float arriveThreshold = 0.1f;

    [Header("偵測 & 追逐")]
    [SerializeField] private float detectionRadius = 4f;
    [SerializeField] private float investigateDelay = 2f;
    [SerializeField] private float chaseMemoryDuration = 1.5f;

    [Header("懷疑值")]
    [SerializeField] private float suspicionCooldown = 2f;

    [Header("參考物件")]
    [SerializeField] private PlayerControl player;

    private Rigidbody2D rb;
    private Vector2[] cachedPatrolPoints;
    private int patrolIndex;
    private bool isInvestigating;
    private bool isChasing;
    private float chaseMemoryTimer;
    private float lastSuspicionTime = -999f;
    private Coroutine investigateCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerControl>();
        }

        BuildPatrolPoints();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        bool playerCharging = player.IsChargingAttack;
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (playerCharging && distanceToPlayer <= detectionRadius && !isChasing && investigateCoroutine == null)
        {
            investigateCoroutine = StartCoroutine(InvestigateThenChase());
        }

        if (isChasing)
        {
            MoveTowards(player.transform.position, chaseSpeed);

            if (playerCharging)
            {
                chaseMemoryTimer = chaseMemoryDuration;
            }
            else
            {
                chaseMemoryTimer -= Time.deltaTime;
                if (chaseMemoryTimer <= 0f)
                {
                    ResetChase();
                }
            }

            return;
        }

        if (isInvestigating)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        if (cachedPatrolPoints == null || cachedPatrolPoints.Length == 0)
        {
            return;
        }

        Vector2 target = cachedPatrolPoints[patrolIndex];
        MoveTowards(target, patrolSpeed);

        if (Vector2.Distance(transform.position, target) <= arriveThreshold)
        {
            patrolIndex = (patrolIndex + 1) % cachedPatrolPoints.Length;
        }
    }

    private IEnumerator InvestigateThenChase()
    {
        isInvestigating = true;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(investigateDelay);

        isInvestigating = false;
        isChasing = true;
        chaseMemoryTimer = chaseMemoryDuration;
        investigateCoroutine = null;
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 current = transform.position;
        Vector2 direction = target - current;

        if (direction.sqrMagnitude <= 0.0001f)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = direction.normalized * speed;
    }

    private void ResetChase()
    {
        isChasing = false;
        rb.velocity = Vector2.zero;
    }

    private void BuildPatrolPoints()
    {
        if (patrolPoints != null && patrolPoints.Length == 4 && patrolPoints[0] != null)
        {
            cachedPatrolPoints = new Vector2[patrolPoints.Length];
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    cachedPatrolPoints[i] = patrolPoints[i].position;
                }
            }
        }
        else if (useLocalSquareOffsets && squareOffsets != null && squareOffsets.Length == 4)
        {
            cachedPatrolPoints = new Vector2[squareOffsets.Length];
            Vector2 origin = transform.position;
            for (int i = 0; i < squareOffsets.Length; i++)
            {
                cachedPatrolPoints[i] = origin + squareOffsets[i];
            }
        }
    }

    private void TryIncreaseSuspicion()
    {
        if (player == null || !player.IsChargingAttack)
        {
            return;
        }

        if (Time.time - lastSuspicionTime < suspicionCooldown)
        {
            return;
        }

        lastSuspicionTime = Time.time;

        if (GameMgr.instance != null)
        {
            GameMgr.instance.AddDamageLevel();
        }

        ResetChase();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<PlayerControl>() != null)
        {
            TryIncreaseSuspicion();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerControl>() != null)
        {
            TryIncreaseSuspicion();
        }
    }

    private void OnDisable()
    {
        if (investigateCoroutine != null)
        {
            StopCoroutine(investigateCoroutine);
            investigateCoroutine = null;
        }

        rb.velocity = Vector2.zero;
        isInvestigating = false;
        ResetChase();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (useLocalSquareOffsets && squareOffsets != null && squareOffsets.Length == 4)
        {
            Vector3 origin = Application.isPlaying ? (Vector3)transform.position : transform.position;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < squareOffsets.Length; i++)
            {
                Vector3 from = origin + (Vector3)squareOffsets[i];
                Vector3 to = origin + (Vector3)squareOffsets[(i + 1) % squareOffsets.Length];
                Gizmos.DrawLine(from, to);
            }
        }
    }
}

