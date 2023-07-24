using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishInventory : MonoBehaviour {

    // *** Structure to save fish *** //
    public struct FishData {
        public FishMovement.Age age;
        public BreedManager.FishType type;
        public bool female;
        public GameObject instance;
    }

    // *** Inventory *** //
    public List<FishData> fishList = new List<FishData>();

    public static FishInventory instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnFish(FishData fishData, bool randomSex) {
        GameObject newFish = ObjectPooler.instance.SpawnFromPool(fishData.type.ToString(), 
            FishTankSelector.fishTankManager.transform.position, Quaternion.identity);

        // *** Assign fish object to save in list  *** //
        fishData.instance = newFish;

        FishMovement fishMovement = newFish.GetComponent<FishMovement>();

        // *** Set fish Age *** //
        fishMovement.SetAge(fishData.age);

        // *** Set fish sex *** //
        fishMovement.female = fishData.female;

        if (randomSex) {
            int randNum = Random.Range(0, 2); // Random int to set sex

            // *** Set random fish sex *** //
            if(randNum == 1) fishMovement.female = false;
            else if(randNum == 0) fishMovement.female = true;

            fishData.female = fishMovement.female;
        }

        // *** Add fish to Inventory *** //
        fishList.Add(fishData);
        // *** Add fish to FishTank list *** //
        FishTankSelector.fishTankManager.fishInTank.Add(newFish);

        //Debug.Log(fishList.Count);
    }

    public void KillFish(GameObject fish) {
        // *** Search and remove fish in Inventory *** //
        for(int i = 0; i < fishList.Count; i++) {
            if (fishList[i].instance == fish) { fishList.RemoveAt(i); }
        }

        // *** Remove fish in FishTank list *** //
        FishTankSelector.fishTankManager.fishInTank.Remove(fish);
        fish.SetActive(false);
    }
}
