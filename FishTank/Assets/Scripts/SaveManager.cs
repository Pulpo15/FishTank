using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        // *** Inventory *** //
        List<FishInventory.FishData> fishList = new List<FishInventory.FishData>();

        // *** Search saved data in file *** //
        if(ES3.KeyExists("FishList")) {
            // *** Assign data to list *** //
            fishList = (List<FishInventory.FishData>)ES3.Load("FishList");
        }

        // *** Spawn every fish *** //
        for(int i = 0; i < fishList.Count; i++) {
            FishInventory.instance.SpawnFish(fishList[i], false);
        }

        GameEvents.instance.MessageRecieved("FishList loaded, count: " + FishInventory.instance.fishList.Count);
    }

    // *** Save data when application is closed *** //
    private void OnApplicationQuit() {
        ES3.Save("FishList", FishInventory.instance.fishList);

        GameEvents.instance.MessageRecieved("FishList saved, count: " + FishInventory.instance.fishList.Count);
    }
}
