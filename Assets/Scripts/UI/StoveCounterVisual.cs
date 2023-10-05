using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour {

    [SerializeField] private GameObject stoveOnGO;
    [SerializeField] private GameObject particlesGO;
    [SerializeField] private StoveCounter stoveCounter;

    private void Start() {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        bool showVisual = e.state == StoveCounter.State.Cooking || e.state == StoveCounter.State.Cooked;
        stoveOnGO.SetActive(showVisual);
        particlesGO.SetActive(showVisual);
    }

}
