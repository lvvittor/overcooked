using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    public event EventHandler<OnPlateSpawnedEventArgs> OnPlateSpawned;
    public event EventHandler<OnPlateTakenEventArgs> OnPlateTaken;
    public class OnPlateSpawnedEventArgs : EventArgs {
    }
    public class OnPlateTakenEventArgs : EventArgs {
    }

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private static float SPAWN_PLATE_TIMER = 4f;

    private float spawnPlateTimer = SPAWN_PLATE_TIMER;
    private int platesCount;
    private int maxPlatesCount = 4;

    private void Update() {
        spawnPlateTimer -= Time.deltaTime;
        if (spawnPlateTimer <= 0f) {
            if (platesCount < maxPlatesCount) {
                platesCount++;
                OnPlateSpawned?.Invoke(this, new OnPlateSpawnedEventArgs());
            }

            spawnPlateTimer = SPAWN_PLATE_TIMER;
        }
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            if (platesCount > 0) {
                platesCount--;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                OnPlateTaken?.Invoke(this, new OnPlateTakenEventArgs());
            }
        }
    }

}
