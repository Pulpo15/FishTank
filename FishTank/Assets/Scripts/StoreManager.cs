using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour {

    // *** Fish available to sell *** //
    private List<FishInventory.FishData> fishList = new List<FishInventory.FishData>();

    private List<Order> orderList = new List<Order>();
    
    struct Order {
        public BreedManager.FishType type;
        public int value;
    }

    private void GenerateOrder() {
        int randNum = Random.Range(0, fishList.Count);

        Order order = new Order();
        order.type = fishList[randNum].type;
        order.value = Random.Range(10, 50);

        orderList.Add(order);
    }

    private void SetFishStore(Image image) {
        string type = image.sprite.name;

        BreedManager.FishType fishType = (BreedManager.FishType)System.Enum.Parse(typeof(BreedManager.FishType), type);

        for(int i = 0; i < fishList.Count; i++) {
            if(fishList[i].type == fishType) return;
        }

        for(int i = 0; i < FishInventory.instance.fishList.Count; i++) {
            if(FishInventory.instance.fishList[i].type == fishType) {
                fishList.Add(FishInventory.instance.fishList[i]);
                FishInventory.instance.fishList.Remove(FishInventory.instance.fishList[i]);
                Debug.Log(FishInventory.instance.fishList[i].female);
                Debug.Log(FishInventory.instance.fishList[i].fishTankName);
            }
        }
    }

    private void Start() {
        GameEvents.instance.onFishButtonClicked += SetFishStore;
    }
}
