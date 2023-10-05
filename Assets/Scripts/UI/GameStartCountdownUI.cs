using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    private void Start() {
        KitchenGameManager.Instance.OnStageChanged += KitchenGameManager_OnStageChanged;

        Hide();
    }

    private void KitchenGameManager_OnStageChanged(object sender, System.EventArgs e) {
        if(KitchenGameManager.Instance.IsCountdownToStartActive()){
            Show();
        } else {
            Hide();
        }
    }

    private void Update(){
        countdownText.SetText(KitchenGameManager.Instance.GetCountdownToStartTimer().ToString("F0"));
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
