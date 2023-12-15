using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipiesDeliveredText;

    private void Start() {
        KitchenGameManager.Instance.OnStageChanged += KitchenGameManager_OnStageChanged;

        Hide();
    }

    private void KitchenGameManager_OnStageChanged(object sender, System.EventArgs e) {
        if(KitchenGameManager.Instance.IsGameOver()){
            Show();
            recipiesDeliveredText.SetText(DeliveryManager.Instance.GetRecipesDelivered().ToString());
        } else {
            Hide();
        }
    }

    private void Show(){
        gameObject.SetActive(true);
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
