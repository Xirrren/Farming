using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopUI : MonoBehaviour
{
    [SerializeField]private Button stopBtn,leaveBtn,saveBtn;
    [SerializeField]private GameObject stopScreen;

    void Start()
    {
        stopScreen.SetActive(false);
        stopBtn.onClick.AddListener(ToggleStop);
        saveBtn.onClick.AddListener(ToggleStop);
        leaveBtn.onClick.AddListener(delegate
        {
            GameMgr.instance.Reset();
            ToggleStop();
            GameStateMgr.instance.ChangeState(GameState.StartMenu);
        });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleStop();
        }

        if (stopScreen.activeInHierarchy)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void ToggleStop()
    {
        stopScreen.SetActive(!stopScreen.activeSelf);
    }
}
