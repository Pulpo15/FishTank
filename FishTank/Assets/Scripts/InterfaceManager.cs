using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour {
    [Header("Inventory")]
    public GameObject inventory;
    public Transform buttonsZone;

    private List<Button> buttonsList;
    private FishTankManager fishTank;

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

    public void ResetUse() {
        FishTankManager.instance.useRemover = false;
        FeedManager.instance.useFood = false;
    }

    private void Start() {
        buttonsList = new List<Button>();

        for(int i = 0; i < buttonsZone.childCount; i++) {
            Transform horizontalZone = buttonsZone.GetChild(i).transform;

            for(int j = 0; j < horizontalZone.childCount; j++) {
                Button button = horizontalZone.GetChild(j).GetComponent<Button>();
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                buttonsList.Add(button);
                button.gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab)) {
            if(inventory.activeSelf) {
                inventory.SetActive(false);
                Time.timeScale = 1f;
            } else if(!inventory.activeSelf) {
                inventory.SetActive(true);

                Time.timeScale = 0f;

                for(int i = 0; i < FishInventory.instance.fishList.Count; i++) {

                    Sprite sprite = Resources.Load<Sprite>("FishPng/" + FishInventory.instance.fishList[i].type);
                    Debug.Log(sprite);
                    buttonsList[i].image.sprite = sprite;
                    buttonsList[i].gameObject.SetActive(true);
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            ResetUse();
        }
    }
}
