using UnityEngine;

public class GarbageSpawner : MonoBehaviour
{
    [Header("垃圾 Prefab")]
    public GameObject garbagePrefab;

    [Header("生成設定")]
    public float spawnIntervalMin = 1.5f;  // 最短生成間隔
    public float spawnIntervalMax = 4f;    // 最長生成間隔

    [Header("生成 X 軸範圍")]
    public float minX = -3f;
    public float maxX = 3f;

    [Header("生成 Y 位置（固定）")]
    public float spawnY = 5f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(wait);

            SpawnTrash();
        }
    }

    void SpawnTrash()
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        Instantiate(garbagePrefab, spawnPos, Quaternion.identity);
    }
}