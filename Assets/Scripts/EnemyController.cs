
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    public float velocityX;
    public float velocityY;

    public GameManager manager;

    [Header("Movement")]

    [SerializeField] private float speed;
    [SerializeField] public int isfacing;
    [SerializeField] private int waitTime;
    [SerializeField] private bool loopWaypoints;

    Vector2 moveDirection;

    public Transform WaypointParent;
    private Transform[] waypoints;
    private int currentWaypointIndex;

    public bool isWaiting;


    [Header("AI")]
    [SerializeField] private bool inSight;
    [SerializeField] private bool inRange;
    [SerializeField] private float maxDistance;

    Transform target;

    private bool sawPlayer;
    private bool inFront;

    Vector3 lastSeenPos;

    [Header("Obstacle Avoidance")]
    [SerializeField] private float avoidanceDistance;
    [SerializeField] private float avoidanceAngle = 35f;
    [SerializeField] private float avoidanceWeight;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Breadcrumbs (Ritorno)")]
    [SerializeField] private float breadcrumbSpacing;
    [SerializeField] private float waitBeforeReturnTime;

    private bool isReturning;
    private bool waitingToReturn;

    private List<Vector2> breadcrumbs = new List<Vector2>();

    [Header("Damage")]
    [SerializeField] private float damage;
    [SerializeField] private bool canTakeDamage = true;


    [Header("Sounds")]
    public AudioSource spotted;


    private bool madeNoise;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.Find("Player").transform;
        anim = GetComponentInChildren<Animator>();

        manager = GameManager.instance;

        waypoints = new Transform[WaypointParent.childCount];
        sawPlayer = false;

        for (int i = 0; i < WaypointParent.childCount; i++)
        {
            waypoints[i] = WaypointParent.GetChild(i);
        }

    }


    void Update()
    {

        velocityX = rb.linearVelocity.x;
        velocityY = rb.linearVelocity.y;

        if (inRange)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, target.transform.position - transform.position);
            if (ray.collider != null)
            {
                inSight = ray.collider.CompareTag("SeePlayer");
            }
            if (inSight)
            {
                lastSeenPos = target.transform.position;
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);
            }
            if (!inSight)
            {
                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
            }
        }

        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > maxDistance)
        {
            inRange = false;
        }

        //Debug.Log(distance);


        if (inSight && inRange && inFront && !manager.gameIsOver && !manager.isPaused)
        {
            Chase();
            RecordBreadcrumb();
            sawPlayer = true;
            isReturning = false;

            if (!madeNoise)
            {
                spotted.PlayOneShot(spotted.clip);
                madeNoise = true;
            }

        }
        else if (!inRange && !isWaiting && !waitingToReturn && !isReturning && !manager.isPaused || inRange && !isWaiting && !sawPlayer && !waitingToReturn && !isReturning && !manager.isPaused)
        {
            StartCoroutine(WaitToReturn());

            sawPlayer = false;
            inFront = false;
            madeNoise = false;
        }
        else if (inRange && !inSight && sawPlayer)
        {
            Search();
            StartCoroutine(DoNotScream());

        }
        else if (isWaiting || manager.gameIsOver || manager.isPaused)
        {
            rb.linearVelocity = new Vector2(0, 0) * speed;
        }
        else if (isReturning)
        {
            if (CanSeeWaypoint())
            {
                breadcrumbs.Clear();
                MoveToWaypoint();
            }
            else
            {
                ReturnViaBreadcrumbs();
            }
        }



        if (isfacing >= 5)
        {
            isfacing = 1;
        }
        CheckDirection();
        HandAnimation();

        if (anim == null)
        {
            Debug.LogError("Animator NOT FOUND on " + gameObject.name);
        }
    }

    private void Chase()
    { 
        Vector3 direction = (target.position - transform.position).normalized;
        Vector2 finalDirection = ApplyObstacleAvoidance(direction);
        rb.linearVelocity = finalDirection * speed;

        
    }

    private void Search()
    {
        Vector3 direction = (lastSeenPos - transform.position).normalized;
        Vector2 finalDirection = ApplyObstacleAvoidance(direction);
        rb.linearVelocity = finalDirection * speed;

        float searchDistance = Vector2.Distance(transform.position, lastSeenPos);

        if (searchDistance < 0.1f)
        {
            sawPlayer = false;
        }
    }

    private Vector2 ApplyObstacleAvoidance(Vector2 currentDirection)
    {
        Vector2 avoidance = Vector2.zero;
        Vector3 dir3 = currentDirection;

        Vector2 leftRay = Quaternion.Euler(0, 0, avoidanceAngle) * dir3;
        Vector2 rightRay = Quaternion.Euler(0, 0, -avoidanceAngle) * dir3;

        RaycastHit2D hitCenter = Physics2D.Raycast(transform.position, currentDirection, avoidanceDistance, obstacleMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, leftRay, avoidanceDistance, obstacleMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, rightRay, avoidanceDistance, obstacleMask);

        if (hitCenter.collider != null)
        {
            avoidance += (Vector2)(Quaternion.Euler(0, 0, -90) * dir3) * avoidanceWeight;
        }
        else
        {
            if (hitLeft.collider != null) avoidance += (Vector2)(Quaternion.Euler(0, 0, -90) * dir3) * avoidanceWeight;
            if (hitRight.collider != null) avoidance += (Vector2)(Quaternion.Euler(0, 0, 90) * dir3) * avoidanceWeight;
        }

        return (currentDirection + avoidance).normalized;
    }

    void MoveToWaypoint()
    {
        Transform moveTo = waypoints[currentWaypointIndex];
        Vector3 direction = (moveTo.position - transform.position).normalized;
        Vector2 finalDirection = ApplyObstacleAvoidance(direction);
        rb.linearVelocity = finalDirection * speed;

        if (Vector2.Distance(transform.position, moveTo.position) < 0.1f && !isWaiting)
        {
            StartCoroutine(WaitAtWaypoint());
        }

    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;
    }

    IEnumerator WaitToReturn()
    {
        waitingToReturn = true;
        isWaiting = true;

        yield return new WaitForSeconds(waitBeforeReturnTime);

        isfacing = isfacing + 1;

        yield return new WaitForSeconds(waitBeforeReturnTime);

        isfacing = isfacing + 1;

        yield return new WaitForSeconds(waitBeforeReturnTime);

        isfacing = isfacing + 1;

        yield return new WaitForSeconds(waitBeforeReturnTime);

        isfacing = isfacing + 1;

        yield return new WaitForSeconds(waitBeforeReturnTime);

        waitingToReturn = false;
        isWaiting = false;
        isReturning = true;
    }

    private bool CanSeeWaypoint()
    {

        if (waypoints == null || waypoints.Length == 0) return false;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        Vector2 direction = targetWaypoint.position - transform.position;
        float distToWaypoint = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distToWaypoint, obstacleMask);
      
        return hit.collider == null;

    }

    private void RecordBreadcrumb()
    {
        if (breadcrumbs.Count == 0 || Vector2.Distance(transform.position, breadcrumbs[breadcrumbs.Count - 1]) >= breadcrumbSpacing)
        {
            breadcrumbs.Add(transform.position);

            //Debug.Log(breadcrumbs.Count);

            if (breadcrumbs.Count > 100) breadcrumbs.RemoveAt(0);
        }
    }

    private void ReturnViaBreadcrumbs()
    {
        if (breadcrumbs.Count == 0)
        {
            //isReturning = false;
            return;
        }

        Vector2 targetPos = breadcrumbs[breadcrumbs.Count - 1];

        Vector2 desiredDirection = (targetPos - (Vector2)transform.position).normalized;
        Vector2 finalDirection = ApplyObstacleAvoidance(desiredDirection);
        rb.linearVelocity = finalDirection * speed;

        if (Vector2.Distance(transform.position, targetPos) < 0.5f)
        {
            breadcrumbs.RemoveAt(breadcrumbs.Count - 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "SeePlayer")
        {
           inFront = true;
            inRange = true;

        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Colpito Player");
            if (canTakeDamage) { 
            manager.Life = manager.Life - damage;
                manager.UpdateLifeBar();
                StartCoroutine(WaitForDamage());
            }
        }

    }

    private IEnumerator WaitForDamage() {

        canTakeDamage = false;
        yield return new WaitForSeconds(0.2f);
        canTakeDamage = true;
    }
    private void CheckDirection()
    {
        if (rb.linearVelocity.y >= 1.1f)
        {
            isfacing = 1; //Up
        }
        else if (rb.linearVelocity.y <= -1.1f)
        {
            isfacing = 2;//Down
        }
        else if (rb.linearVelocity.x >= 1)
        {
            isfacing = 3;//Right
        }
        else if (rb.linearVelocity.x <= -1)
        {
            isfacing = 4;//Left
        }


        // Debug.Log(isfacing);
    }

    void HandAnimation()
    {

        anim.SetFloat("isFacing", isfacing);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isWaiting", isWaiting);
        anim.SetBool("isDead", manager.gameIsOver);

    }

    private IEnumerator DoNotScream()
    {
        yield return new WaitForSeconds(3);
        if (!inSight)
        {
            madeNoise = false;
        }
    }
}
