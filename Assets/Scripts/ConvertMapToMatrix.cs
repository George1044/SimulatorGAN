using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConvertMapToMatrix : MonoBehaviour
{

    public Tile[] grass, road, obstacle;
    int vertical, horizontal, columns, rows, i = 0, j = 0, f = 0;
    int[,] matrix;
    Tile currentTile;
    Tilemap tilemap;
    StringBuilder sb;
    GameObject[] goals;
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        horizontal = (int)tilemap.localBounds.max.x;
        vertical = (int)tilemap.localBounds.max.y;

        columns = horizontal - (int)tilemap.localBounds.min.x;
        rows = vertical - (int)tilemap.localBounds.min.y;
        matrix = new int[rows, columns];

        for (int y = vertical - 1; y >= (int)tilemap.localBounds.min.y; y--)
        {
            for (int x = (int)tilemap.localBounds.min.x; x < horizontal; x++)
            {
                currentTile = (Tile)tilemap.GetTile(new Vector3Int(x, y, 0));
                if (grass.Contains(currentTile))
                {
                    matrix[i, j] = 0;
                }
                else if (road.Contains(currentTile))
                {
                    matrix[i, j] = 1;
                }
                else if (obstacle.Contains(currentTile))
                {
                    matrix[i, j] = 2;
                }
                else matrix[i, j] = -1;
                j++;
            }
            i++;
            j = 0;
        }
        Debug.Log("Min x: " + tilemap.localBounds.min.x + " Min y: " + tilemap.localBounds.min.y);
        goals = GameObject.FindGameObjectsWithTag("Goal");
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (GameObject goal in goals)
        {
            Debug.Log((int)goal.transform.position.x - (int)tilemap.localBounds.min.x);
            matrix[(int)(-(goal.transform.position.y) - tilemap.localBounds.min.y), (int)((goal.transform.position.x) - tilemap.localBounds.min.x)] = 6;
        }
        matrix[(int)(-(player.transform.position.y) - tilemap.localBounds.min.y), (int)((player.transform.position.x) - tilemap.localBounds.min.x)] = 5;
        sb = new StringBuilder();
        for (int a = 0; a < rows; a++)
        {
            for (int b = 0; b < columns; b++)
            {
                sb.Append(matrix[a, b]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        while (System.IO.File.Exists("map" + f + ".txt"))
        {
            f++;
        }
        StreamWriter writer = new StreamWriter("map" + f + ".txt", true);
        writer.Write(sb.ToString());
        writer.Close();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
