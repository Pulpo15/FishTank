using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour {
    #region Macros
    const int MAX_POSITION = 1;
    const int MIN_POSITION = 0;
    #endregion

    #region Public
    public float[] speed;
    public Transform[] points;
    public string foodTag;
    #endregion

    #region Private
    private Vector3 targetPosition;
    private float randomSpeed = 2;
    private bool updateTarget;
    private GameObject foodTarget;
    #endregion

    private void GetTargetPosition(Vector3? foodPosition) {
        // *** Get new food position *** //
        if(foodPosition != null) {
            targetPosition = (Vector3)foodPosition;
            return;
        }
        // *** Check if there is food in FishTank *** //
        GameObject go = GameObject.FindGameObjectWithTag("DefaultFood");
        if(go != null) {
            targetPosition = go.transform.position;
            return;
        }

        Bounds bounds = new Bounds();

        // *** Get defined vertex of FishTank *** //
        for(int i = 0; i < points.Length; i++) {
            bounds.Encapsulate(points[i].position);
        }

        // *** Get size of FishTank using defined vertex *** //
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        targetPosition = new Vector3(x, y, z);

        Instantiate(points[0], targetPosition, Quaternion.identity);
    }

    private void SetTargetFood(GameObject food) {
        foodTarget = food;
        updateTarget = true;
    }

    private void Start() {
        // *** Subscribe events *** //
        GameEvents.instance.onUpdateFishTarget += GetTargetPosition;
        GameEvents.instance.onFoodInstantiate += SetTargetFood;

        // *** Set fish target at the start *** //
        GameEvents.instance.UpdateFishTarget(null);
    }

    void Update() {
        if(targetPosition != null) {
            if(updateTarget) targetPosition = foodTarget.transform.position;
            // *** If fish has reached it's target get a new one *** //
            if(transform.position == targetPosition) {
                randomSpeed = Random.Range(speed[0], speed[1]);
                GameEvents.instance.UpdateFishTarget(null);
            }
            // *** If fish has a target move towards it with random speed between assigned in editor *** //
            else {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                    randomSpeed * Time.deltaTime);
                transform.LookAt(targetPosition);
            }            
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag(foodTag)) {
            Destroy(other.gameObject);
            updateTarget = false;
            GameEvents.instance.UpdateFishTarget(null);
        }
    }
}
