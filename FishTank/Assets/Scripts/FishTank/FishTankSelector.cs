using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTankSelector : MonoBehaviour {

    public static FishTankManager fishTankManager;

    private void Update() {
        if(Time.timeScale == 0f) return;

        // *** Select FishTank *** //
        if(Input.GetMouseButton(0) && Cursor.visible && fishTankManager == null) {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if(Physics.Raycast(ray, out RaycastHit hit)) {
                if(hit.collider.CompareTag("FishTank")) {
                    GameObject obj = hit.collider.gameObject;

                    fishTankManager = obj.GetComponent<FishTankManager>();
                    GameEvents.instance.FishTankUpdated();
                }
            } else {
                GameEvents.instance.MessageRecieved("Select a fishtank");
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            fishTankManager = null;
            GameEvents.instance.FishTankRemoved();
        }
    }

    private void UpdateFishTank() {

    }
}
