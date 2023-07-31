using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{

    // *** Inventory *** //
    private List<FishInventory.FishData> fishList = new List<FishInventory.FishData>();

    // Start is called before the first frame update
    void Start() {
        if(ES3.KeyExists("FishList")) {
            fishList = (List<FishInventory.FishData>)ES3.Load("FishList");
        }

        for(int i = 0; i < fishList.Count; i++) {
            FishInventory.instance.SpawnFish(fishList[i], false);
        }

        GameEvents.instance.MessageRecieved("FishList loaded, count: " + FishInventory.instance.fishList.Count);
    }

    private void Update() {

    }

    private void OnApplicationQuit() {
        ES3.Save("FishList", FishInventory.instance.fishList);

        GameEvents.instance.MessageRecieved("FishList saved, count: " + FishInventory.instance.fishList.Count);
    }
}
