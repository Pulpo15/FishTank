using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedManager : MonoBehaviour, IPooledObject {
    #region Macros
    const string DEFAULT = "DefaultFood";
    #endregion

    #region Public
    public List<GameObject> foodList;
    #endregion

    #region Private
    Bounds bounds;
    #endregion

    public void OnObjectSpawn() {

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

    private void Start() {
        // *** Get FishTank Bounds *** //
        bounds = GameEvents.instance.GetFishTankBounds();
    }

    private void Update() {
        if(Input.GetMouseButtonDown(0)) {
            if(GameEvents.instance.moving || GameEvents.instance.camPosition != 1) {
                GameEvents.instance.MessageRecieved("Camera position is not correct");
            } else {
                Vector3 pos = FoodInstantiate();
                if(pos != Vector3.zero) {
                    GameObject obj = ObjectPooler.instance.SpawnFromPool(DEFAULT, pos, transform.rotation);
                    GameEvents.instance.FoodInstantiate(obj);
                }
            }
        }
    }
}
