using System.Collections;
using UnityEngine;

public class FarmAI : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3f;
    public float arrivalDistance = 0.5f;
    
    [Header("治療設置")]
    public float healDuration = 3f;
    public int healAmount = 1;
    public float searchInterval = 1f;
    
    private Rigidbody2D rb;
    private FarmTile targetFarm;
    private FarmTile[] allFarms;
    private bool isHealing = false;
    private float searchTimer = 0f;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        FindAllFarms();
    }
    
    void Update()
    {
        if (isHealing)
            return;
        
        searchTimer += Time.deltaTime;
        
        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            
            if (targetFarm == null || targetFarm.currentHealth >= targetFarm.farmData.maxHealth)
            {
                FindNextTarget();
            }
        }
        
        if (targetFarm != null)
        {
            MoveToTarget();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    
    void FindAllFarms()
    {
        allFarms = FindObjectsByType<FarmTile>(FindObjectsSortMode.None);
    }
    
    void FindNextTarget()
    {
        FarmTile closestFarm = null;
        float closestDistance = float.MaxValue;
        
        foreach (FarmTile farm in allFarms)
        {
            if (farm.currentHealth < farm.farmData.maxHealth)
            {
                float distance = Vector2.Distance(transform.position, farm.transform.position);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestFarm = farm;
                }
            }
        }
        
        targetFarm = closestFarm;
    }
    
    void MoveToTarget()
    {
        Vector2 direction = (targetFarm.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, targetFarm.transform.position);
        
        if (distance > arrivalDistance)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
            
            if (targetFarm.currentHealth < targetFarm.farmData.maxHealth)
            {
                StartCoroutine(HealFarm(targetFarm));
            }
        }
    }
    
    IEnumerator HealFarm(FarmTile farm)
    {
        isHealing = true;
        
        float elapsed = 0f;
        
        while (elapsed < healDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        farm.RestoreHealth(healAmount);
        
        isHealing = false;
        targetFarm = null;
    }
    
    void OnDrawGizmosSelected()
    {
        if (targetFarm != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetFarm.transform.position);
            Gizmos.DrawWireSphere(targetFarm.transform.position, arrivalDistance);
        }
    }
}
