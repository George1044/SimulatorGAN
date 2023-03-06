using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalReached : MonoBehaviour
{
    // Start is called before the first frame update
    float timer; //timer to calculate simulation time
    bool reached;
    void Start()
    {
        timer = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Agent"))
        {
            reached = true;
            int collisionCount = other.gameObject.GetComponent<AgentController>().collisionCount;
            Debug.Log($"Goal Reached; simulation time: {timer}; collision count: {collisionCount}");
            other.gameObject.GetComponent<Rigidbody2D>().Sleep();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!reached) timer += Time.deltaTime;
    }
}