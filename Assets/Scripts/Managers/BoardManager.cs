using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject tile;

    [Header("Board Options")]
    [SerializeField] private int width = 12;
    [SerializeField] private int height = 12;
    [SerializeField] private float offset = 0.1f;

    private Vector3 originPosition = Vector3.zero;
    private int[,] gridArray;
    private float tileSize;
    
    public float TileSize => tileSize;
    public int Width => width;
    public int Height => height;


    private void Start()
    {
        InitBoard();
    }

    private void InitBoard() //temporary for testing
    {
        gridArray = new int[width, height];

        tileSize = 1 - offset;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tile, GetWorldPosition(x, y), Quaternion.identity);
                spawnedTile.transform.localScale = new Vector3(tileSize, tileSize, 1f);
                spawnedTile.name = $"Tile {x} {y}";
            }
        }

    }

    public Vector3 GetWorldPosition(float x, float y)
    {
        return originPosition + new Vector3(x + 0.5f, y + 0.5f);
    }

    private void GetGridCoordinates(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x);
        y = Mathf.FloorToInt((worldPosition - originPosition).y);
    }

    public void TrySetStone(Vector3 worldPosition, int value, Action<Vector2, List<Vector2>> onComplete)
    {
        GetGridCoordinates(worldPosition, out int x, out int y);

        if (IsFreeValidSlot(x, y))
        {
            gridArray[x, y] = value;
            var capturedStonesCoordinates = CheckForCapturedStones(x, y);
            var newStoneCoordinates = new Vector2(x, y);

            onComplete?.Invoke(newStoneCoordinates, capturedStonesCoordinates);
        }
    }

    private bool IsFreeValidSlot(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y] == 0;
        }
        else
        {
            return false;
        }
    }


    //Checks if surrounding stones are enemy stones, the checks if any of these stones are captured
    private List<Vector2> CheckForCapturedStones(int x, int y)
    {
        List<Vector2> capturedStones = new List<Vector2>();

        var top = CheckTop(x,y);
        var bot = CheckBot(x,y);
        var left = CheckLeft(x,y);
        var right = CheckRight(x,y);

        if (top)
        {
            if (CheckTop(x, y + 1) && CheckLeft(x, y + 1) && CheckRight(x, y + 1))
            {
                capturedStones.Add(new Vector2(x, y + 1));
                gridArray[x, y + 1] = 0;
            }
        }

        if (bot)
        {
            if (CheckBot(x, y -1) && CheckLeft(x, y -1) && CheckRight(x, y - 1))
            {
                capturedStones.Add(new Vector2(x, y - 1));
                gridArray[x, y - 1] = 0;
            }
        }

        if (left)
        {
            if (CheckTop(x - 1, y) && CheckLeft(x - 1, y) && CheckBot(x - 1, y))
            {
                capturedStones.Add(new Vector2(x - 1, y));
                gridArray[x - 1, y] = 0;
            }
        }

        if (right)
        {
            if (CheckTop(x + 1, y) && CheckBot(x + 1, y) && CheckRight(x + 1, y))
            {
                capturedStones.Add(new Vector2(x + 1, y));
                gridArray[x + 1, y] = 0;
            }
        }

        return capturedStones;
    }

    private bool CheckTop(int x, int y)
    {
        if (y == height - 1) return false;
        if (gridArray[x, y + 1] == 0) return false;
        return (gridArray[x, y] != gridArray[x, y + 1]);
    }

    private bool CheckBot(int x, int y)
    {
        if (y == 0) return false;
        if (gridArray[x, y - 1] == 0) return false;
        return gridArray[x, y] != gridArray[x, y - 1];
    }

    private bool CheckLeft(int x, int y)
    {
        if (x == 0) return false;
        if (gridArray[x - 1, y] == 0) return false;
        return gridArray[x, y] != gridArray[x - 1, y];
    }

    private bool CheckRight(int x, int y)
    {
        if (x == width - 1) return false;
        if (gridArray[x + 1, y] == 0) return false;
        return gridArray[x, y] != gridArray[x + 1, y];
    }

    public void ResetBoard()
    {
        gridArray = new int[width, height];
    }
}
