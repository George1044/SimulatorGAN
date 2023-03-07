using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text.RegularExpressions;

public class ConvertMatrixToMap : MonoBehaviour
{

    public float timeScale;
    public Tile[] floor;
    public bool randomMap;
    public GameObject goal, agent, highObstacle, lowObstacle, actualGoal, pathfinder;
    private List<GameObject> agents = new List<GameObject>(), goals = new List<GameObject>();
    public float timeoutTimer = 2;
    public int mapNumber;
    public bool autogenerate;
    int i = 0, j = 0;
    public int minGoalDistance = 8;
    private bool failed = false;
    int HEIGHT = 32, WIDTH = 32;
    int[,] matrix;
    Tile currentTile;
    Tilemap tilemap;
    StringBuilder sb;
    string simulationResult="";
    public string mapName = "map";
    //public GameObject pathfinder, player;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScale;
        if (autogenerate) Generate("0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 \n0 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 1 1 1 0 \n0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 1 0 0 \n0 0 0 0 0 1 1 0 1 1 0 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 \n0 0 0 0 0 0 1 0 1 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 \n0 0 0 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 1 0 1 1 0 1 1 0 0 1 0 0 \n0 0 0 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 1 1 1 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 1 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 2 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 1 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 3 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 1 0 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 1 1 1 0 \n0 0 1 0 0 0 1 0 1 0 0 0 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 0 0 0 \n0 0 1 0 0 1 1 0 1 1 0 1 1 0 1 0 0 1 0 1 0 0 0 1 0 1 0 0 0 0 0 0 \n0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 0 1 0 0 0 0 0 0 \n0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 1 1 0 1 1 0 0 0 0 0 \n0 0 1 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 \n0 1 1 1 0 0 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 \n0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 \n");

    }
    void Generate()
    {
        //Time.timeScale = Time.timeScale * 100;
        if (!randomMap)
        {
            String stringName;
            string pwd = System.IO.Directory.GetCurrentDirectory();
            if (randomMap)
                stringName = pwd + "/Maps/randomMap" + mapNumber + ".txt";
            else
                stringName = pwd + "/Maps/" + mapName + mapNumber + ".txt";
            StreamReader reader1 = new StreamReader(stringName);
            StreamReader reader = new StreamReader(stringName);
            int width = reader1.ReadLine().Split(' ').Length - 1;
            String textFile = reader.ReadToEnd();
            textFile = Regex.Replace(textFile, "\n|\r", "");
            String[] numbers = (textFile.Split(' '));
            int height = numbers.Length / (width);


            matrix = new int[height, width];
            int row = -1, column = -1;
            for (int i = 0; i < numbers.Length - 1; i++)
            {
                row = (int)(i / width);
                column = (int)(i % width);
                matrix[row, column] = Int32.Parse(numbers[i]);
            }
            if (!Array.Exists(numbers, element => element == "0"))
            {
                Debug.Log("This map does not contain any empty space");
            }
            else
            if (!Array.Exists(numbers, element => element == "2"))
            {
                //place agent at random location 
                int randomX = UnityEngine.Random.Range(0, height);
                int randomY = UnityEngine.Random.Range(0, width);
                float timer = 0;
                while (matrix[(int)randomX, (int)randomY] != 0)
                {
                    timer += Time.deltaTime;
                    if (timer > timeoutTimer)
                    {
                        Debug.LogError("Generation timed out.");
                        failed = true;
                        Application.Quit();
                        break;
                    }
                    randomX = UnityEngine.Random.Range(0, height);
                    randomY = UnityEngine.Random.Range(0, width);
                }
                if (!failed) matrix[randomX, randomY] = 2; else Application.Quit();
                //place goal at random location respecting minGoalDistance
                int randomX2 = UnityEngine.Random.Range(0, height);
                int randomY2 = UnityEngine.Random.Range(0, width);
                double distance = Math.Sqrt(Math.Pow(randomX - randomX2, 2) + Math.Pow(randomY - randomY2, 2));
                timer = 0;
                while ((matrix[(int)randomX2, (int)randomY2] != 0) || (distance < minGoalDistance))
                {
                    Debug.Log(timer);
                    timer += Time.deltaTime;
                    if (timer > timeoutTimer)
                    {
                        Debug.Log(timer);
                        Debug.LogError("Generation timed out, try lowering minimum goal distance");
                        failed = true;
                        Application.Quit();
                        break;
                    }
                    randomX2 = UnityEngine.Random.Range(0, height);
                    randomY2 = UnityEngine.Random.Range(0, width);
                    distance = Math.Sqrt(Math.Pow(randomX - randomX2, 2) + Math.Pow(randomY - randomY2, 2));
                }
                if (!failed) matrix[randomX2, randomY2] = 3; else Application.Quit();
            }
            tilemap = GetComponent<Tilemap>();
            for (int y = (int)height / 2 - 1; y >= (int)-height / 2; y--)
            {
                for (int x = (int)-width / 2; x < (int)width / 2; x++)
                {

                    currentTile = floor[0];
                    if (matrix[i, j] == 0)
                    {
                        currentTile = floor[UnityEngine.Random.Range(0, (floor.Length))];
                    }
                    else if (matrix[i, j] == 1)
                    {
                        Instantiate(highObstacle, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    }
                    else if (matrix[i, j] == 4)
                    {
                        Instantiate(lowObstacle, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    }
                    else if (matrix[i, j] == 2)
                    {
                        agents.Add(Instantiate(agent, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity));
                    }
                    else if (matrix[i, j] == 3)
                    {
                        goals.Add(Instantiate(goal, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity));
                        currentTile = floor[0];
                    }

                    tilemap.SetTile(new Vector3Int(x, y, 0), currentTile);
                    j++;
                }


                j = 0;
                i++;
            }

            Invoke("Scan", 0.1f);
            /*
            int randomX = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            while (matrix[(int)randomX, (int)randomY] != 0)
            {
                randomX = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
                randomY = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            }
            agent = Instantiate(agent, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);

            int randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            while ((matrix[(int)randomX2, (int)randomY2] != 0) && (randomX2 != randomX || randomY2 != randomY))
            {
                randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
                randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            }
            goal = Instantiate(goal, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            */
            foreach (GameObject agent in agents)
            {
                agent.GetComponent<AgentController>().target = goals.FirstOrDefault().GetComponent<Transform>();
                goals.RemoveAt(0);
            }

        }
        else
        {
            int randomX = UnityEngine.Random.Range((int)-WIDTH / 2, (int)WIDTH / 2);
            int randomY = UnityEngine.Random.Range((int)-HEIGHT / 2, (int)HEIGHT / 2);
            agent = Instantiate(agent, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            currentTile = floor[0];
            tilemap.SetTile(new Vector3Int(randomX, randomY, 0), currentTile);

            int randomX2 = UnityEngine.Random.Range((int)-WIDTH / 2, (int)WIDTH / 2);
            int randomY2 = UnityEngine.Random.Range((int)-HEIGHT / 2, (int)HEIGHT / 2);
            while (randomX2 == randomX || randomY2 == randomY)
            {
                randomX2 = UnityEngine.Random.Range((int)-WIDTH / 2, (int)WIDTH / 2);
                randomY2 = UnityEngine.Random.Range((int)-HEIGHT / 2, (int)HEIGHT / 2);
            }

            goal = Instantiate(goal, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            agent.GetComponent<AgentController>().target = goal.GetComponent<Transform>();
            currentTile = floor[0];
            tilemap.SetTile(new Vector3Int(randomX2, randomY2, 0), currentTile);
            Invoke("Scan", 0.1f);
        }




    }
    public string Generate(String matrixString)
    {
        if (!randomMap)
        {
            Debug.Log(matrixString);
            StringReader reader1 = new StringReader(matrixString);
            StringReader reader = new StringReader(matrixString);
            int width = reader1.ReadLine().Split(' ').Length - 1;
            String textFile = reader.ReadToEnd();
            textFile = Regex.Replace(textFile, "\n|\r", "");
            String[] numbers = (textFile.Split(' '));
            int height = numbers.Length / (width);
            Debug.Log($"heigh{height} width{width}");
            matrix = new int[height, width];
            int row = -1, column = -1;
            for (int i = 0; i < numbers.Length - 1; i++)
            {
                row = (int)(i / width);
                column = (int)(i % width);
                matrix[row, column] = Int32.Parse(numbers[i]);
            }
            if (!Array.Exists(numbers, element => element == "0"))
            {
                Debug.Log("This map does not contain any empty space");
            }
            else
            if (!Array.Exists(numbers, element => element == "2"))
            {
                //place agent at random location 
                Debug.Log("Randomizing agent placement.");
                int randomX = UnityEngine.Random.Range(0, height);
                int randomY = UnityEngine.Random.Range(0, width);
                float timer = 0;
                while (matrix[(int)randomX, (int)randomY] != 0)
                {
                    timer += Time.deltaTime;
                    if (timer > timeoutTimer)
                    {
                        Debug.LogError("Generation timed out.");
                        failed = true;
                        Application.Quit();
                        break;
                    }
                    randomX = UnityEngine.Random.Range(0, height);
                    randomY = UnityEngine.Random.Range(0, width);
                }
                if (!failed) matrix[randomX, randomY] = 2; else Application.Quit();
                //place goal at random location respecting minGoalDistance
                int randomX2 = UnityEngine.Random.Range(0, height);
                int randomY2 = UnityEngine.Random.Range(0, width);
                double distance = Math.Sqrt(Math.Pow(randomX - randomX2, 2) + Math.Pow(randomY - randomY2, 2));
                timer = 0;
                while ((matrix[(int)randomX2, (int)randomY2] != 0) || (distance < minGoalDistance))
                {
                    timer += Time.deltaTime;
                    if (timer > timeoutTimer)
                    {
                        Debug.Log(timer);
                        Debug.LogError("Generation timed out, try lowering minimum goal distance");
                        failed = true;
                        Application.Quit();
                        break;
                    }
                    randomX2 = UnityEngine.Random.Range(0, height);
                    randomY2 = UnityEngine.Random.Range(0, width);
                    distance = Math.Sqrt(Math.Pow(randomX - randomX2, 2) + Math.Pow(randomY - randomY2, 2));
                }
                if (!failed) matrix[randomX2, randomY2] = 3; else Application.Quit();
            }
            tilemap = GetComponent<Tilemap>();
            for (int y = (int)height / 2 - 1; y >= (int)-height / 2; y--)
            {
                for (int x = (int)-width / 2; x < (int)width / 2; x++)
                {

                    currentTile = floor[0];
                    if (matrix[i, j] == 0)
                    {
                        currentTile = floor[UnityEngine.Random.Range(0, (floor.Length))];
                    }
                    else if (matrix[i, j] == 1)
                    {
                        Instantiate(highObstacle, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    }
                    else if (matrix[i, j] == 4)
                    {
                        Instantiate(lowObstacle, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    }
                    else if (matrix[i, j] == 2)
                    {
                        agents.Add(Instantiate(agent, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity));
                    }
                    else if (matrix[i, j] == 3)
                    {
                        goals.Add(Instantiate(goal, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity));
                        currentTile = floor[0];
                    }

                    tilemap.SetTile(new Vector3Int(x, y, 0), currentTile);
                    j++;
                }


                j = 0;
                i++;
            }

            Invoke("Scan", 0.1f);
            /*
            int randomX = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            while (matrix[(int)randomX, (int)randomY] != 0)
            {
                randomX = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
                randomY = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            }
            agent = Instantiate(agent, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);

            int randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            while ((matrix[(int)randomX2, (int)randomY2] != 0) && (randomX2 != randomX || randomY2 != randomY))
            {
                randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
                randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            }
            goal = Instantiate(goal, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            */
            goal = goals.FirstOrDefault();
            foreach (GameObject agent in agents)
            {
                agent.GetComponent<AgentController>().target = goals.FirstOrDefault().GetComponent<Transform>();
                goals.RemoveAt(0);
            }

            // StartCoroutine(WaitTillSimulationComplete(isDone =>
            // {
            //     if (isDone)
            //     {
            //         GoalReached goalReached = goal.GetComponent<GoalReached>();
            //         simulationResult = "{simulation_time: " + goalReached.timer + ",\ncollision_count: " + goalReached.collisionCount + "}";
            //     }
            // }));
            return simulationResult;

        }
        else
        {
            int randomX = UnityEngine.Random.Range((int)-WIDTH / 2, (int)WIDTH / 2);
            int randomY = UnityEngine.Random.Range((int)-HEIGHT / 2, (int)HEIGHT / 2);
            agent = Instantiate(agent, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            currentTile = floor[0];
            tilemap.SetTile(new Vector3Int(randomX, randomY, 0), currentTile);

            int randomX2 = UnityEngine.Random.Range((int)-WIDTH / 2, (int)WIDTH / 2);
            int randomY2 = UnityEngine.Random.Range((int)-HEIGHT / 2, (int)HEIGHT / 2);
            while (randomX2 == randomX || randomY2 == randomY)
            {
                randomX2 = UnityEngine.Random.Range((int)-WIDTH / 2, (int)WIDTH / 2);
                randomY2 = UnityEngine.Random.Range((int)-HEIGHT / 2, (int)HEIGHT / 2);
            }

            goal = Instantiate(goal, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            agent.GetComponent<AgentController>().target = goal.GetComponent<Transform>();
            currentTile = floor[0];
            tilemap.SetTile(new Vector3Int(randomX2, randomY2, 0), currentTile);
            Invoke("Scan", 0.1f);
            return simulationResult;
        }


    }
    public IEnumerator WaitTillSimulationComplete(System.Action<bool> callback = null)
    {
        Debug.Log("Start simulation");
        yield return WaitUntilTrue(IsReached);
        callback(true);
        Debug.Log("Simulation Complete");
    }
    bool IsReached()
    {
        return goal.GetComponent<GoalReached>().reached;
    }
    IEnumerator WaitUntilTrue(Func<bool> checkMethod)
    {
        while (checkMethod() == false)
        {
            yield return null;
        }
        yield return true;
    }

    void Scan()
    {
        pathfinder.GetComponent<AstarPath>().Scan();
        //foreach (GameObject agent in agents) agent.GetComponent<Pathfinding.AIBase>().SearchPath();

        //player.GetComponent<Pathfinding.AIBase>().canMove = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
