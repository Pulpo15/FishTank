using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {
    #region Macros
    const int MAX_POSITION = 1;
    const int MIN_POSITION = 0;
    #endregion

    public Transform[] points;

    private Vector3 GetTargetPosition() {
        Bounds bounds = new Bounds();

        for(int i = 0; i < points.Length; i++) {
            bounds.Encapsulate(points[i].position);
        }

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 targetPosition = new Vector3(x, y, z);

        Debug.Log(targetPosition);

        Instantiate(points[0], targetPosition, Quaternion.identity);

        return targetPosition;
    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Return)) {

        }
    }
}
