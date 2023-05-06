using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTankManager : MonoBehaviour {
    #region Macros
    const string BOUNDS_TAG = "FishTankBounds";
    #endregion

    // *** Returns Bounds of FishTank *** //
    private Bounds GetFishTankBounds() {

        List<Transform> fishTankBounds = new List<Transform>();

        // *** Get child GO Bounds in FishTank *** //
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            if(gameObject.transform.GetChild(i).CompareTag(BOUNDS_TAG)) {
                for(int x = 0; x < gameObject.transform.GetChild(i).transform.childCount; x++) {
                    fishTankBounds.Add(gameObject.transform.GetChild(i).transform.GetChild(x));
                }
            }
        }

        Bounds bounds = new Bounds();

        // *** Get defined vertex of FishTank *** //
        for(int i = 0; i < fishTankBounds.Count; i++) {
            bounds.Encapsulate(fishTankBounds[i].position);
        }

        return bounds;
    }

    private void Start() {
        // *** Subscribe events *** //
        GameEvents.instance.onGetFishTankBounds += GetFishTankBounds;
    }
}
