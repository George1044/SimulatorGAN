using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AgentController : MonoBehaviour
{

    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public int MAX_SEE_AHEAD = 10;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndofPath = false;

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
        else
        {
            reachedEndofPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        

        //obstacle avoidance:
        int layerMask = LayerMask.GetMask("High Obstacle", "Low Obstacle");
        RaycastHit2D hit;
        Debug.DrawRay(transform.position, rb.velocity.normalized * MAX_SEE_AHEAD, Color.red);
        hit = Physics2D.Raycast(transform.position, rb.velocity.normalized, MAX_SEE_AHEAD, layerMask);
        Vector2 avoidance_force = new Vector2(0,0);

        if(hit == true){
            Vector2 other = hit.transform.position;
            avoidance_force = (rb.velocity*MAX_SEE_AHEAD - other).normalized;
            avoidance_force = avoidance_force.normalized;
        }

        Vector2 force = (direction+avoidance_force) * speed * Time.deltaTime;
        rb.AddForce(force);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) currentWaypoint++;



    }
}