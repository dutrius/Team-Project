using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    private SteamVR_TrackedObject _trackedObj;
    private GameObject _collidingObject;
    private GameObject _objectInHand;
    [SerializeField]
    private GameObject _levelObject;
    private Map _map;

    const float DEFAULTMOVEMENT = 2.0f;
    const int FACTIONTURN = 0;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)_trackedObj.index); }
    }

    void Awake()
    {
        _trackedObj = GetComponent<SteamVR_TrackedObject>();
        _map = _levelObject.GetComponent<Map>();
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!_collidingObject)
        {
            return;
        }

        _collidingObject = null;
    }
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void GrabObject()
    {
        _objectInHand = _collidingObject;
        _collidingObject = null;
        FixedJoint joint = AddFixedJoint();

        switch (_objectInHand.tag)
        {
            case "HeightControllers":
                joint.connectedBody = _objectInHand.transform.parent.GetComponent<Rigidbody>();
                break;
            case "Unit":
                _map.HighlightCells(_map.GetMoveableHexes(_map.FindClosestHex(_objectInHand), DEFAULTMOVEMENT, FACTIONTURN));
                _map.SelectCell(_map.FindClosestHex(_objectInHand));
                _objectInHand.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                joint.connectedBody = _objectInHand.GetComponent<Rigidbody>();
                break;
            default:
                joint.connectedBody = _objectInHand.GetComponent<Rigidbody>();
                break;
        }
    }
    

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            switch (_objectInHand.tag)
            {
                case "HeightControllers":
                    break;
                case "Unit":
                    _map.SnapUnit(_objectInHand, _map.FindClosestHex(_objectInHand));
                    _objectInHand.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    break;
                default:
                    _objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
                    _objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
                    break;
            }
        }
        _objectInHand = null;
    }

    private void SetCollidingObject(Collider col)
    {
        if (_collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        _collidingObject = col.gameObject;
    }

    void Update ()
    {
        if (Controller.GetHairTriggerDown())
        {
            if (_collidingObject)
            {
                GrabObject();
            }
        }
        
        if (Controller.GetHairTriggerUp())
        {
            if (_objectInHand)
            {
                ReleaseObject();
            }
        }
    }
}
