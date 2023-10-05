using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientIconUI : MonoBehaviour {

    [SerializeField] private Image image;
    public void SetKitchenobjectSO(KitchenObjectSO kitchenObjectSO) {
        image.sprite = kitchenObjectSO.sprite;
    }
}
