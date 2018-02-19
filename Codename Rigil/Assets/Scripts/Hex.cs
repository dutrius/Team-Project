using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex : MonoBehaviour {

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
    private Map.HexCooridnates _cooridnates;
    private bool[] _factionInfluence;
    private Color _colour;
    private SpriteRenderer _renderer;

    private void Start()
    {
        //Sets the value of all required variables
        _map = this.transform.parent.GetComponent<Map>();
        uint _numberOfPlayers = _map.GetNumberOfPlayers();
        _factionInfluence = new bool[_numberOfPlayers];

        _renderer = this.transform.GetComponent<SpriteRenderer>();



        _movementRequirement = 1.0f;
        _movementRestriction = MovementType.Passable;
        _movementModifer = TerrainMovementModifier.Normal;

        if(_cooridnates._x == 3 && _cooridnates._y == 3)
        {
            Colour = Color.black;
            _movementRequirement = 5;
        }
    }

    //Debug function to test functionality
    void OnMouseDown()
    {
        Debug.Log("Sprite Clicked");
    }

    //Returns the terrain difficulty modifier as a float, is taken into account but currently tiles dont have different modifiers
    private float GetTerrainDifficulty(TerrainMovementModifier i)
    {
        switch (i)
        {
            case TerrainMovementModifier.Easy:
                return 0.75f;
            case TerrainMovementModifier.Normal:
                return 1.0f;
            case TerrainMovementModifier.Hard:
                return 1.25f;
            default:
                return 1.0f;
        }
    }

    public float MovementPointsRequired(int faction)
    {
        return _movementRequirement;
    }

    private float MovementRequirements
    {
        get
        {
            return _movementRequirement * GetTerrainDifficulty(_movementModifer);
        }
    }

    public bool Occupied
    {
        get
        {
            return _movementRestriction == MovementType.Occupied;
        }
        set
        {
            if (value == true)
            {
                _movementRestriction = MovementType.Occupied;
            }
            else
            {
                _movementRestriction = MovementType.Passable;
            }
        }
    }

    public Map.HexCooridnates Coords
    {
        get
        {
            return _cooridnates;
        }
        set
        {
            _cooridnates = value;
        }
    }

    public Color Colour
    {
        get
        {
            return _colour;
        }
        set
        {
            _renderer = this.transform.GetComponent<SpriteRenderer>();
            _colour = value;
            _renderer.color = _colour;
            if (_cooridnates._x == 3 && _cooridnates._y == 3)
            {
                _renderer.color = Color.black;
            }
        }
    }
}
