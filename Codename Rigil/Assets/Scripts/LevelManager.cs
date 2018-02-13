using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField]
    private uint _numberOfPlayers;
    private uint _currentPlayer;

    public uint NumberOfPlayers
    {
        get
        {
            return _numberOfPlayers;
        }
    }

    public uint CurrentPlayer
    {
        get
        {
            return _currentPlayer;
        }
    }
}
