using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishMovement : MonoBehaviour, IPooledObject {
    #region Macros
    #endregion

    #region Public
    public Animator animator;
    public float minSpeed, maxSpeed;
    public float maxEatTime;
    public float maxDeadTime = 30;
    public int maxNextAge = 10;
    public int waterQuality = 0;
    public BreedManager.FishType fishType;
    public Age age = Age.Adult;
    public FeedManager.Food food = FeedManager.Food.DefaultFood;
    public bool female = false;
    [HideInInspector]
    public GameObject partner;
    #endregion

    #region Private
    private FishTankManager fishTankManager;
    private Vector3 targetPosition;
    private int nextAge = 0;
    private float randomSpeed = 2;
    private float eatTime;
    private bool updateTarget;
    private bool canEat = false;
    private bool canBreed = true;
    private GameObject foodTarget;
    private bool dead = false;
    private List<Transform> fishTankBounds;
    private int obstacleLayer;
    #endregion

    public bool GetDead() { return dead; }
    public void SetFishTank(FishTankManager _manager) { fishTankManager = _manager; }

    public enum Age {
        Baby,
        Teen,
        Adult,
        Elder
    }

    private void SetUp() {
        // *** Get FishTank Bounds *** //
        fishTankBounds = fishTankManager.GetFishTankBounds();

        // *** Check assigned bounds *** //
        if(fishTankBounds.Count < 1) {
            Debug.LogWarning("Fish Tank Bounds Unassigned : " + gameObject.name);
            gameObject.SetActive(false);
            return;
        }

        // *** Assign time to eat again *** //
        eatTime = maxEatTime;

        // *** Reset animations *** //
        animator.Play("Swim");

        // *** Restart gravity *** //
        gameObject.GetComponent<Rigidbody>().useGravity = false;

        // *** Restart trigger *** //
        gameObject.GetComponent<CapsuleCollider>().isTrigger = true;

        // *** Set fish target at the start *** //
        GetTargetPosition(null);
    }

    public void SaveSpawn(FishTankManager fishTank) {
        fishTankManager = fishTank;
        SetUp();
    }

    // *** Interface called at ObjectPooler spawn *** //
    public void OnObjectSpawn() {
        if (FishTankSelector.fishTankManager != null) {
            fishTankManager = FishTankSelector.fishTankManager;
            SetUp();
        }
    }

    // *** Set designed age *** //
    public void SetAge(Age _age) { 
        age = _age;
    }

    // *** Search closest food to fish *** //
    private Transform GetClosestFood() {
        var objects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == food.ToString());

        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach(var t in objects) {
            if(t.activeInHierarchy) {
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if(dist < minDist) {
                    tMin = t.transform;
                    minDist = dist;
                }
            }
        }
        return tMin;
    }

    // *** Check if fish can be alive in actual water quality *** //
    public void CheckWaterQuality(int _waterQuality) {
        if(_waterQuality <= waterQuality) {
            StartCoroutine(ReduceWaterQuality(_waterQuality));
        }
    }

    // *** Coroutine to kill fish *** //
    private IEnumerator ReduceWaterQuality(int _waterQuality) {
        yield return new WaitForSeconds(5f);

        if(_waterQuality <= waterQuality) {
            eatTime = maxDeadTime + 1;
        }
    }

    private void GetTargetPosition(Vector3? foodPosition) {
        // *** Get new food position *** //
        if(foodPosition != null) {
            targetPosition = (Vector3)foodPosition;
            return;
        }

        // *** Check if there is food in FishTank *** //
        if(canEat && partner == null) {
            Transform obj = GetClosestFood();
            if (obj != null) {
                SetTargetFood(obj.gameObject);
                return;
            }
        }

        #region Debug bounds
        //Debug.Log($"1 Max x: {fishTankBounds[0].position.x}, Min x: {fishTankBounds[1].position.x}");
        //Debug.Log($"2 Max y: {fishTankBounds[1].position.y}, Min y: {fishTankBounds[0].position.y}");
        //Debug.Log($"3 Max z: {fishTankBounds[1].position.z}, Min z: {fishTankBounds[0].position.z}");
        #endregion

        // *** Get size of FishTank using defined vertex *** //
        float x = Random.Range(fishTankBounds[1].position.x, fishTankBounds[0].position.x);
        float y = Random.Range(fishTankBounds[0].position.y, fishTankBounds[1].position.y);
        float z = Random.Range(fishTankBounds[0].position.z, fishTankBounds[1].position.z);

        targetPosition = new Vector3(x, y, z);
    }

    // *** If food is correct & can breed set target *** //
    private void SetTargetFood(GameObject _food) {
        if(_food.name == food.ToString() && !updateTarget && canEat) {
            foodTarget = _food;
            updateTarget = true;
        }
    }

    private void Start() {
        // *** Subscribe events *** //
        GameEvents.instance.onUpdateFishTarget += GetTargetPosition;
        GameEvents.instance.onFoodInstantiate += SetTargetFood;

        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    private void Update() {
        #region Detect obsctacle
        RaycastHit hit;
        bool obstacleDetected = Physics.Raycast(transform.position, transform.forward, out hit, 1f);

        if(obstacleDetected && hit.collider.gameObject.layer == obstacleLayer) {
            updateTarget = false;

            partner = null;

            animator.Play("Eyes_Happy"); // Reset animation
            canEat = false;
            GetTargetPosition(null);
            canEat = true;
        }
        #endregion

        #region Move to target
        if(targetPosition != null && !dead) {

            #region Move to food
            // *** If fish has eaten get new targetPosition *** //
            if(updateTarget && !foodTarget.activeSelf) {
                updateTarget = false;
                GetTargetPosition(null);
            }
            // *** If fish has not eaten follow food *** //
            else if(updateTarget) {
                targetPosition = foodTarget.transform.position;
                animator.Play("Eyes_Excited"); // Happy eyes
            }
            #endregion

            #region Move to position
            // *** If fish has reached it's target get a new one *** //
            if(transform.position == targetPosition) {
                randomSpeed = Random.Range(minSpeed, maxSpeed);
                GetTargetPosition(null);
            }
            // *** If fish has a target move towards it with random speed between assigned in editor *** //
            else {
                // *** Move fish to position *** //
                transform.position = Vector3.MoveTowards(transform.position, targetPosition,
                    randomSpeed * Time.deltaTime);

                // *** Rotate fish *** //
                Vector3 dir = targetPosition - transform.position;
                if (dir != Vector3.zero) {
                    Quaternion rot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5f * Time.deltaTime);
                }
            }
            #endregion

        } else if (targetPosition == null && !dead) { // *** targetPosition is null so assign it again *** //
            randomSpeed = Random.Range(minSpeed, maxSpeed);
            GetTargetPosition(null);
        }
        #endregion

        #region Timer
        // *** Timer to eat again *** //
        if(!canEat || !dead) {
            eatTime += Time.deltaTime;
            if(eatTime >= maxEatTime && !canEat) {
                canEat = true;
                canBreed = true;
                animator.Play("Eyes_Squint"); // Hungry eyes
            } 
            if (eatTime >= maxDeadTime && !dead) {
                dead = true;

                targetPosition = Vector3.zero;

                animator.Play("Eyes_Dead"); // Dead eyes
                animator.Play("Death"); // Death animation

                gameObject.GetComponent<Rigidbody>().useGravity = true;
                gameObject.GetComponent<CapsuleCollider>().isTrigger = false;


            }
        }
        #endregion

        // *** Set Target Position to Parter to breed *** //
        if(partner != null && canBreed && partner.activeSelf) {
            GetTargetPosition(partner.transform.position);
            animator.Play("Eyes_Excited"); // Happy eyes
        }

        if(dead) {
            eatTime += Time.deltaTime;
            if(eatTime>= 5f) {
                fishTankManager.UpdateWaterQuality(-10);
                eatTime = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        // *** Collision with Food *** //
        if(other.gameObject == foodTarget && canEat) {
            other.gameObject.SetActive(false);

            eatTime = 0; // Reset timer to eat again
            updateTarget = false;
            canEat = false;
            GetTargetPosition(null);

            animator.Play("Eyes_Happy"); // Reset animation

            nextAge++; // Var to grow

            // *** Grow fish *** //
            if(nextAge >= maxNextAge) {
                nextAge = 0;

                // *** If elder DIE *** //
                if (age == Age.Elder) {
                    eatTime = maxDeadTime + 1;
                    return;
                }

                // *** Get size of Enum(Age) to iterate and set next Age *** //
                System.Array values = System.Enum.GetValues(typeof(Age));

                for(int i = 0; i < values.Length; i++) {
                    if(age == (Age)values.GetValue(i)) {
                        age = (Age)values.GetValue(i + 1); // Set next Age
                        return;
                    }   
                }
            }

            // *** If Adult search add fish to couple *** //
            //if(age == Age.Adult) GameEvents.instance.SearchCouple(fishType, gameObject);
        } 
        // *** Collision with partner *** //
        if(other.gameObject == partner) {
            canBreed = true;
            partner = null;

            animator.Play("Eyes_Happy"); // Reset animation

            // *** If female Breed new Fish *** //
            if (female) GameEvents.instance.BreedNewFish(fishType, transform);
        }
    }
}
