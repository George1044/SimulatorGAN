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

    public Tile[] floor;
    public bool randomMap;
    public GameObject goal, agent, highObstacle, lowObstacle, pathfinder;
    private List<GameObject> agents = new List<GameObject>(), goals = new List<GameObject>();
    public float timeoutTimer = 2;
    public int mapNumber;
    int i = 0, j = 0;
    int width = 32, height = 32;
    public int minGoalDistance = 8;
    private bool failed = false;
    int[,] matrix;
    Tile currentTile;
    Tilemap tilemap;
    StringBuilder sb;
    String stringName;
    public string mapName = "map";
    //public GameObject pathfinder, player;

    // Start is called before the first frame update
    void Start()
    {
        if (!randomMap)
        {
            string pwd = System.IO.Directory.GetCurrentDirectory();
            if (randomMap)
                stringName = pwd + "/Maps/randomMap" + mapNumber + ".txt";
            else
                stringName = pwd + "/Maps/" + mapName + mapNumber + ".txt";
            StreamReader reader1 = new StreamReader(stringName);
            StreamReader reader = new StreamReader(stringName);
            width = reader1.ReadLine().Split(' ').Length - 1;
            String textFile = reader.ReadToEnd();
            textFile = Regex.Replace(textFile, "\n|\r", "");
            String[] numbers = (textFile.Split(' '));
            height = numbers.Length / (width);


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
                        UnityEditor.EditorApplication.isPlaying = false;
                        break;
                    }
                    randomX = UnityEngine.Random.Range(0, height);
                    randomY = UnityEngine.Random.Range(0, width);
                }
                if (!failed) matrix[randomX, randomY] = 2; else UnityEditor.EditorApplication.isPlaying = false;

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
                        UnityEditor.EditorApplication.isPlaying = false;
                        break;
                    }
                    randomX2 = UnityEngine.Random.Range(0, height);
                    randomY2 = UnityEngine.Random.Range(0, width);
                    distance = Math.Sqrt(Math.Pow(randomX - randomX2, 2) + Math.Pow(randomY - randomY2, 2));
                }
                if (!failed) matrix[randomX2, randomY2] = 3; else UnityEditor.EditorApplication.isPlaying = false;

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
                agent.GetComponent<Pathfinding.AIDestinationSetter>().target = goals.FirstOrDefault().GetComponent<Transform>();
                goals.RemoveAt(0);
            }

        }
        else
        {
            int randomX = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            agent = Instantiate(agent, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            currentTile = floor[0];
            tilemap.SetTile(new Vector3Int(randomX, randomY, 0), currentTile);

            int randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            while (randomX2 == randomX || randomY2 == randomY)
            {
                randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
                randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            }

            goal = Instantiate(goal, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            agent.GetComponent<Pathfinding.AIDestinationSetter>().target = goal.GetComponent<Transform>();
            currentTile = floor[0];
            tilemap.SetTile(new Vector3Int(randomX2, randomY2, 0), currentTile);
            Invoke("Scan", 0.1f);
        }




    }

    void Scan()
    {
        pathfinder.GetComponent<AstarPath>().Scan();
        foreach (GameObject agent in agents) agent.GetComponent<Pathfinding.AIBase>().SearchPath();

        //player.GetComponent<Pathfinding.AIBase>().canMove = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
