using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishInventory : MonoBehaviour {
    
    public FishTankManager TankManager;

    public struct FishData {
        public FishMovement.Age age;
        public BreedManager.FishType type;
        public bool female;
        public GameObject instance;
    }

    public List<FishData> fishList = new List<FishData>();

    public static FishInventory instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnFish(FishData fishData, bool randomSex) {
        GameObject newFish = ObjectPooler.instance.SpawnFromPool(fishData.type.ToString(), transform.position, Quaternion.identity);

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

        fishList.Add(fishData);
        TankManager.fishInTank.Add(newFish);

        //Debug.Log(fishList.Count);
    }

    public void KillFish(GameObject fish) {
        for(int i = 0; i < fishList.Count; i++) {
            if (fishList[i].instance == fish) { fishList.RemoveAt(i); }
        }
        TankManager.fishInTank.Remove(fish);
        fish.SetActive(false);
    }
}
