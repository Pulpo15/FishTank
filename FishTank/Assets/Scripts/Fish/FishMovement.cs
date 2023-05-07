using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour, IPooledObject {
    #region Macros
    const int NEXT_AGE = 3;
    #endregion

    #region Public
    public float minSpeed, maxSpeed;
    public float maxEatTime;
    public BreedManager.FishType fishType;
    public Age age = Age.Adult;
    public FeedManager.Food food = FeedManager.Food.DefaultFood;
    public bool female = false;
    [HideInInspector]
    public GameObject partner;
    #endregion

    #region Private
    private List<GameObject> graphics;
    private Bounds bounds;
    private Vector3 targetPosition;
    private int nextAge = 0;
    private float randomSpeed = 2;
    private float eatTime;
    private bool updateTarget;
    private bool canEat = true;
    private bool canBreed = true;
    private GameObject foodTarget;
    #endregion

    public enum Age {
        Baby,
        Teen,
        Adult,
        Elder
    }

    public void OnObjectSpawn() {
        if(graphics == null) {
            // *** Get Graphics *** //
            GameObject go = gameObject.transform.GetChild(0).gameObject;
            graphics = new List<GameObject>();
            for(int i = 0; i < go.transform.childCount; i++) {
                graphics.Add(go.transform.GetChild(i).gameObject);
            }
        }

        // *** Assign time to eat again *** //
        eatTime = maxEatTime;

        // *** Set fish target at the start *** //
        GetTargetPosition(null);
    }

    // *** Set designed age and it's graphics *** //
    public void SetAge(Age _age) { 
        age = _age;

        // *** Get size of Enum(Age) to iterate and set next Age *** //
        System.Array values = System.Enum.GetValues(typeof(Age));

        for(int i = 0; i < values.Length; i++) {
            graphics[i].SetActive(false); // Disable young graphic
        }

        for(int i = 0; i < values.Length; i++) {
            if(age == (Age)values.GetValue(i)) {
                graphics[i].SetActive(true); // Enable next graphic
                return;
            }
        }
    }

    private Transform GetClosestFood() {

        return null;
    }

    private void GetTargetPosition(Vector3? foodPosition) {
        // *** 
        if(targetPosition != null && updateTarget) {
            return;
        }
        // *** Get new food position *** //
        if(foodPosition != null) {
            targetPosition = (Vector3)foodPosition;
            return;
        }
        // *** Check if there is food in FishTank *** //
        if(canEat) {
            GameObject go = GameObject.Find(food.ToString());
            if(go != null) {
                SetTargetFood(go);
                return;
            }
        }

        #region Debug bounds
        //Debug.Log($"Max x: {bounds.max.x}, Min x: {bounds.min.x}");
        //Debug.Log($"Max y: {bounds.max.y}, Min y: {bounds.min.y}");
        //Debug.Log($"Max z: {bounds.max.z}, Min z: {bounds.min.z}");
        #endregion

        // *** Get size of FishTank using defined vertex *** //
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        targetPosition = new Vector3(x, y, z);

        //Instantiate(fishTankBounds[0], targetPosition, Quaternion.identity);
    }

    // *** If food is correct & can breed set target *** //
    private void SetTargetFood(GameObject _food) {
        if(_food.name == food.ToString() && !updateTarget && canEat) {
            foodTarget = _food;
            updateTarget = true;
        }
    }

    private void Start() {
        // *** Get FishTank Bounds *** //
        bounds = GameEvents.instance.GetFishTankBounds();

        // *** Check assigned bounds *** //
        if(bounds.size == Vector3.zero) {
            Debug.LogWarning("Fish Tank Bounds Unassigned : " + gameObject.name);
            gameObject.SetActive(false);
            return;
        }

        // *** Subscribe events *** //
        GameEvents.instance.onUpdateFishTarget += GetTargetPosition;
        GameEvents.instance.onFoodInstantiate += SetTargetFood;

        OnObjectSpawn();

    }

    private void Update() {
        if(targetPosition != null) {
            // *** If fish has eaten get new targetPosition *** //
            if(updateTarget && !foodTarget.activeSelf) { 
                updateTarget = false;
                GetTargetPosition(null); 
            } 
            // *** If fish has not eaten follow food *** //
            else if(updateTarget) targetPosition = foodTarget.transform.position;

            // *** If fish has reached it's target get a new one *** //
            if(transform.position == targetPosition) {
                randomSpeed = Random.Range(minSpeed, maxSpeed);
                GetTargetPosition(null);
            }
            // *** If fish has a target move towards it with random speed between assigned in editor *** //
            else {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                    randomSpeed * Time.deltaTime);
                transform.LookAt(targetPosition);
            }            
        } else { // *** targetPosition is null so assign it again *** //
            randomSpeed = Random.Range(minSpeed, maxSpeed);
            GetTargetPosition(null);
        }

        // *** Timer to eat again *** //
        if(!canEat) {
            eatTime -= Time.deltaTime;
            if(eatTime <= 0) {
                canEat = true;
                canBreed = true;
                eatTime = maxEatTime;
            }
        }
        // *** Set Target Position to Parter to breed *** //
        if(partner != null && canBreed && partner.activeSelf) {
            GetTargetPosition(partner.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other) {
        // *** Collision with Food *** //
        if(other.gameObject == foodTarget && canEat) {
            other.gameObject.SetActive(false);
            updateTarget = false;
            canEat = false;
            GetTargetPosition(null);

            nextAge++; // Var to grow

            // *** Grow fish *** //
            if(nextAge >= NEXT_AGE) {
                nextAge = 0;

                // *** If elder DIE *** //
                if (age == Age.Elder) {
                    gameObject.SetActive(false);
                    return;
                }

                // *** Get size of Enum(Age) to iterate and set next Age *** //
                System.Array values = System.Enum.GetValues(typeof(Age));

                for(int i = 0; i < values.Length; i++) {
                    if(age == (Age)values.GetValue(i)) {
                        age = (Age)values.GetValue(i + 1); // Set next Age

                        graphics[i].SetActive(false); // Disable young graphic
                        graphics[i + 1].SetActive(true); // Enable next graphic
                        return;
                    }   
                }
            }

            // *** If Adult search add fish to couple *** //
            if(age == Age.Adult) GameEvents.instance.SearchCouple(fishType, gameObject);
        } 
        // *** Collision with partner *** //
        if(other.gameObject == partner) {
            canBreed = false;
            partner = null;
            // *** If female Breed new Fish *** //
            if (female) GameEvents.instance.BreedNewFish(fishType, transform);
        }
    }
}
