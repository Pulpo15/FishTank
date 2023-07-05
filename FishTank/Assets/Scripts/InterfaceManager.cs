using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceManager : MonoBehaviour {
    public void UseFood() {
        if(!FeedManager.instance.useFood) {
            FeedManager.instance.useFood = true;
            FishTankManager.instance.useRemover = false;
        }
        else if(FeedManager.instance.useFood) {
            FeedManager.instance.useFood = false;
            FishTankManager.instance.useRemover = true;
        }
    }

    public void UseRemover() {
        if(!FishTankManager.instance.useRemover) {
            FishTankManager.instance.useRemover = true;
            FeedManager.instance.useFood = false;
        }
        else if(FishTankManager.instance.useRemover) {
            FishTankManager.instance.useRemover = false;
            FeedManager.instance.useFood = true;
        }
    }
}
