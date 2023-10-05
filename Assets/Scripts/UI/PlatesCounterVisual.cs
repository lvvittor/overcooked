using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour {
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisualFrefab;
    [SerializeField] private PlatesCounter platesCounter;

    private static float PLATE_VISUAL_OFFSET = 0.1f;

    private List<GameObject> plates;

    private void Awake() {
        plates = new List<GameObject>();
    }

    private void Start() {
        platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
        platesCounter.OnPlateTaken += PlatesCounter_OnPlateTaken;
    }

    private void PlatesCounter_OnPlateSpawned(object sender, PlatesCounter.OnPlateSpawnedEventArgs e) {
        Transform plateVisual = Instantiate(plateVisualFrefab, counterTopPoint.position + Vector3.up * PLATE_VISUAL_OFFSET * plates.Count, Quaternion.identity);

        plates.Add(plateVisual.gameObject);
    }

    private void PlatesCounter_OnPlateTaken(object sender, PlatesCounter.OnPlateTakenEventArgs e) {
        GameObject plateGO = plates[plates.Count - 1];
        plates.Remove(plateGO);
        Destroy(plateGO);
    }
}
