using UnityEngine;
using System.Collections.Generic;
using System;

public enum Direction
{
    North,
    East,
    South, 
    West
}

public class Creature
{
    private static float ReplicatePercentage = 0.1f;

    private Action<Creature> _onPositionChanged;
    private Action<Creature> _onDirectionChanged;
    private Action<Creature> _onCreatureDied;
    
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    
    public Direction FacingDirection { get; private set; }
    public int ID { get; private set; }

    public float Energy { get; private set; }
    public float Mass { get; private set; }
    public Color Color { get; private set; }
    
    public float LifeTime { get; private set; }

    public float _startingEnergy;

    public int DistanceTravelled { get; private set; }
    public int TimesRotated { get; private set; }
    
    private Vector2 _finalMoveVector;
    private int _finalRotationAmount;
    private float _totalEnergyUseage;

    public Creature(int id, float mass, float energy, Color color)
    {
        ID = id;
        Mass = mass;
        Energy = energy;
        Color = color;

        _startingEnergy = Energy;
    }
    public Creature(Creature original)
    {
        ID = original.ID;
        Mass = original.Mass;
        Energy = original.Energy;
        Color = original.Color;

        _startingEnergy = Energy;
    }

    public void AddMovement(Vector2 moveVector)
    {
        _finalMoveVector += moveVector;
    }
    public void AddMovement(int moveX, int moveY)
    {
        AddMovement(new Vector2(moveX, moveY));
    }

    public void MoveForward()
    {
        switch (FacingDirection) {
            case Direction.North:
                AddMovement(0, 1);
                break;
            case Direction.East:
                AddMovement(-1, 0);
                break;
            case Direction.South:
                AddMovement(0, -1);
                break;
            case Direction.West:
                AddMovement(1, 0);
                break;
        }
    }
    public void MoveBackward()
    {
        switch (FacingDirection) {
            case Direction.North:
                AddMovement(0, -1);
                break;
            case Direction.East:
                AddMovement(1, 0);
                break;
            case Direction.South:
                AddMovement(0, 1);
                break;
            case Direction.West:
                AddMovement(-1, 0);
                break;
        }
    }
    public void MoveLeft()
    {
       switch (FacingDirection) {
            case Direction.North:
                AddMovement(-1, 0);
                break;
            case Direction.East:
                AddMovement(0, 1);
                break;
            case Direction.South:
                AddMovement(1, 0);
                break;
            case Direction.West:
                AddMovement(0, -1);
                break;
        }
    }
    public void MoveRight()
    {
        switch (FacingDirection) {
            case Direction.North:
                AddMovement(1, 0);
                break;
            case Direction.East:
                AddMovement(0, -1);
                break;
            case Direction.South:
                AddMovement(-1, 0);
                break;
            case Direction.West:
                AddMovement(0, 1);
                break;
        }
    }

    public void MoveTo(Vector2 moveVector)
    {
        MoveTo(Mathf.RoundToInt(moveVector.x), Mathf.RoundToInt(moveVector.y));
    }
    public void MoveTo(int targetX, int targetY)
    {
        MoveBy(targetX - PosX, targetY - PosY);
        var targetTile = WorldSettings.ActiveWorld.GetTileAt(PosX, PosY);
        
        if (targetTile.ContainsCreature()) 
            Debug.LogError("Creature already contained in the area this creature is being moved to");

        targetTile.AddCreature(this);
    }

    public void FaceDirection(Direction targetDirection)
    {
        FacingDirection = targetDirection;

        _onDirectionChanged(this);
    }
    public void Rotate(int rotationAmount)
    {
        _finalRotationAmount += rotationAmount;
    }

    public void UseEnergy(float amount)
    {
        if (Energy <= 0) // Already dead
            return;

        _totalEnergyUseage += amount;
    }
    public void RecieveEnergy(float amount)
    {
        Energy += amount;
    }
    public void Kill()
    {
        WorldSettings.ActiveWorld.GetTileAt(PosX, PosY).RemoveCreature();
        
        if (_onCreatureDied != null)
            _onCreatureDied(this);
    }

    public void Update()
    {
        FinalizeRotateCreature((int)Mathf.Clamp01(_finalRotationAmount));
        _finalRotationAmount = 0;

        TryMoveCreature();
        
        FinalizeEnergyUseage();
    }

    public void AddLifeTime(float amount)
    {
        LifeTime += amount;
    }

    public bool CanReplicate()
    {
        if (Energy > _startingEnergy * (1 + ReplicatePercentage))
            return true;
        return false;
    }
    
    private void MoveBy(int moveX, int moveY)
    {
        if (moveX == 0 && moveY == 0)
            return;

        PosX += moveX;
        PosY += moveY;

        if (_onPositionChanged != null)
            _onPositionChanged(this);
    }
    private void TryMoveCreature()
    {
        if (_finalMoveVector == Vector2.zero)
            return;

        _finalMoveVector.Normalize();

        var xMove = Mathf.RoundToInt(_finalMoveVector.x);
        var yMove = Mathf.RoundToInt(_finalMoveVector.y);

        var targetTile = WorldSettings.ActiveWorld.GetTileAt(PosX + xMove, PosY + yMove);
        
        if (targetTile != null && targetTile.ContainsCreature() == false) {
            WorldSettings.ActiveWorld.GetTileAt(PosX, PosY).RemoveCreature();

            MoveBy(xMove, yMove);
            targetTile.AddCreature(this);

            DistanceTravelled += Mathf.Abs(xMove) + Mathf.Abs(yMove);
        }

        _finalMoveVector = Vector2.zero;
    }

    private void FinalizeRotateCreature(int rotationAmount)
    {
        if (rotationAmount == 0)
            return;

        int directionInt = (int)FacingDirection;

        directionInt = (directionInt + rotationAmount) % 4;

        FacingDirection = (Direction)directionInt;

        TimesRotated++;

        _onDirectionChanged(this);
    }

    private void FinalizeEnergyUseage()
    {
        Energy -= _totalEnergyUseage;

        if (Energy <= 0)
            Kill();

        _totalEnergyUseage = 0;
    }

    public void RegisterPositionChangedCallback(Action<Creature> callback)
    {
        _onPositionChanged += callback;
    }
    public void RegisterDirectionChangedCallback(Action<Creature> callback)
    {
        _onDirectionChanged += callback;
    }
    public void RegisterCreatureDiedCallback(Action<Creature> callback)
    {
        _onCreatureDied += callback;
    }
}