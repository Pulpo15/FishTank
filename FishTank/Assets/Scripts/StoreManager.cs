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

    public void SetFishStore(Image image) {
        string type = image.sprite.name;

        Debug.Log(type);
    }
}
