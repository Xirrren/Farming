using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestFarmland : MonoBehaviour
{
    public Button saveBtn,lv1,lv2,lv3;

    private void Start()
    {
        saveBtn.onClick.AddListener(delegate { VFXMgr.instance.Play("Save farmland", transform.position); });
        lv1.onClick.AddListener(delegate
        {
            VFXMgr.instance.PlayExclusive("Damage", "Damage1", transform.position);
        });
        
        lv2.onClick.AddListener(delegate
        {
            VFXMgr.instance.PlayExclusive("Damage", "Damage2", transform.position);
        });
        
        lv3.onClick.AddListener(delegate
        {
            VFXMgr.instance.PlayExclusive("Damage", "Damage3", transform.position);
        });
    }
}
