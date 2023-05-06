using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoBehaviour {
    #region Macros
    const int DEFAULT = 0;
    #endregion

    #region Public
    public List<GameObject> foodList;
    #endregion

    #region Private
    Bounds bounds;
    #endregion

    // *** Instantiate food at mouse position *** //
    private void FoodInstantiate(int type) {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 15f;
        Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos); // 
        objectPos.y = 5f; // FishTank Height

        // *** Instantiate food only in FishTank bounds *** //
        if (objectPos.z > bounds.min.z && objectPos.z < bounds.max.z && 
            objectPos.x > bounds.min.x && objectPos.x < bounds.max.x) {
            GameObject go = Instantiate(foodList[type], objectPos, Quaternion.identity);
            GameEvents.instance.FoodInstantiate(go);
        } else {
            GameEvents.instance.MessageRecieved("Put food only in the FishTank");
        }

    }

    private void Start() {
        // *** Subscribe events *** //
        GameEvents.instance.onFoodMousePressed += FoodInstantiate;

        // *** Get FishTank Bounds *** //
        bounds = GameEvents.instance.GetFishTankBounds();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) GameEvents.instance.FoodMousePressed(DEFAULT);
    }
}
