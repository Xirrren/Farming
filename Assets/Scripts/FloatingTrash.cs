using UnityEngine;

public class FloatingTrash : MonoBehaviour
{
    [Header("速度設定")]
    public float fallSpeed = 1.2f; // 往下速度

    [Header("左右搖擺")]
    public float waveAmplitude = 0.5f;  // 左右搖幅
    public float waveFrequency = 1.5f;  // 左右搖擺頻率

    [Header("上下漂動")]
    public float floatAmplitude = 0.2f; // 上下浮動幅度
    public float floatFrequency = 2f;   // 上下浮動頻率

    private float startX;
    private float startY;
    private float timeOffset;

    void Start()
    {
        startX = transform.position.x;
        startY = transform.position.y;

        // 讓每個垃圾的波動不同步
        timeOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float t = Time.time + timeOffset;

        // 左右 S 波擺動
        float waveX = Mathf.Sin(t * waveFrequency) * waveAmplitude;

        // 上下微浮動
        float floatY = Mathf.Sin(t * floatFrequency) * floatAmplitude;

        // 往下移動
        float fallY = -fallSpeed * Time.deltaTime;

        // 更新位置
        transform.position = new Vector3(
            startX + waveX,
            transform.position.y + fallY + floatY * Time.deltaTime,
            0f
        );

        // 超出下方自動清除
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}