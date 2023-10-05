using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeDelivered;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailure;

    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private RecipeListSO recipeList;

    private List<RecipeSO> waitingRecipes;

    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 10f;

    private int waitingRecipeMax = 4;

    private int successfulRecipesDelivered;

    private void Awake() {
        Instance = this;
        waitingRecipes = new List<RecipeSO>();
    }

    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f) {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (waitingRecipes.Count < waitingRecipeMax) {

                RecipeSO waitingRecipe = recipeList.recipeSOList[UnityEngine.Random.Range(0, recipeList.recipeSOList.Count)];
                Debug.Log(waitingRecipe.recipeName);
                waitingRecipes.Add(waitingRecipe);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }   
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (int i = 0; i < waitingRecipes.Count; i++) {
            RecipeSO waitingRecipe = waitingRecipes[i];
            if (waitingRecipe.kitchenObjects.Count == plateKitchenObject.GetKitchenObjectSOList().Count) {
                bool plateContentsMatchRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipe.kitchenObjects) {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO) {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound) {
                        plateContentsMatchRecipe = false;
                    }
                }

                if (plateContentsMatchRecipe) {
                    successfulRecipesDelivered++;
                    waitingRecipes.RemoveAt(i);
                    Debug.Log("Recipe delivered!");
                    OnRecipeDelivered?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        Debug.Log("Player did not deilver a correct recipe!");
        OnRecipeFailure?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipeSOList() {
        return waitingRecipes;
    }

    public int GetRecipesDelivered() {
        return successfulRecipesDelivered;
    }
}
