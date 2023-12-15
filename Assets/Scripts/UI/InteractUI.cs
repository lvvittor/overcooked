using System.Collections;
using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactKey;
    [SerializeField] private Player player;
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseMagnitude = 0.1f;

    private void Start()
    {
        interactKey.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
        player.OnSelectedCounterChanged += Player_OnSelectedCounterChange;
        CuttingCounter.OnAnyPlaced += CuttingCounter_OnAnyPlaced;
        CuttingCounter.OnFinishCut += CuttingCounter_OnFinishCut;
        OptionsUI.OnRebind += OptionsUI_OnRebind;
        Hide();
    }

    private void OptionsUI_OnRebind(object sender, System.EventArgs e)
    {
        interactKey.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact_Alternate);
    }

    private void CuttingCounter_OnFinishCut(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Player_OnSelectedCounterChange(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter is CuttingCounter cuttingCounter)
        {
            if (cuttingCounter.HasKitchenObject() && cuttingCounter.HasRecipeWithInput(cuttingCounter.GetKitchenObject().GetKitchenObjectSO()))
                Show();
        }
        else
        {
            Hide();
        }
    }

    private void CuttingCounter_OnAnyPlaced(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        if (cuttingCounter != null)
        {
            if (cuttingCounter.HasKitchenObject() && cuttingCounter.HasRecipeWithInput(cuttingCounter.GetKitchenObject().GetKitchenObjectSO()))
                Show();
        }
    }

    private void Update()
    {
        // Perform the cyclic effect of growing and shrinking the text
        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseMagnitude;
        interactKey.transform.localScale = new Vector3(scale, scale, 1f);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
