using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTankManager : MonoBehaviour {
    #region Macros
    const string BOUNDS_TAG = "FishTankBounds";
    #endregion

    #region Public
    #endregion

    #region Private
    private BreedManager.FishType fishType;
    private int fishId = 0;
    private int enumSize;
    #endregion

    // *** Change selected food type *** //
    private void ModifyFoodType(int id) {
        // *** Get size of enum *** //
        Array values = Enum.GetValues(typeof(BreedManager.FishType));

        for(int i = 0; i < values.Length; i++) {
            if(id == i) {
                fishType = (BreedManager.FishType)values.GetValue(i);
            }
        }
    }

    // *** Prefactored Spawn Fish *** //
    private void SpawnNewFish(string tag, bool female) {
        GameObject obj = ObjectPooler.instance.SpawnFromPool(fishType.ToString(), transform.position, transform.rotation);
        obj.GetComponent<FishMovement>().SetAge(FishMovement.Age.Adult);
        obj.GetComponent<FishMovement>().female = female;
    }

    // *** Returns Bounds of FishTank *** //
    private Bounds GetFishTankBounds() {

        List<Transform> fishTankBounds = new List<Transform>();

        // *** Get child GO Bounds in FishTank *** //
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(BOUNDS_TAG)) {
                for(int x = 0; x < gameObject.transform.GetChild(i).transform.childCount; x++) {
                    fishTankBounds.Add(gameObject.transform.GetChild(i).transform.GetChild(x));
                }
            }
        }

        Bounds bounds = new Bounds();

        // *** Get defined vertex of FishTank *** //
        for(int i = 0; i < fishTankBounds.Count; i++) {
            bounds.Encapsulate(fishTankBounds[i].position);
        }

        return bounds;
    }

    private void Start() {
        // *** Subscribe events *** //
        GameEvents.instance.onGetFishTankBounds += GetFishTankBounds;

        // *** Get size of enum *** //
        Array values = Enum.GetValues(typeof(BreedManager.FishType));
        enumSize = values.Length - 1;
    }

    private void Update() {
        // *** Change selected fish type *** //
        if(Input.GetMouseButtonDown(4) && fishId < enumSize) {
            fishId++;
            ModifyFoodType(fishId);
        }
        if(Input.GetMouseButtonDown(3) && fishId > 0) {
            fishId--;
            ModifyFoodType(fishId);
        }

        // *** Spawn selected fish *** //
        if(Input.GetKeyDown(KeyCode.RightShift)) {
            SpawnNewFish(fishType.ToString(), false);
        } else if(Input.GetKeyDown(KeyCode.RightControl)) {
            SpawnNewFish(fishType.ToString(), true);
        }


    }
}
