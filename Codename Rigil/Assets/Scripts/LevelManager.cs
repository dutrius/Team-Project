using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField]
    private uint _numberOfPlayers;

    public uint NumberOfPlayers
    {
        get
        {
            return _numberOfPlayers;
        }
    }
}
