using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoBehaviour {
    #region Macros
    const int DEFAULT = 0;
    #endregion

    public List<GameObject> foodList;

    // *** Instantiate food at mouse position *** //
    private void FoodInstantiate(int type) {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 15f;
        Vector3 objectPos = Camera.main.ScreenToWorldPoint(mousePos);
        objectPos.y = 5f;
        //Debug.Log(objectPos);

        if (objectPos.z > -10f && objectPos.z < 10f && objectPos.x > -5f && objectPos.x < 5f) {
            GameObject go = Instantiate(foodList[type], objectPos, Quaternion.identity);
            GameEvents.instance.FoodInstantiate(go);
        } else {
            Debug.Log("Error");
        }

    }

    private void Start() {
        GameEvents.instance.onFoodMousePressed += FoodInstantiate;
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) GameEvents.instance.FoodMousePressed(DEFAULT);
    }
}
