using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AgentController : MonoBehaviour
{

    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float MAX_SEE_AHEAD = 10f;
    public float MAX_AVOIDANCE_FORCE = 10f;
    public float CAST_RADIUS = 1f;
    public float AGENT_RADIUS = 1f;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndofPath = false;
    [System.NonSerialized]
    public int collisionCount = 0;

    Seeker seeker;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 1f, 0.5f);

    }
    public void UpdatePath()
    {
        if (reachedEndofPath) return;
        if (seeker.IsDone()) seeker.StartPath(rb.position, target.position, OnPathComplete);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            reachedEndofPath = false;
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("High Obstacle") || collision.gameObject.CompareTag("Low Obstacle"))
        {
            collisionCount++;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveDirection = rb.velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndofPath = true;
            return;
        }
        else if (currentWaypoint == (path.vectorPath.Count - 1))
        {
            reachedEndofPath = false;
            nextWaypointDistance = 0.5f;
        }
        {
            reachedEndofPath = false;
        }

        Vector2 targetPos = (Vector2)path.vectorPath[currentWaypoint];

        Vector2 direction = (targetPos - rb.position).normalized;


        //obstacle avoidance:
        int layerMask = LayerMask.GetMask("High Obstacle", "Low Obstacle");
        float dynamic_length = rb.velocity.magnitude * MAX_SEE_AHEAD;
        Debug.DrawRay(transform.position, rb.velocity.normalized * dynamic_length, Color.red);
        //RaycastHit2D hit = Physics2D.CircleCast(transform.position, CAST_RADIUS, rb.velocity.normalized, dynamic_length, layerMask);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, dynamic_length, layerMask );
        Vector2 avoidance_force = new Vector2(0, 0);
        if (hit == true)
        {

            Vector2 otherPos = hit.transform.position;
            Vector2 agentToOther = otherPos - (Vector2)rb.transform.position;
            // if (Vector2.Distance(otherPos, targetPos) <= AGENT_RADIUS)
            // {
            avoidance_force = (rb.velocity.normalized * MAX_SEE_AHEAD - agentToOther).normalized * MAX_AVOIDANCE_FORCE;
            Debug.DrawRay(otherPos, avoidance_force, Color.yellow);
            // }
        }

        Vector2 force = (direction + avoidance_force) * speed * Time.deltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) currentWaypoint++;



    }
}