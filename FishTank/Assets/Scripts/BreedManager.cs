using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedManager : MonoBehaviour {

    public List<GameObject> BabyPrefabs;

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

                if(!fish.GetComponent<FishMovement>().female) {
                    if(item.male == null) { // Assign male
                        value.male = fish;
                    } else if (item.male != null) {
                        BreedFishStructure maleStruct = new BreedFishStructure();
                        maleStruct.type = fishType;
                        maleStruct.male = fish;
                        breedFish.Add(maleStruct);
                    }
                } else if (fish.GetComponent<FishMovement>().female) {
                    if(item.female == null) { // Assign female
                        value.female = fish;
                    } else if(item.female != null) {
                        BreedFishStructure femaleStruct = new BreedFishStructure();
                        femaleStruct.type = fishType;
                        femaleStruct.female = fish;
                        breedFish.Add(femaleStruct);
                    }
                }

                // *** If both GameObjects are not, then null assign each partner *** //
                if (value.male != null && value.female != null) {
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

    private void InstantiateNewFish(FishType fishType) {
        for(int i = 0; i < BabyPrefabs.Count; i++) {
            Debug.Log(BabyPrefabs[i].name + " " + fishType.ToString());
            if(BabyPrefabs[i].name == "Baby" + fishType.ToString()) {
                Instantiate(BabyPrefabs[i], Vector3.zero, Quaternion.identity);
            }
        }
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
