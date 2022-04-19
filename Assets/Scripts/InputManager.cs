using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum Player
    {
        Default = 0,
        White = 1,
        Black = 2
    }

    public Player CurrentPlayer
    {
        get
        {
            return currentPlayer;
        }
        set
        {
            currentPlayer = value;
        }
    }

    [SerializeField] private GameObject blackStone;
    [SerializeField] private GameObject whiteStone;
    [SerializeField] private GameObject tile;

    private Player currentPlayer = Player.Default;
    private Board board;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        board = new Board(this, 12, 12, 10f);
        DrawBoard();
        CurrentPlayer = Player.White; //later to be set by gamemanager.
    }

    private void DrawBoard() //temporary for testing
    {
        for (int x = 0; x < 12; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                var spawnedTile = Instantiate(tile, board.GetWorldPosition(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
            }
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            board.SetValue(GetMouseWorldPosition(), (int)currentPlayer);
                // if player 1, else value = 2. //at end of turn of the current player, switch to the other player and change this value accordingly.
            //end of turn means successfully placed a stone and results calculated.
        }
    }

    public void PutStone(Vector3 pos)
    {
        var stone = currentPlayer == Player.White ? whiteStone : blackStone;
        Instantiate(stone, pos, Quaternion.identity);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = mainCam.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

}
