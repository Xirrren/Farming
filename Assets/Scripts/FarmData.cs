using UnityEngine;

[CreateAssetMenu(fileName = "New Farm Data", menuName = "Farming/Farm Data")]
public class FarmData : ScriptableObject
{
    [Header("農田階段設定")]
    public Sprite[] stageSprites;
    public float stageDuration = 10f;
    
    [Header("互動設定")]
    public int maxHealth = 3;
    public int damagePerHit = 1;
}