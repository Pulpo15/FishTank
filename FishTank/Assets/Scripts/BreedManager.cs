using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedManager : MonoBehaviour {
    #region Macros
    const string FISHCONTAINER_TAG = "FishContainer";
    #endregion

    #region Public
    public List<GameObject> BabyPrefabs;
    #endregion

    #region Private
    private GameObject fishContainer;
    #endregion

    public enum FishType {
        Default,
        Clown
    }

    // *** Couple structure with type *** //
    private struct BreedFishStructure {
        public FishType type;
        public GameObject male;
        public GameObject female;
    }

    // *** Couple fish list *** //
    private List<BreedFishStructure> breedFish = new List<BreedFishStructure>();

    private void AssignCouple(FishType fishType, GameObject fish) {
        foreach(BreedFishStructure item in breedFish) {

            // *** Check Enum FishType *** //
            if (item.type == fishType) {
                // *** Assign iterator to new value so we can modify it *** //
                BreedFishStructure value = item;

                if(item.male == fish || item.female == fish) return; // If fish already in couple return;

                if(!fish.GetComponent<FishMovement>().female) {
                    if(item.male == null) { // Assign male
                        value.male = fish;
                    } else if (item.male != null) { // Current male couple is assigned, create new one //
                        BreedFishStructure maleStruct = new BreedFishStructure();
                        maleStruct.type = fishType;
                        maleStruct.male = fish;
                        breedFish.Add(maleStruct);
                    }
                } else if (fish.GetComponent<FishMovement>().female) {
                    if(item.female == null) { // Assign female
                        value.female = fish;
                    } else if(item.female != null) { // Current female couple is assigned, create new one //
                        BreedFishStructure femaleStruct = new BreedFishStructure();
                        femaleStruct.type = fishType;
                        femaleStruct.female = fish;
                        breedFish.Add(femaleStruct);
                    }
                }

                // *** If both GameObjects are not null, then assign each partner *** //
                if(value.male != null && value.female != null) {
                    value.male.GetComponent<FishMovement>().partner = value.female;
                    value.female.GetComponent<FishMovement>().partner = value.male;

                    // *** Reset data *** //
                    breedFish.Remove(item);

                    BreedFishStructure empty = new BreedFishStructure();
                    empty.type = fishType;
                    breedFish.Add(empty);
                    return;
                }

                breedFish.Remove(item); // Remove old data
                breedFish.Add(value); // Add new data

                return; // Kill loop
            }
        }
    }

    private void InstantiateNewFish(FishType fishType, Transform transform) {
        for(int i = 0; i < BabyPrefabs.Count; i++) {
            if(BabyPrefabs[i].name == fishType.ToString()) {
                GameObject newFish = Instantiate(BabyPrefabs[i], transform.position, Quaternion.identity, fishContainer.transform);

                FishMovement fishMovement = newFish.GetComponent<FishMovement>();

                int randNum = UnityEngine.Random.Range(0, 1); // Random int to set sex

                if(randNum == 1) fishMovement.female = false;
                else if (randNum == 0) fishMovement.female = true;
            }
        }
    }

    private void Start() {
        // *** Subscribe events *** //
        GameEvents.instance.onSearchCouple += AssignCouple;
        GameEvents.instance.onBreedNewFish += InstantiateNewFish;

        // *** Set Fish Container *** //
        fishContainer = GameObject.FindGameObjectWithTag(FISHCONTAINER_TAG);

        // *** Create fish couple list with size of enum *** //
        Array values = Enum.GetValues(typeof(FishType));

        for(int i = 0; i < values.Length; i++) {
            FishType fishType = (FishType)values.GetValue(i);
            
            BreedFishStructure fishStructure = new BreedFishStructure();
            fishStructure.type = fishType;

            breedFish.Add(fishStructure);
        }
    }
}
