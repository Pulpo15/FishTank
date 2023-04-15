using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour {

    public static GameEvents instance;
    public bool moving;
    public int camPosition = 0;
    public bool foodOnFishtank;

    private void Awake() {
        instance = this;
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
        if(onFoodMousePressed != null && camPosition == 1 && !moving && !foodOnFishtank) {
            foodOnFishtank = false;
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

    public event Action<GameObject> onSetFoodTarget;
    public void SetFoodTarget(GameObject foodTarget) {
        if(onUpdateFishTarget != null) {
            onSetFoodTarget(foodTarget);
        }
    }
}
