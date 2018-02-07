using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour {

    //Struct used for referencing coorinates in a hexgrid
    public struct HexCooridnates
    {
        public int _x { get; private set; }

        public int _z { get; private set; }

        public HexCooridnates(int x, int z)
        {
            _x = x;
            _z = z;
        }
    }

    enum MovementType
    {
        Passable,
        Occupied,
        Impassible
    }

    //Used to change how easy it is to move through a tile
    enum TerrainMovementModifier
    {
        Easy,
        Normal,
        Hard
    }

    private float _movementRequirement;
    private MovementType _movementRestriction;
    private TerrainMovementModifier _movementModifer;
    private Map _map;
    private HexCooridnates _cooridnates;
    private bool[] _factionInfluence;

    private void Start()
    {
        _movementRequirement = 1.0f;
        _movementRestriction = MovementType.Passable;
        _movementModifer = TerrainMovementModifier.Normal;
        _map = this.transform.parent.GetComponent<Map>();
        uint _numberOfPlayers = _map.GetNumberOfPlayers();
        _factionInfluence = new bool[_numberOfPlayers];
    }
}
