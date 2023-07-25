using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour {

    public FishTankManager breedFishTank;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.P)) {
            for(int i = 0; i < breedFishTank.fishInTank.Count; i++) {
                for(int j = 0; j < FishInventory.instance.fishList.Count; j++) {
                    if(breedFishTank.fishInTank[i] == FishInventory.instance.fishList[j].instance) {
                        GameEvents.instance.SearchCouple(FishInventory.instance.fishList[j].type, breedFishTank.fishInTank[i]);
                    }

                }
            }
        }
    }

}
