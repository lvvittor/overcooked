using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateRecipeVisual : MonoBehaviour {

    [Serializable]
    public struct KitchenObjectSO_GameObject {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSOGameObjects;


    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
        foreach (KitchenObjectSO_GameObject item in kitchenObjectSOGameObjects) {
            item.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        foreach (KitchenObjectSO_GameObject item in kitchenObjectSOGameObjects) {
            if (item.kitchenObjectSO == e.kitchenObjectSO) {
                item.gameObject.SetActive(true);
            }
        }
    }
}
