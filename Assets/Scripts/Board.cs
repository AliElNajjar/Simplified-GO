using System;
using UnityEngine;

public class Board
{

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    private InputManager inputManager;
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private int[,] gridArray;

    public Board(InputManager inputManager, int width, int height, float cellSize, Vector3 originPosition = default)
    {
        this.inputManager = inputManager;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height];

    }


    public Vector3 GetWorldPosition(int x, int y)
    {
        return originPosition + cellSize * new Vector3(x + 0.5f, y + 0.5f);
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        GetXY(worldPosition, out int x, out int y);

        if (IsFreeValidSlot(x, y))
        {
            gridArray[x, y] = value;
            inputManager.PutStone(GetWorldPosition(x,y));
            ChangePlayer();
            if (OnGridValueChanged != null) OnGridValueChanged(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    private void ChangePlayer() // in a manager to separate concerns
    {
        //after a time delay?
        inputManager.CurrentPlayer = inputManager.CurrentPlayer == InputManager.Player.White ? InputManager.Player.Black : InputManager.Player.White;
    }

    public bool IsFreeValidSlot(int x, int y)
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
}
