using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTankManager : MonoBehaviour {
    #region Macros
    const string BOUNDS_TAG = "FishTankBounds";
    #endregion

    #region Public
    [HideInInspector]
    public bool useRemover;
    public bool BreedFishTank;
    public int maxWaterQuality = 100;
    public List<GameObject> fishInTank;
    #endregion

    #region Private
    private BreedManager.FishType fishType;
    private int fishId = 0;
    private int enumSize;
    private int waterQuality;
    private int fishSize = 0;
    #endregion

    // *** Update FishTank Water quality & check if fish can be alive in it *** //
    public void UpdateWaterQuality(int _waterQuality) { 
        waterQuality += _waterQuality;

        if(waterQuality < 0) waterQuality = 0;

        foreach(GameObject fish in fishInTank) {
            if(fish != null) {
                fish.GetComponent<FishMovement>().CheckWaterQuality(waterQuality);
            }
        }
    }

    //private void OnDrawGizmos() {
    //    Bounds bounds = GetFishTankBounds();

    //    for(int i = 0; i < bounds.max.x * 2 + 1; i++) {
    //        for(int j = 0; j < bounds.max.z * 2 + 1; j++) {
    //            for(int k = 0; k < bounds.max.y * 2 + 1; k++) {

    //                Vector3 pos = new Vector3(bounds.min.x + i, bounds.min.y + k, bounds.min.z + j);

    //                if(pos == obstacle.transform.position) {
    //                    Gizmos.color = new Color(1, 0, 0, 1f);
    //                } else {
    //                    Gizmos.color = new Color(0, 1, 0, 0.2f);
    //                }

    //                Gizmos.DrawCube(pos, new Vector3(1, 1, 1));
    //            }
    //        }

    //    }
    //}

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

    // *** Returns Bounds of FishTank *** //
    public List<Transform> GetFishTankBounds() {
        List<Transform> fishTankBounds = new List<Transform>();

        // *** Get child GO Bounds in FishTank *** //
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(BOUNDS_TAG)) {
                for(int x = 0; x < gameObject.transform.GetChild(i).transform.childCount; x++) {
                    fishTankBounds.Add(gameObject.transform.GetChild(i).transform.GetChild(x));
                }
            }
        }

        return fishTankBounds;
    }

    private void Start() {
        // *** Get size of enum *** //
        Array values = Enum.GetValues(typeof(BreedManager.FishType));
        enumSize = values.Length - 1;

        waterQuality = maxWaterQuality;
    }

    private void Update() {

        // *** If FishTank is not selected don't Update *** //;
        if(FishTankSelector.fishTankManager != this) return;

        #region SelectFish
        // *** Change selected fish type *** //
        if(Input.GetMouseButtonDown(4) && fishId < enumSize) {
            fishId++;
            ModifyFoodType(fishId);
        }
        if(Input.GetMouseButtonDown(3) && fishId > 0) {
            fishId--;
            ModifyFoodType(fishId);
        }
        #endregion

        #region SpawnFish
        // *** Spawn selected fish *** //
        if(Input.GetKeyDown(KeyCode.RightShift)) {
            if(fishSize < fishInTank.Count) {
                FishInventory.FishData fishData = new FishInventory.FishData();

                fishData.type = fishType;
                fishData.age = FishMovement.Age.Adult;
                fishData.female = true;

                FishInventory.instance.SpawnFish(fishData, false);

                fishSize++;
            } else GameEvents.instance.MessageRecieved("FishTank is Full");

        } else if(Input.GetKeyDown(KeyCode.RightControl) && fishSize < fishInTank.Count) {
            if(fishSize < fishInTank.Count) {
                FishInventory.FishData fishData = new FishInventory.FishData();

                fishData.type = fishType;
                fishData.age = FishMovement.Age.Adult;
                fishData.female = false;

                FishInventory.instance.SpawnFish(fishData, false);
                fishSize++;
            } else GameEvents.instance.MessageRecieved("FishTank is Full");
        } 
        #endregion

        #region RemoveDeadFish
        // *** Remove dead fish *** //
        if(Input.GetMouseButton(0) && Cursor.visible && useRemover) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            // *** Raycast mouse position *** //
            if(Physics.Raycast(ray, out RaycastHit hit)) {
                // *** Check if obect tag is a fish *** //
                if(hit.collider.CompareTag("DefaultFish")) {
                    GameObject obj = hit.collider.gameObject;
                    FishMovement fish = obj.GetComponent<FishMovement>();

                    if(fish.GetDead()) {
                        FishInventory.instance.KillFish(obj);
                    }
                }
            } else {
                GameEvents.instance.MessageRecieved("Select a fish");
            }
        }
        #endregion
    }
}
