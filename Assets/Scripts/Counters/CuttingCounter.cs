using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {

    public static event EventHandler OnAnyCut;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    private Dictionary<KitchenObjectSO, KitchenObjectSO> recipeLookup = new Dictionary<KitchenObjectSO, KitchenObjectSO>();
    private Dictionary<KitchenObjectSO, CuttingRecipeSO> recipeSOlookup = new Dictionary<KitchenObjectSO, CuttingRecipeSO>();

    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;

    private int cuttingProgress;

    private void Start() {
        foreach (CuttingRecipeSO cuttingRecipe in cuttingRecipes) {
            recipeLookup[cuttingRecipe.input] = cuttingRecipe.output;
            recipeSOlookup[cuttingRecipe.input] = cuttingRecipe;
        }
    }
    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                if (HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    cuttingProgress = 0;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                    });
                }
            }
        } else {
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                }
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input) {
        if (recipeLookup.ContainsKey(input)) {
            return true;
        }
        return false;
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
            KitchenObjectSO output = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax) {
                cuttingProgress = 0;

                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(output, this);
            }
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input) {
        KitchenObjectSO output;
        if (recipeLookup.TryGetValue(input, out output)) {
            return output;
        }
        return null;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO input) {
        CuttingRecipeSO recipeSO;
        if (recipeSOlookup.TryGetValue(input, out recipeSO)) {
            return recipeSO;
        }
        return null;
    }
}
