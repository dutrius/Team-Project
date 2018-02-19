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
        public HexCooridnates _hex { get; private set; }

        public float _distance { get; set; }

        public DistanceMarker(HexCooridnates hex, float distance)
        {
            _hex = hex;
            _distance = distance;
        }
    }

    //Used if map generaters besides quadrilaterals are used later
    enum MapType { Quad }

    //Used to generate the correct distance between hexes
    private const float DefaultHexWidth = 0.038f;
    private const float DefaultHexHeight = 0.044f;
    private const float Scale = 4.0f;

    private const float HexWidth = DefaultHexWidth * Scale;
    private const float HexHeight = DefaultHexHeight * Scale;

    //Unit prefabs
    [SerializeField]
    private GameObject _greenFrog;
    [SerializeField]
    private GameObject _blueFrog;

    //The prefab of the hex object
    [SerializeField]
    private Hex _hexObject;
    [SerializeField]
    private GameObject _heightControl;
    private GameObject[] _heightControllers;

    //Used if other map generation options are wanted in the future
    [SerializeField]
    private MapType _mapShape = MapType.Quad;

    //The dimensions of the map.
    //TODO : Make a list so different parameter signatures can be used for alternative map generator options and shapes
    [SerializeField]
    private uint[] _dimensions = new uint[] { 10, 10 };
    private float _xOffset;
    private float _yOffset;

    //Reference to the level manager
    private LevelManager _levelManager;
    private Hex[,] _hexes;

    private void Start()
    {
        //Used to offset the map so its centered on the map manager
        _xOffset = _dimensions[0] * HexWidth / 2;
        _yOffset = _dimensions[1] * HexHeight / 2;

        _hexes = new Hex[_dimensions[0], _dimensions[1]];
        _levelManager = this.transform.GetComponent<LevelManager>();
        switch (_mapShape)
        {
            case MapType.Quad:
                GenerateQuadMap();
                break;
        }
        List<GameObject> units = new List<GameObject>();
        units.Add(Instantiate(_greenFrog));
        units.Add(Instantiate(_blueFrog));
        PlaceUnits(units);
    }

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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
        
        GameObject[] _heightControllers = new GameObject[4];
        for(int i = 0; i < 4; i++)
        {
            _heightControllers[i] = Instantiate(_heightControl);
            _heightControllers[i].transform.SetParent(transform, false);
        }

        _heightControllers[0].transform.localPosition = new Vector3(_xOffset + HexWidth / 2, 0, _yOffset + HexHeight / 2);
        _heightControllers[1].transform.localPosition = new Vector3(_xOffset + HexWidth / 2, 0, -_yOffset - HexHeight);
        _heightControllers[2].transform.localPosition = new Vector3(-_xOffset - HexWidth, 0, _yOffset + HexHeight / 2);
        _heightControllers[3].transform.localPosition = new Vector3(-_xOffset - HexWidth, 0, -_yOffset - HexHeight);
    }

    //Generates an individual hex
    private Hex GenerateHex(int x, int y, int i)
    {
        Vector3 position;
        position.x = x  * HexWidth - _xOffset;
        position.y = 0;
        position.z = (y + (x * 0.5f) - x / 2) * HexHeight - _yOffset;

        Hex hex = Instantiate<Hex>(_hexObject);
        hex.transform.SetParent(transform,false);
        hex.transform.localPosition = position;
        hex.transform.localScale *= Scale;

        hex.Coords = new HexCooridnates(x, y - (x / 2));

        return hex;
    }

    //Places all units on the map
    private void PlaceUnits(List<GameObject> units)
    {
        foreach(GameObject unit in units)
        {
            Vector3 position;
            position.x = 0;
            position.y = this.transform.position.y;
            position.z = 0;
            unit.transform.position = position;
            SnapUnit(unit, new HexCooridnates(5,0));
        }
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
        foreach(Hex hex in _hexes)
        {
            hex.Colour = Color.white;
        }
        int[] index;
        foreach (HexCooridnates coordinates in coords)
        {
            index = HexCoordToArrayindex(coordinates);
            _hexes[index[0], index[1]].Colour = Color.blue;
        }
    }
    //Converts Array coordinates to hexcoordinates
    private HexCooridnates ArrayIndexToHexCoord(int x, int y)
    {
        return new HexCooridnates(x, y - (x / 2));
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
    public List<HexCooridnates> GetMoveableHexes(HexCooridnates start, float MaxMovement, int currentFaction)
    {
        List<DistanceMarker> _reachableHexes = Flood(new DistanceMarker(start, 0), currentFaction, MaxMovement);

        List<HexCooridnates> _reachableHexCoords = new List<HexCooridnates>();
        foreach(DistanceMarker marker in _reachableHexes)
        {
            _reachableHexCoords.Add(marker._hex);
        }
        return _reachableHexCoords;
    }

    private int[,] _updateOrder = new int[,]
{
        {1 ,  -1},
        {1 ,  0},
        {0 ,  -1},
        {0 ,  1},
        {-1 , 0},
        {-1 , 1}
};

    private List<DistanceMarker> Flood(DistanceMarker currentHexMarker, int currentFaction, float maxMovement)
    {
        List<DistanceMarker> _reachableHexes = new List<DistanceMarker>();

        for (int i = 0; i < 6; i++)
        {
            int testX = currentHexMarker._hex._x + _updateOrder[i, 0];
            int testY = currentHexMarker._hex._y + _updateOrder[i, 1];

            int[] arrayIndex = HexCoordToArrayindex(new HexCooridnates(testX, testY));

            //tests if hex exists
            if (testX >= 0 &&
                testX < _dimensions[0] &&
                testY >= 0 - (testX / 2) &&
                testY  < _dimensions[1] - (testX / 2))
            {
                DistanceMarker marker = new DistanceMarker(new HexCooridnates(testX, testY), currentHexMarker._distance + _hexes[arrayIndex[0], arrayIndex[1]].MovementPointsRequired(currentFaction));
                //Checks if marker is in range
                if (marker._distance <= maxMovement && IsShorterRoute(_reachableHexes, marker))
                {
                    _reachableHexes.Add(marker);
                    _reachableHexes.AddRange(Flood(marker, currentFaction, maxMovement));
                }
            }
        }
        return _reachableHexes;
    }

    private bool IsShorterRoute(List<DistanceMarker> reachableHexes, DistanceMarker current)
    {
        foreach (DistanceMarker old in reachableHexes)
        {
            if (current._hex._x == old._hex._x && current._hex._y == old._hex._y && old._distance < current._distance)
                return false;
        }
        return true;
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

    public HexCooridnates FindClosestHex(GameObject unit)
    {
        Transform unitTransform = unit.transform;
        float unitX = unitTransform.position.x;
        float unitZ = unitTransform.position.z;

        int x = Mathf.RoundToInt(unitX / HexWidth) + (int)_dimensions[0] / 2;
        int y = Mathf.RoundToInt((unitZ - (x * 0.5f - x / 2) * HexHeight) / HexHeight) + (int)_dimensions[0] / 2;

        return ArrayIndexToHexCoord(x, y);
    }

    //Snaps a unit to the selected hex
    public void SnapUnit(GameObject unit, HexCooridnates coords)
    {
        Transform unitTransform = unit.transform;

        int[] coordinates = HexCoordToArrayindex(coords);

        Vector3 position;
        position.x = coordinates[0] * HexWidth - _xOffset;
        position.y = this.transform.position.y;
        position.z = (coordinates[1] + (coordinates[0] * 0.5f) - coordinates[0] / 2) * HexHeight - _yOffset;
        unitTransform.position = position;

        Vector3 rotation;
        rotation.x = 270;
        rotation.y = 0;
        rotation.z = unitTransform.rotation.eulerAngles.z;
        unitTransform.rotation = Quaternion.Euler(rotation);
    }

    public uint GetNumberOfPlayers()
    {
        return _levelManager.NumberOfPlayers;
    }
}
