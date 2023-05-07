using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {

    public static GameEvents instance;
    [HideInInspector]
    public bool moving;
    [HideInInspector]
    public int camPosition = 0;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        instance.onMessageRecieved += ShowMessage;
    }

    // *** Move camera to delimited points *** //
    public event Action<int> onCamKeyPressed;
    public void CamKeyPressed(int id) {
        if(onCamKeyPressed != null && !moving) {
            moving = true;
            camPosition = id;
            onCamKeyPressed(id);
        }
    }

    // *** Message system *** //
    public event Action<string> onMessageRecieved;
    public void MessageRecieved(string message) { 
        if(onMessageRecieved != null) {
            onMessageRecieved(message);
        }
    }

    // *** Update fish target when food is in FishTank *** //
    public event Action<Vector3?> onUpdateFishTarget;
    public void UpdateFishTarget(Vector3? position) {
        if(onUpdateFishTarget != null) {
            onUpdateFishTarget(position);
        }
    }

    // *** When Food instantiated assign food to target *** //
    public event Action<GameObject> onFoodInstantiate;
    public void FoodInstantiate(GameObject foodTarget) {
        if(onUpdateFishTarget != null) {
            onFoodInstantiate(foodTarget);
        }
    }

    public event Action onFoodEaten;
    public void FoodEaten() { 
        if (onFoodEaten != null) {
            onFoodEaten();
        }
    }

    // *** Assign fish in BreedManager *** //
    public event Action<BreedManager.FishType, GameObject> onSearchCouple;
    public void SearchCouple(BreedManager.FishType fish, GameObject go) { 
        if (onSearchCouple != null) {
            onSearchCouple(fish, go);
        }
    }

    // *** Instantiate new Fish *** //
    public event Action<BreedManager.FishType, Transform> onBreedNewFish;
    public void BreedNewFish(BreedManager.FishType fish, Transform transform) {
        if(onBreedNewFish != null) {
            onBreedNewFish(fish, transform);
        }
    }

    // *** Returns selected FishTank bounds *** //
    public event Func<Bounds> onGetFishTankBounds;
    public Bounds GetFishTankBounds() { 
        if (onGetFishTankBounds != null) {
            return onGetFishTankBounds();
        }
        Bounds bounds = new Bounds();
        return bounds;
    }

    private void ShowMessage(string message) {
        Debug.Log(message);
    }
}
