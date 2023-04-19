using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {

    public GameObject fishContainer;
    private List<GameObject> fishList;

    public static GameEvents instance;
    [HideInInspector]
    public bool moving;
    [HideInInspector]
    public int camPosition = 0;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        foreach (Transform fish in fishContainer.transform) {
            fishList.Add(fish.gameObject);
        }
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
            MessageRecieved("Camera position is not correct");
        }
    }

    // *** Message system *** //
    public event Action<string> onMessageRecieved;
    public void MessageRecieved(string message) { 
        Debug.Log(message);
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
}
