using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Action<Player, int> OnScore;
    public Action OnGameReset;

    [SerializeField] private GameObject boardManagerPrefab;
    [Space(20f)]
    [SerializeField] private GameObject blackStone;
    [SerializeField] private GameObject whiteStone;
    [Space(20f)]
    [SerializeField] [Range(0.5f, 1)] private float stoneRelativeScale = 0.9f;

    private Player currentPlayer = Player.Default;
    private Vector3 stoneScale = Vector3.one;
    private Dictionary<Vector2, GameObject> stones = new Dictionary<Vector2, GameObject>();

    private BoardManager boardManager;
    private InputManager inputManager;

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


    private IEnumerator Start()
    {
        SetManagers();
        SetListeners();

        yield return new WaitUntil(() => boardManager.isActiveAndEnabled);
        SetupGame();

        yield return new WaitUntil(() => inputManager != null);
        StartGame();
    }

    private void SetManagers()
    {
        boardManager = Instantiate(boardManagerPrefab, transform).GetComponent<BoardManager>();
        inputManager = gameObject.AddComponent<InputManager>();
    }

    private void StartGame()
    {
        CurrentPlayer = Player.White;
        inputManager.enabled = true;
    }

    private void SetupGame()
    {
        Camera.main.transform.position = new Vector3(boardManager.Width / 2, boardManager.Height / 2, -10f);
        Camera.main.orthographicSize =
            boardManager.Width > boardManager.Height ? boardManager.Width / 2 : boardManager.Height / 2;

        stoneScale *= stoneRelativeScale * boardManager.TileSize;
    }

    private void SetListeners()
    {
        inputManager.OnInput += HandleInput;
    }

    private void HandleInput(Vector3 position)
    {

        boardManager.TrySetStone(position, (int)currentPlayer, OnComplete);
    }

    private void OnComplete(Vector2 stonePosition, List<Vector2> capturedStones)
    {
        SetStone(stonePosition);
        RemoveCapturedStones(capturedStones);
        UpdateScore(capturedStones);
    }

    private void RemoveCapturedStones(List<Vector2> stoneCoordinates)
    {
        foreach (var coord in stoneCoordinates)
        {
            stones.TryGetValue(coord, out GameObject stone);
            Destroy(stone);
            stones.Remove(coord);
        }
    }

    private void SetStone(Vector2 coordinates)
    {
        var stone = currentPlayer == Player.White ? whiteStone : blackStone;

        var worldPos = boardManager.GetWorldPosition(coordinates.x, coordinates.y);

        var spawnedStone = Instantiate(stone, worldPos, Quaternion.identity);
        spawnedStone.transform.localScale = stoneScale;
        stones.Add(coordinates, spawnedStone);
    }

    private void UpdateScore(List<Vector2> capturedStones)
    {
        if (capturedStones.Count > 0) OnScore.Invoke(currentPlayer, capturedStones.Count);
        ChangePlayer();
    }

    private void ChangePlayer()
    {
        currentPlayer = currentPlayer == Player.White ? Player.Black : Player.White;
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGameRoutine());
    }

    private IEnumerator ResetGameRoutine()
    {
        inputManager.enabled = false;
        yield return new WaitForSeconds(1f);

        foreach (var stone in stones)
        {
            Destroy(stone.Value);
        }
        stones.Clear();
        boardManager.ResetBoard();
        currentPlayer = Player.White;
        OnGameReset?.Invoke();
        inputManager.enabled = true;
    }
}

public enum Player
{
    Default = 0,
    White = 1,
    Black = 2
}