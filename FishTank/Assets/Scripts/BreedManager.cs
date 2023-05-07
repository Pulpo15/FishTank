using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedManager : MonoBehaviour {
    #region Macros
    #endregion

    #region Public
    #endregion

    #region Private
    #endregion

    public enum FishType {
        BlueTang,
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
        GameObject newFish = ObjectPooler.instance.SpawnFromPool(fishType.ToString(), transform.position, Quaternion.identity);

        FishMovement fishMovement = newFish.GetComponent<FishMovement>();

        int randNum = UnityEngine.Random.Range(0, 2); // Random int to set sex

        if(randNum == 1) fishMovement.female = false;
        else if(randNum == 0) fishMovement.female = true;
    }

    private void Start() {
        // *** Subscribe events *** //
        GameEvents.instance.onSearchCouple += AssignCouple;
        GameEvents.instance.onBreedNewFish += InstantiateNewFish;

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
