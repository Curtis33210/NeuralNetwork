using UnityEngine;
using System.Collections.Generic;
using System;
using NeuralNetwork;

using Random = UnityEngine.Random;

public class World
{
    private Tile[] _worldTiles;

    private Creature[] _creatures;
    private Creature[] _activeCreatures;

    private int _activeCreaturesCount;
    
    private Action<Creature> _onCreatureAddedToWorld;
    private Action<Creature> _onCreatureRemovedFromWorld;

    public int MapWidth { get; private set; }
    public int MapHeight { get; private set; }

    private float _timeSinceLastSecond;

    public World(int sizeX, int sizeY, int maxCreatures)
    {
        MapWidth = sizeX;
        MapHeight = sizeY;

        _creatures = new Creature[maxCreatures];
        _activeCreatures = new Creature[maxCreatures];

        CreateTiles(sizeX, sizeY);
    }
    
    public Tile GetTileAt(int x, int y)
    {
        if (x >= MapWidth || y >= MapHeight)
            return null;
        if (x < 0 || y < 0)
            return null;

        return _worldTiles[(MapWidth * y) + x];
    }
    public Tile GetTileFromWorldPos(int x, int y)
    {
        if (x >= MapWidth || y >= MapHeight)
            return null;
        if (x < 0 || y < 0)
            return null;
        
        return _worldTiles[(MapWidth * y) + x];
    }

    public void AddCreatureToWorld(Creature newCreature)
    {
        if (AddCreatureErrorCheck(newCreature) == false)
            return;

        _creatures[newCreature.ID] = newCreature;
        _activeCreatures[_activeCreaturesCount] = newCreature;

        _activeCreaturesCount++;
        
        newCreature.RegisterCreatureDiedCallback(RemoveCreatureFromWorld);

        if (_onCreatureAddedToWorld != null)
            _onCreatureAddedToWorld(newCreature);
    }

    public Creature CreateNewCreature(float mass, float energy, Color color)
    {
        var creatureID = GetAvaliableCreatureID();
        
        var creature = new Creature(creatureID, mass, energy, color);

        if (creatureID == -1)
            Debug.LogError("Creature ID is invalid. This may be because there are no more avaliable spots left.");

        return creature;
    }
    public Creature CreateNewCreature(float mass, float energy)
    {
        var creatureColor = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));

        return CreateNewCreature(mass, energy, creatureColor);
    }
    public Creature CreateNewCreature()
    {
        var mass = Random.Range(0, 1.0f);
        var energy = Random.Range(0, 50000f);
        var creatureColor = new Color(Random.Range(0, 1.0f), Random.Range(0, 1.0f), Random.Range(0, 1.0f));

        return CreateNewCreature(mass, energy, creatureColor);
    }

    public void RemoveCreatureFromWorld(Creature creature)
    {
        if (creature == null) {
            Debug.LogError("Creatures is null. Perhaps this creature has already been removed?");
            return;
        }

        _activeCreaturesCount--;

        for (int i = 0; i <= _activeCreaturesCount; i++) {
            if (_activeCreatures[i] == creature) {
                _activeCreatures[i] = _activeCreatures[_activeCreaturesCount];
                _activeCreatures[_activeCreaturesCount] = null;
            }
        }

        _creatures[creature.ID] = null;
         
        if (_onCreatureRemovedFromWorld != null)
            _onCreatureRemovedFromWorld(creature);
    }
    public void RemoveCreatureFromWorld(int creatureID)
    {
        RemoveCreatureFromWorld(_creatures[creatureID]);
    }

    public void RemoveAllActiveCreatures()
    {
        for (int i = 0; i < _creatures.Length; i++) {
            if (_creatures[i] != null)
                _creatures[i].UseEnergy(Mathf.Infinity);
        }
    }
    
    public Creature[] GetAllActiveCreatures()
    {
        var activeCreatures = new List<Creature>();

        for (int i = 0; i < _creatures.Length; i++) {
            if (_creatures[i] != null)
                activeCreatures.Add(_creatures[i]);
        }

        return activeCreatures.ToArray();
    }
    
    public void UpdateWorld(float dt)
    {
        _timeSinceLastSecond += dt;
        
        if (_timeSinceLastSecond > 1) {
            OneSecondPast();
            _timeSinceLastSecond -= 1;
        }
       
        for (int i = 0; i < _creatures.Length; i++) {
            if (_creatures[i] == null)
                continue;

            _creatures[i].Update();
        }
    }
    private void OneSecondPast()
    {
        var networkUpdates = NetworkSettings.NetworkUpdatesPerSecond;

        for (int i = 0; i < _activeCreaturesCount; i++) {
            _activeCreatures[i].AddLifeTime(networkUpdates);
        }
    }

    private int GetAvaliableCreatureID()
    {
        for (int i = 0; i < _creatures.Length; i++) {
            if (_creatures[i] == null)
                return i;
        }

        return -1;
    }

    private bool AddCreatureErrorCheck(Creature creature)
    {
        if (creature == null) {
            Debug.LogError("Creature being added is null. This cannot happen");
            return false;
        } else if (creature.ID < 0) {
            Debug.LogError("Creature being added has a negative ID. This cannot happen");
            return false;
        } else if (creature.ID >= _creatures.Length) {
            Debug.LogError("Creature being added has an ID greater than the max allowed creatures. This cannot happen");
            return false;
        }

        return true;
    }

    private void CreateTiles(int sizeX, int sizeY)
    {
        _worldTiles = new Tile[sizeY * sizeX];

        for (int y = 0; y < sizeY; y++) {
            for (int x = 0; x < sizeX; x++) {
                var newTile = new Tile(x, y);

                newTile.ChangeTileType(TileType.Rock);

                _worldTiles[(sizeX * y) + x] = newTile;
            }
        }

        Debug.Log("Created world with " + sizeX * sizeY + " tiles.");
    }
    
    #region Callbacks

    public void RegisterCreatureAddedToWorldCallback(Action<Creature> callback)
    {
        _onCreatureAddedToWorld += callback;
    }
    public void RegisterCreatureRemovedFromWorldCallback(Action<Creature> callback)
    {
        _onCreatureRemovedFromWorld += callback;
    }

    #endregion
}