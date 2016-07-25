using UnityEngine;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class RandomizeSensor : CreatureSensor
{
    private static float _sensorModifier = 0.5f;

    private float _actionCost;

    private float _minValue;
    private float _maxValue;
    
    public RandomizeSensor(Creature targetCreature) : base(targetCreature)
    {
        _minValue = 0;
        _maxValue = 1;

        _actionCost = _sensorModifier * NetworkSettings.BaseActionCost;
    }

    public override float GetInput()
    {
        _creature.UseEnergy(_actionCost);

        return Random.Range(_minValue, _maxValue);
    }

    public override void RandomizeVariables()
    {
        _minValue = Random.Range(0, 2);
        _maxValue = Random.Range(2, 11);
    }

    public override CreatureSensor PerfectCopy(Creature targetCreature)
    {
        var newSensor = new RandomizeSensor(targetCreature);

        newSensor._minValue = this._minValue;
        newSensor._maxValue = this._maxValue;

        return newSensor;
    }
    public override CreatureSensor ImperfectCopy(Creature targetCreature, float errorChance, float minDeviationPercent, float maxDeviationPercent)
    {
        var newSensor = (RandomizeSensor)PerfectCopy(targetCreature);

        newSensor._minValue = ValueMutator.MutatePercentage(this._minValue, errorChance, minDeviationPercent, maxDeviationPercent);
        newSensor._maxValue = ValueMutator.MutatePercentage(this._maxValue, errorChance, minDeviationPercent, maxDeviationPercent);

        return newSensor;
    }
}