using UnityEngine;
using System.Collections.Generic;

public class BasicRotator : CreatureAction
{
    private static float RotationCost = 0.2f;

    private float _actionCost;
    private int _rotationDirection;
    
    public BasicRotator(Creature targetCreature) : base(targetCreature)
    {
        _rotationDirection = 1;

        if (targetCreature == null)
            return;

        _actionCost = NetworkSettings.BaseActionCost * RotationCost * _creature.Mass;
    }

    public override void DoOutput(float outputStrength)
    {
        _creature.UseEnergy(NetworkSettings.BaseActionCost);
        
        if (outputStrength < 0.5f)
            return;

        _creature.Rotate(_rotationDirection);

        _creature.UseEnergy(_actionCost);
    }

    public override void RandomizeVariables()
    {
        var random = Random.Range(0, 2);

        if (random == 0)
            _rotationDirection = 1;
        else
            _rotationDirection = -1;
    }

    public override CreatureAction PerfectCopy(Creature targetCreature)
    {
        var newRotator = new BasicRotator(targetCreature);

        newRotator._rotationDirection = _rotationDirection;

        return newRotator;
    }

    public override CreatureAction ImperfectCopy(Creature targetCreature, float errorChance, float minDeviationPercent, float maxDeviationPercent)
    {
        var newRotator = (BasicRotator)PerfectCopy(targetCreature);
        
        var chance = Random.Range(0, 1.0f);

        if (chance > (1 - errorChance))
            newRotator.InvertRotation();
        
        return newRotator;
    }
    
    private void InvertRotation()
    {
        if (_rotationDirection == 1)
            _rotationDirection = -1;
        else
            _rotationDirection = 1;
    }
}