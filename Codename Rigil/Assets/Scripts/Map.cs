using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

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

    private const float HexWidth = 1.0f;
    private const float HexHeight = 1.0f;

    [SerializeField]
    private GameObject _hexPrefab;

    [SerializeField]
    private MapType _mapShape = MapType.Quad;

    [SerializeField]
    private uint[] _dimensions = new uint[] { 10, 10 };

    LevelManager _levelManager;

    private void Start()
    {
        _levelManager = this.transform.GetComponent<LevelManager>();
    }

    public uint GetNumberOfPlayers()
    {
        return _levelManager.NumberOfPlayers;
    }

    public void HexSelected(HexCooridnates coords)
    {

    }
}
