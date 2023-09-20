using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress {

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Cooking,
        Cooked,
        Burned
    }

    private State currentState;

    [SerializeField] private CookingRecipeSO[] cookingRecipes;
    [SerializeField] private BurningRecipeSO[] burningRecipes;

    private Dictionary<KitchenObjectSO, KitchenObjectSO> recipeLookup = new Dictionary<KitchenObjectSO, KitchenObjectSO>();
    private Dictionary<KitchenObjectSO, CookingRecipeSO> recipeSOlookup = new Dictionary<KitchenObjectSO, CookingRecipeSO>();

    private Dictionary<KitchenObjectSO, KitchenObjectSO> burningRecipeLookup = new Dictionary<KitchenObjectSO, KitchenObjectSO>();
    private Dictionary<KitchenObjectSO, BurningRecipeSO> burningRecipeSOlookup = new Dictionary<KitchenObjectSO, BurningRecipeSO>();

    private float cookingTimer = 0f;
    private float burningTimer = 0f;
    private CookingRecipeSO cookingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start() {
        currentState = State.Idle;
        foreach (CookingRecipeSO cookingRecipe in cookingRecipes) {
            recipeLookup[cookingRecipe.input] = cookingRecipe.output;
            recipeSOlookup[cookingRecipe.input] = cookingRecipe;
        }
        foreach (BurningRecipeSO burningRecipe in burningRecipes) {
            burningRecipeLookup[burningRecipe.input] = burningRecipe.output;
            burningRecipeSOlookup[burningRecipe.input] = burningRecipe;
        }
    }

    private void Update() {
        if (HasKitchenObject()) {
            switch (currentState) {
            case State.Idle:
                    break;
            case State.Cooking:
                    cookingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = cookingTimer / cookingRecipeSO.cookingTimerMax
                    });
                    if (cookingTimer >= cookingRecipeSO.cookingTimerMax) {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(cookingRecipeSO.output, this);
                        currentState = State.Cooked;
                        burningTimer = 0f;
                        burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                    }
                    break;
            case State.Cooked:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });
                    if (burningTimer >= burningRecipeSO.burningTimerMax) {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                        currentState = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }
                    break;
            case State.Burned:
                    break;

            }
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cookingRecipeSO = GetCookingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    currentState = State.Cooking;
                    cookingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = cookingTimer / cookingRecipeSO.cookingTimerMax
                    });
                } else if (HasBurningRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    currentState = State.Cooked;
                    burningTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });
                }
            }
        } else {
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();

                        currentState = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }
                }
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);

                currentState = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs { state = currentState });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO input) {
        if (recipeLookup.ContainsKey(input)) {
            return true;
        }
        return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO input) {
        KitchenObjectSO output;
        if (recipeLookup.TryGetValue(input, out output)) {
            return output;
        }
        return null;
    }

    private CookingRecipeSO GetCookingRecipeSOWithInput(KitchenObjectSO input) {
        CookingRecipeSO recipeSO;
        if (recipeSOlookup.TryGetValue(input, out recipeSO)) {
            return recipeSO;
        }
        return null;
    }

    private bool HasBurningRecipeWithInput(KitchenObjectSO input) {
        if (burningRecipeLookup.ContainsKey(input)) {
            return true;
        }
        return false;
    }

    private KitchenObjectSO GetBurningOutputForInput(KitchenObjectSO input) {
        KitchenObjectSO output;
        if (burningRecipeLookup.TryGetValue(input, out output)) {
            return output;
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO input) {
        BurningRecipeSO recipeSO;
        if (burningRecipeSOlookup.TryGetValue(input, out recipeSO)) {
            return recipeSO;
        }
        return null;
    }
}
