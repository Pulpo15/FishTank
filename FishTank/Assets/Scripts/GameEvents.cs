using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {

    public GameObject fishContainer;
    private List<FishMovement> fishList;

    public static GameEvents instance;
    [HideInInspector]
    public bool moving;
    [HideInInspector]
    public int camPosition = 0;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        fishList = new List<FishMovement>();
        for(int i = 0; i < fishContainer.transform.childCount; i++) {
            fishList.Add(fishContainer.transform.GetChild(i).GetComponent<FishMovement>());
        }
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

    // *** Instantiate food at mouse position *** //
    public event Action<int> onFoodMousePressed;
    public void FoodMousePressed(int id) {
        if(onFoodMousePressed != null && camPosition == 1 && !moving) {
            onFoodMousePressed(id);
        } else if (camPosition != 1) {
            instance.MessageRecieved("Camera position is not correct");
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

    // 
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

    public event Action<BreedManager.FishType, Transform> onBreedNewFish;
    public void BreedNewFish(BreedManager.FishType fish, Transform transform) {
        if(onBreedNewFish != null) {
            onBreedNewFish(fish, transform);
        }
    }

    private void ShowMessage(string message) {
        Debug.Log(message);
    }
}
