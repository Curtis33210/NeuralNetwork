using UnityEngine;
using System.Collections.Generic;
using System;

public enum TileType
{
    None,
    Dirt,
    Grass, 
    Rock,
    Count
}

public class Tile
{
    private Action<Tile> _onTileTypeChanged;
    private Action<Tile> _onFoodAdded;
    private Action<Tile> _onFoodRemoved;

    public TileType Type { get; private set; }
    
    public int PosX { get; private set; }
    public int PosY { get; private set; }

    private Food _food;
    private Creature _creature;

    public Tile(int x, int y)
    {
        PosX = x;
        PosY = y;
    }
    
    public void ChangeTileType(TileType newType)
    {
        Type = newType;

        if (_onTileTypeChanged == null)
            return;

        _onTileTypeChanged(this);
    }

    public void RemoveFood()
    {
        if (_food == null)
            return;

        if (_onFoodRemoved != null) {
            _onFoodRemoved(this);
        }

        _food = null;
    }

    public void AddFood(Food food)
    {
        if (TryAddFood(food) == false) {
            Debug.LogError("Cannot add food to this tile. Food already Exists here");
        }
    }
    public bool TryAddFood(Food food)
    {
        if (_food != null)
            return false;

        _food = food;
        _food.RegisterOnFoodOutOfEnergyCallback(OnFoodOutOfEnergy);

        if (_onFoodAdded != null)
            _onFoodAdded(this);

        return true;
    }

    public Food GetFood()
    {
        return _food;
    }

    public void AddCreature(Creature creature)
    {
        if (TryAddCreature(creature) == false) {
            Debug.LogError("Could not add Creature");
        }
    }
    public bool TryAddCreature(Creature creature)
    {
        if (ContainsCreature())
            return false;

        _creature = creature;
        return true;
    }

    public void RemoveCreature()
    {
        _creature = null;
    }

    public Creature GetCreature()
    {
        return _creature;
    }
    public bool ContainsCreature()
    {
        if (_creature != null) 
            return true;
        return false;
    }

    private void OnFoodOutOfEnergy(Food food)
    {
        if (food == _food)
            RemoveFood();
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        _onTileTypeChanged += callback;
    }
    public void RegisterOnFoodAddedCallback(Action<Tile> callback)
    {
        _onFoodAdded += callback;
    }
    public void RegisterOnFoodRemovedCallback(Action<Tile> callback)
    {
        _onFoodRemoved += callback;
    }
}