using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalReached : MonoBehaviour
{
    // Start is called before the first frame update
    [System.NonSerialized] public float timer;//timer to calculate simulation time
    [System.NonSerialized] public bool reached;
    [System.NonSerialized] public int collisionCount;
    void Start()
    {
        timer = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Agent"))
        {
            reached = true;
            collisionCount = other.gameObject.GetComponent<AgentController>().collisionCount;
            Debug.Log($"Goal Reached; simulation time: {timer}; collision count: {collisionCount}");
            other.gameObject.GetComponent<Rigidbody2D>().Sleep();
            Invoke("Restart", 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!reached) timer += Time.deltaTime;
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}