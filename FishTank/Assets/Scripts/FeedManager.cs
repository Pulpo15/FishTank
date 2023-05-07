using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoBehaviour {
    #region Macros
    #endregion

    #region Public
    public Food food;
    #endregion

    #region Private
    Bounds bounds;
    private int foodId = 0;
    private int enumSize;
    #endregion

    public enum Food {
        DefaultFood,
        Special,
        Big
    }

    // *** Instantiate food at mouse position *** //
    private Vector3 FoodInstantiate() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 15f;
        Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos); // 
        objectPos.y = 5f; // FishTank Height

        // *** Instantiate food only in FishTank bounds *** //
        if (objectPos.z > bounds.min.z && objectPos.z < bounds.max.z && 
            objectPos.x > bounds.min.x && objectPos.x < bounds.max.x) {

            return objectPos;
        } else {
            GameEvents.instance.MessageRecieved("Put food only in the FishTank");
        }
        return Vector3.zero;
    }

    // *** Change selected food type *** //
    private void ModifyFoodType(int id) {
        // *** Get size of enum *** //
        Array values = Enum.GetValues(typeof(Food));

        for(int i = 0; i < values.Length; i++) {
            if(id == i) {
                food = (Food)values.GetValue(i);
            }
        }
    }

    private void Start() {
        // *** Get FishTank Bounds *** //
        bounds = GameEvents.instance.GetFishTankBounds();

        // *** Get size of enum *** //
        Array values = Enum.GetValues(typeof(Food));
        enumSize = values.Length;
    }

    private void Update() {
        // *** Instantiate food if mouse & camera position is correct *** //
        if(Input.GetMouseButtonDown(0)) {
            if(GameEvents.instance.moving || GameEvents.instance.camPosition != 1) {
                GameEvents.instance.MessageRecieved("Camera position is not correct");
            } else {
                Vector3 pos = FoodInstantiate();
                if(pos != Vector3.zero) {
                    GameObject obj = ObjectPooler.instance.SpawnFromPool(food.ToString(), pos, transform.rotation);
                    GameEvents.instance.FoodInstantiate(obj);
                }
            }
        }

        // *** Change selected food type *** //
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 && foodId < enumSize) {
            foodId++;
            ModifyFoodType(foodId);
        } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0 && foodId > 0) {
            foodId--;
            ModifyFoodType(foodId);
        }
    }
}
