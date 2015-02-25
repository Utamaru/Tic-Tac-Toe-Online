using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviourSingleton<Grid>
{
    public delegate void CheckGameOverDelegate(PunTeams.Team team);
    public event CheckGameOverDelegate OnGameOver;

    private Tile[,] tiles;
    private int turns;

    private void Awake()
    {
        turns = 0;
        InitializeGrid();
    }

    public void SetTile(Tile tile, IntVector2 coordinates)
    {
        tiles[coordinates.x, coordinates.y] = tile;
    }

    public bool PerformTurn(IntVector2 coordinates, PunTeams.Team team)
    {
        bool result = tiles[coordinates.x, coordinates.y].Occupy(team == PunTeams.Team.X ? TileState.X : TileState.O);

        if (result)
        {
            turns += 1;
        }

        if (team == PhotonNetwork.player.GetTeam())
        {
            if (Victory())
            {
                if (OnGameOver != null)
                {
                    OnGameOver(PhotonNetwork.player.GetTeam());
                }
                result = false;
            }
            else if (turns == 9)
            {
                if (OnGameOver != null)
                {
                    OnGameOver(PunTeams.Team.none);
                }
                result = false;
            }
        }

        return result;
    }

    public void Clear()
    {
        turns = 0;

        for (int i = 0; i < Helpers.GridSize.x; ++i)
        {
            for (int j = 0; j < Helpers.GridSize.y; ++j)
            {
                tiles[i, j].Clear();
            }
        }
    }

    private void InitializeGrid()
    {
        tiles = new Tile[Helpers.GridSize.x, Helpers.GridSize.y];
    }

    private bool Victory()
    {
        for (int i = 0; i < 3; ++i)
        {
            if (IsEqual(tiles[i, 0], tiles[i, 1], tiles[i, 2]) |
                IsEqual(tiles[0, i], tiles[1, i], tiles[2, i]))
            {
                return true;
            }
        }

        return IsEqual(tiles[0, 0], tiles[1, 1], tiles[2, 2]) || IsEqual(tiles[0, 2], tiles[1, 1], tiles[2, 0]);
    }

    private bool IsEqual(Tile tileOne, Tile tileTwo, Tile tileThree)
    {
        return (tileOne.State != TileState.Free) && (tileOne.State == tileTwo.State) && (tileOne.State == tileThree.State);
    }
}
