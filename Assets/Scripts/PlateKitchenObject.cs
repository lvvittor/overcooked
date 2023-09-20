using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjects;

    private List<KitchenObjectSO> kitchenObjects;

    private void Awake() {
        kitchenObjects = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjects.Contains(kitchenObjectSO)) {
            return false;
        }

        if (kitchenObjects.Contains(kitchenObjectSO)) {
            return false;
        } else {
            kitchenObjects.Add(kitchenObjectSO);
            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
            return true;
        }
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjects;
    }
}
