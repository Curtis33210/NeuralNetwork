using UnityEngine;
using System.Collections.Generic;

public class BasicMovement : CreatureAction
{
    public static float MovementCost = 0.5f;

    private float _actionCost;

    private Direction _creatureSide;

    public BasicMovement(Creature targetCreature) : base(targetCreature)
    {
        if (targetCreature == null)
            return;

        _actionCost = NetworkSettings.BaseActionCost * MovementCost * _creature.Mass;
    }

    public override void DoOutput(float outputStrength)
    {
        _creature.UseEnergy(NetworkSettings.BaseActionCost);

        if (outputStrength < 0.5f)
            return;

        switch (_creatureSide) {
            case Direction.North:
                _creature.MoveForward();
                break;
            case Direction.East:
                _creature.MoveRight();
                break;
            case Direction.South:
                _creature.MoveBackward();
                break;
            case Direction.West:
                _creature.MoveLeft();
                break;
        }

        _creature.UseEnergy(_actionCost);
    }

    public override void RandomizeVariables()
    {
        _creatureSide = (Direction)Random.Range(0, 4);
    }

    public override CreatureAction PerfectCopy(Creature targetCreature)
    {
        var newMovement = new BasicMovement(targetCreature);
        
        newMovement._creatureSide = _creatureSide;

        return newMovement;
    }

    public override CreatureAction ImperfectCopy(Creature targetCreature, float errorChance, float minDeviation, float maxDeviation)
    {
        var newMovement = new BasicMovement(targetCreature);

        newMovement._creatureSide = (Direction) ValueMutator.MutateFlat((int)_creatureSide, errorChance, -1, 1);
        
        return newMovement;
    }
}