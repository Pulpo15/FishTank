using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {
    #region Macros
    const int MAX_POSITION = 1;
    const int MIN_POSITION = 0;
    #endregion

    public float speed;
    public Transform[] points;

    private Vector3 targetPosition;

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

    private void Start() {
        targetPosition = GetTargetPosition();
    }

    void Update() {
        if(targetPosition != null) {
            if(transform.position == targetPosition) targetPosition = GetTargetPosition();
            else {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                    speed * Time.deltaTime);
                transform.LookAt(targetPosition);
            }
        }
    }
}
