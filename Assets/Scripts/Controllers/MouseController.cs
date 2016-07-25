using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Text;

public class MouseController : MonoBehaviour
{
    [SerializeField]
    private WorldController _worldController;

    [SerializeField]
    private Text _tileInfo;
    [SerializeField]
    private Text _creatureInfo;
    [SerializeField]
    private Transform _camera;

    private Vector3 _previousMousePos;
    private World _world;

    private Tile _currTile;
    private Creature _targetCreature;

    private StringBuilder sb;
    private string defaultInfo;

    private string[] _setStrings;

    private bool _followTarget;

    private void Start()
    {
        _world = _worldController.ActiveWorld;

        defaultInfo = DisplayDefaultCreatureInfo();
        

        _followTarget = false;
    }

    private void Update()
    {
        if (_world == null)
            return;
        
        if (_targetCreature != null && _followTarget) {
            _camera.position = Vector3.Lerp(_camera.position, new Vector3(_targetCreature.PosX, _targetCreature.PosY, -10), Time.deltaTime);
            
        }
        
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.mousePosition != _previousMousePos) 
            OnMouseMoved();
        
        if (Input.GetMouseButtonDown(0) && _currTile != null) {
            _followTarget = false;
            _targetCreature = _currTile.GetCreature();

            if (_targetCreature != null)
                _targetCreature.RegisterCreatureDiedCallback(OnCreatureDied);
        }

        DisplayCreatureInfo(_targetCreature);

        _previousMousePos = Input.mousePosition;
    }

    private void OnMouseMoved()
    {
        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var tile = _world.GetTileFromWorldPos(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));
        
        if (tile == _currTile)
            return;

        DisplayTileInfo(tile);
    }

    private void DisplayTileInfo(Tile tile)
    {
        if (tile == null) {
            _tileInfo.text = "Tile Type:\n";
            _tileInfo.text += "Position:";
        } else {
            _tileInfo.text = "Tile Type: " + tile.Type + "\n";
            _tileInfo.text += "Position: (" + tile.PosX + ", " + tile.PosY + ")";
        }

        _currTile = tile;
    }

    private void DisplayCreatureInfo(Creature creature)
    {
        if (creature == null) {
            _creatureInfo.text = defaultInfo;
            return;
        }

        sb = new StringBuilder();

        sb.Append("Creature ID: ");
        sb.Append(creature.ID);
        sb.Append("\n");

        sb.Append("Color: ");
        sb.Append(creature.Color);
        sb.Append("\n");

        sb.Append("Direction: ");
        sb.Append(creature.FacingDirection);
        sb.Append("\n");

        sb.Append("Energy: ");
        sb.Append(creature.Energy);
        sb.Append("\n");

        sb.Append("Mass: ");
        sb.Append(creature.Mass);
        sb.Append("\n");

        sb.Append("Lifetime: ");
        sb.Append(creature.LifeTime);
        sb.Append("\n");

        sb.Append("Distance Travelled: ");
        sb.Append(creature.DistanceTravelled);
        sb.Append("\n");

        sb.Append("Times Rotated: ");
        sb.Append(creature.TimesRotated);
        sb.Append("\n");

        _creatureInfo.text = sb.ToString();
    }

    private void OnCreatureDied(Creature deadCreature)
    {
        DisplayDefaultCreatureInfo();

        _followTarget = false;

        _targetCreature = null;
    }

    private string DisplayDefaultCreatureInfo()
    {
        sb = new StringBuilder();

        sb.Append("Creature ID: ");
        sb.Append("\n");

        sb.Append("Color: ");
        sb.Append("\n");

        sb.Append("Direction: ");
        sb.Append("\n");

        sb.Append("Energy: ");
        sb.Append("\n");

        sb.Append("Mass: ");
        sb.Append("\n");

        sb.Append("Lifetime: ");
        sb.Append("\n");

        sb.Append("Distance Travelled: ");
        sb.Append("\n");

        sb.Append("Times Rotated: ");
        sb.Append("\n");

        return sb.ToString();
    }

    public void FollowTarget()
    {
        if (_targetCreature == null)
            return;

        _followTarget = !_followTarget;
    }
}