using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXMgr : MonoBehaviour
{
    public static VFXMgr instance{get; private set;}

    private Dictionary<string, GameObject> vFXPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> exclusiveVFXGroups = new Dictionary<string, GameObject>(); // 互斥特效組
    private List<GameObject> allActiveVFX = new List<GameObject>(); // 追蹤所有場上的特效

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadAllParticles();
    }

    void LoadAllParticles()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("VFXs/Prefabs");
        foreach (GameObject prefab in prefabs)
        {
            if (!vFXPrefabs.ContainsKey(prefab.name))
            {
                vFXPrefabs.Add(prefab.name, prefab);
            }
        }
        Debug.Log($"載入 {vFXPrefabs.Count} 個粒子特效");
    }
    
    public GameObject Play(string particleName, Vector3 position, Quaternion rotation = default)
    {
        if (vFXPrefabs.TryGetValue(particleName, out GameObject prefab))
        {
            GameObject obj = Instantiate(prefab, position, rotation);

            return obj;
        }
        else
        {
            Debug.LogWarning($"Particle [{particleName}] 不存在！");
            return null;
        }
    }
    
    public GameObject PlayExclusive(string groupName, string particleName, Vector3 position, Quaternion rotation = default)
    {
        // 如果該組已有特效在播放，先銷毀
        if (exclusiveVFXGroups.ContainsKey(groupName) && exclusiveVFXGroups[groupName] != null)
        {
            Destroy(exclusiveVFXGroups[groupName]);
        }

        // 播放新特效
        GameObject newVFX = Play(particleName, position, rotation);
        
        // 記錄到互斥組
        if (newVFX != null)
        {
            exclusiveVFXGroups[groupName] = newVFX;
        }

        return newVFX;
    }
    
    public void ClearAllVFX()
    {
        // 銷毀所有追蹤的特效
        foreach (GameObject vfx in allActiveVFX)
        {
            if (vfx != null)
            {
                Destroy(vfx);
            }
        }
        
        // 清空列表
        allActiveVFX.Clear();
        exclusiveVFXGroups.Clear();
        
        Debug.Log("已清除所有場上特效");
    }
}
