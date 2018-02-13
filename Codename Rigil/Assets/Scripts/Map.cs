using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    //Struct used for referencing coorinates in a hexgrid
    public struct HexCooridnates
    {
        public int _x { get; private set; }

        public int _y { get; private set; }

        public HexCooridnates(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }

    //Used for tracking the total distance between 2 hexes
    public struct DistanceMarker
    {
        public Hex _hex { get; private set; }

        public float _distance { get; set; }

        public DistanceMarker(Hex hex, float distance)
        {
            _hex = hex;
            _distance = distance;
        }
    }

    //Used if map generaters besides quadrilaterals are used later
    enum MapType { Quad }

    //Used to generate the correct distance between hexes
    private const float HexWidth = 3.8f;
    private const float HexHeight = 4.4f;

    //The prefab of the hex object
    [SerializeField]
    private Hex _hexObject;

    //Used if other map generation options are wanted in the future
    [SerializeField]
    private MapType _mapShape = MapType.Quad;

    //The dimensions of the map.
    //TODO : Make a list so different parameter signatures can be used for alternative map generator options and shapes
    [SerializeField]
    private uint[] _dimensions = new uint[] { 10, 10 };

    //Reference to the level manager
    private LevelManager _levelManager;
    private Hex[,] _hexes;

    //Used only for debugging
    public void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            SelectCell(new HexCooridnates(3,3));
        }
        if (Input.GetKeyDown("w"))
        {
            HighlightCells(GetHexesInRange(new HexCooridnates(3, 3), 1.0f));
        }
        if (Input.GetKeyDown("e"))
        {
            HighlightCells(GetHexesInRange(new HexCooridnates(3, 3), 2.0f));
        }
        if (Input.GetKeyDown("r"))
        {
            HighlightCells(GetHexesInRange(new HexCooridnates(3, 3), 3.0f));
        }
        if (Input.GetKeyDown("t"))
        {
            HighlightCells(GetHexesInRange(new HexCooridnates(3, 3), 1.0f, 4.0f));
        }
        if (Input.GetKeyDown("y"))
        {
            HighlightCells(GetHexesInRange(new HexCooridnates(3, 3), 2.0f, 4.0f));
        }
    }


    private void Start()
    {
        _hexes = new Hex[_dimensions[0], _dimensions[1]];
        _levelManager = this.transform.GetComponent<LevelManager>();
        switch (_mapShape)
        {
            case MapType.Quad:
                GenerateQuadMap();
                break;
        }
    }

    //Generates a rectangular map based upon 2 uint values provided by the _dimension class variable that should be set at compile time
    private void GenerateQuadMap()
    {
        for (int x = 0, i = 0; x < _dimensions[0]; x++)
        {
            for(int y = 0; y < _dimensions[1]; y++)
            {
                _hexes[x, y] = GenerateHex(x, y, i++);
            }
        }
    }

    //Generates an individual hex
    private Hex GenerateHex(int x, int y, int i)
    {
        Vector3 position;
        position.x = x  * HexWidth;
        position.y = (y + (x * 0.5f) - x / 2) * HexHeight;
        position.z = 0.0f;

        Hex hex = Instantiate<Hex>(_hexObject);
        hex.transform.SetParent(transform,false);
        hex.transform.localPosition = position;

        hex.Coords = new HexCooridnates(x, y - (x / 2));

        return hex;
    }

    //Selects a cell
    public void SelectCell(HexCooridnates coords)
    {
        int[] index = HexCoordToArrayindex(coords);
        _hexes[index[0], index[1]].Colour = Color.red;
    }

    //highlights cells
    public void HighlightCells(List<HexCooridnates> coords)
    {
        int[] index;
        foreach (HexCooridnates coordinates in coords)
        {
            index = HexCoordToArrayindex(coordinates);
            _hexes[index[0], index[1]].Colour = Color.blue;
        }
    }

    //Converts hexcoordinates to the correct spot in the 2D array
    private int[] HexCoordToArrayindex(HexCooridnates hexcoordinates)
    {
        int[] index = new int[2];
        index[0] = hexcoordinates._x;
        index[1] = hexcoordinates._y + (hexcoordinates._x / 2);
        return index;
    }

    //returns a list of all the reachable hexes
    private List<HexCooridnates> GetMoveableHexes(HexCooridnates coords, float remainingUnitMovement)
    {
        List<DistanceMarker> _reachableHexes = new List<DistanceMarker>();
        List<HexCooridnates> _reachableHexCoords = new List<HexCooridnates>();
        return _reachableHexCoords;
    }

    //Returns all targetable hexes
    private List<HexCooridnates> GetHexesInRange(HexCooridnates coords, float maxRange)
    {
        return GetHexesInRange(coords, 0.0f, maxRange);
    }

    //Returns all targetable hexes
    private List<HexCooridnates> GetHexesInRange(HexCooridnates coords, float minRange, float maxRange)
    {
        List<HexCooridnates> _reachableHexCoords = new List<HexCooridnates>();
        for(int x = 0 - (int)maxRange; x <= maxRange; x++)
        {
            for (int y = 0 - (int)maxRange; y <= maxRange; y++)
            {
                int[] i = HexCoordToArrayindex(new HexCooridnates(x + coords._x, y + coords._y));
                //Checks if hex is on the board
                if (i[0] >= 0 && i[0] <= _dimensions[0] - 1 && i[1] >= 0 && i[1] <= _dimensions[1])
                {
                    //Checks if hex is in distance
                    if (Mathf.Abs(x + y) <= maxRange && Mathf.Abs(x) <= maxRange && Mathf.Abs(y) <= maxRange)
                    {
                        _reachableHexCoords.Add(new HexCooridnates(x + coords._x, y + coords._y));
                    }
                }
            }
        }

        //removes tiles lower than the minimum range
        if(minRange > 0)
        {
            List<HexCooridnates> _closeHex = GetHexesInRange(coords, minRange - 1);
            foreach(HexCooridnates hex in _closeHex)
            {
                _reachableHexCoords.Remove(hex);
            }
        }
        return _reachableHexCoords;
    }

    public uint GetNumberOfPlayers()
    {
        return _levelManager.NumberOfPlayers;
    }
}
