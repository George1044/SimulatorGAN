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

    public Tile[] grass, road, obstacle;
    public bool randomMap;
    public GameObject goal, player;
    public int mapNumber;
    int i = 0, j = 0;
    int[,] matrix;
    Tile currentTile;
    Tilemap tilemap;
    StringBuilder sb;
    String stringName;

    // Start is called before the first frame update
    void Start()
    {
        if (randomMap)
            stringName = "randomMap" + mapNumber + ".txt";
        else
            stringName = "map" + mapNumber + ".txt";
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
                if (matrix[i, j] == 0)
                {
                    currentTile = grass[0];
                }
                else if (matrix[i, j] == 1)
                {
                    currentTile = road[80];
                }
                else if (matrix[i, j] == 2)
                {
                    currentTile = obstacle[0];
                }
                else if (matrix[i, j] == 5)
                {
                    Instantiate(player, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    currentTile = road[80];
                }
                else if (matrix[i, j] == 6)
                {
                    Instantiate(goal, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                    currentTile = road[4];
                }

                tilemap.SetTile(new Vector3Int(x, y, 0), currentTile);
                j++;
            }
            j = 0;
            i++;
        }
        if (randomMap)
        {
            int randomX = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            Instantiate(player, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            currentTile = road[80];
            tilemap.SetTile(new Vector3Int(randomX, randomY, 0), currentTile);

            int randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
            int randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            while (randomX2 == randomX || randomY2 == randomY)
            {
                randomX2 = UnityEngine.Random.Range((int)-width / 2, (int)width / 2);
                randomY2 = UnityEngine.Random.Range((int)-height / 2, (int)height / 2);
            }
            Instantiate(goal, new Vector3(randomX + 0.5f, randomY + 0.5f, 0), Quaternion.identity);
            currentTile = road[4];
            tilemap.SetTile(new Vector3Int(randomX2, randomY2, 0), currentTile);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
