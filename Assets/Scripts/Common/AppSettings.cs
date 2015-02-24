using UnityEngine;
using System.Collections;
using System;

public static class Helpers
{
    public static IntVector2 GridSize;

    static Helpers()
    {
        GridSize = new IntVector2(3, 3);
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}

public struct IntVector2
{
    public int x; 
    public int y;

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", this.x, this.y);
    }
}

public enum TileState
{
    Free, 
    X, 
    O
}

public enum GameState
{
    None,
    WaitingForPlayers,
    WaitingForTurnByX,
    WaitingForTurnByO,
    WaitingForRestart,
    YouWon,
    YouLost,
    Tie
}

