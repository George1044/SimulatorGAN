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
    public GameObject goal, agent, highObstacle, lowObstacle;
    private List<GameObject> agents = new List<GameObject>(), goals = new List<GameObject>();
    public int mapNumber;
    int i = 0, j = 0, a = 0, g = 0;
    int[,] matrix;
    Tile currentTile;
    Tilemap tilemap;
    StringBuilder sb;
    String stringName;
    public GameObject pathfinder;

    // Start is called before the first frame update
    void Start()
    {
        string pwd = System.IO.Directory.GetCurrentDirectory();
        if (randomMap)
            stringName = pwd + "/Maps/randomMap" + mapNumber + ".txt";
        else
            stringName = pwd + "/Maps/map" + mapNumber + ".txt";
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
                    a++;
                }
                else if (matrix[i, j] == 3)
                {
                    goals.Add(Instantiate(goal, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity));
                    currentTile = floor[14];
                    g++;
                }

                tilemap.SetTile(new Vector3Int(x, y, 0), currentTile);
                j++;
            }
            j = 0;
            i++;
        }
        g = 0;
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Pathfinding.AIDestinationSetter>().target = goals.FirstOrDefault().GetComponent<Transform>();
            goals.RemoveAt(0);
            g++;
        }
        if (randomMap)
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
            currentTile = floor[14];
            tilemap.SetTile(new Vector3Int(randomX2, randomY2, 0), currentTile);
        }

        Invoke("Scan", 1.0f);

    }

    void Scan()
    {
        pathfinder.GetComponent<AstarPath>().Scan();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
