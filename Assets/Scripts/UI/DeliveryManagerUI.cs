using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static DeliveryManager;

public class DeliveryManagerUI : MonoBehaviour {
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);

    }

    private void Start() {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeDelivered += DeliveryManager_OnRecipeDelivered;
        DeliveryManager.Instance.OnRecipeTimeout += DeliveryManager_OnRecipeTimeout;

        UpdateVisual(new List<float>());
    }

    private void DeliveryManager_OnRecipeDelivered(object sender, RecipeEventArgs e) {
        UpdateVisual(e.Timers);
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, RecipeEventArgs e) {
        UpdateVisual(e.Timers);
    }

    private void DeliveryManager_OnRecipeTimeout(object sender, RecipeEventArgs e) {
        UpdateVisual(e.Timers);
    }

    private void UpdateVisual(List<float> timers) {
        foreach (Transform child in container) {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        List<RecipeSO> waitingRecipes = DeliveryManager.Instance.GetWaitingRecipeSOList();
        for (int i = 0; i < waitingRecipes.Count; i++)
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            DeliveryManagerSingleUI singleUI = recipeTransform.GetComponent<DeliveryManagerSingleUI>();
            singleUI.SetRecipeSO(waitingRecipes[i]);

            // Check if there is a timer for this recipe
            if (i < timers.Count)
            {
                singleUI.SetTimer(timers[i]);
            }
        }
    }
}
