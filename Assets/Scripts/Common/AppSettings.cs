using UnityEngine;
using System.Collections;
using System;

public static class Helpers
{
    public static IntVector2 GridSize;

    private static string[] messages;

    static Helpers()
    {
        GridSize = new IntVector2(3, 3);

        messages = new string[]
        {
            string.Empty,
            "Waiting for another player to join your game",
            "Waiting for your opponent's response",
            "You won\nPress \"spacebar\" to try to win again",
            "You lost\nPress \"spacebar\" to take revenge",
            "It's a tie\nPress \"spacebar\" to rematch",
            "It is your turn",
            "It is your opponent's turn"
        };
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static string GetMessageFromGameState(GameState state)
    {
        return messages[(int)state];
    }

    public static string GetMessageFromGameState(int index)
    {
        return messages[index];
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
    None = 0,
    WaitingForPlayers = 1,
    WaitingForRestart = 2,
    YouWon = 3,
    YouLost = 4,
    Tie = 5,
    WaitingForTurnByX,
    WaitingForTurnByO,
}

