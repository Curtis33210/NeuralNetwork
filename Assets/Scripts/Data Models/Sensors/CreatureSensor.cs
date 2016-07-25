using UnityEngine;
using System.Collections.Generic;

public abstract class CreatureSensor : IInput
{
    protected Creature _creature;

    protected CreatureSensor(Creature targetCreature)
    {
        _creature = targetCreature;
    }

    public void UpdateCreature(Creature newCreature)
    {
        _creature = newCreature;
    }

    public abstract float GetInput();
    public abstract void RandomizeVariables();

    public abstract CreatureSensor PerfectCopy(Creature targetCreature);
    public abstract CreatureSensor ImperfectCopy(Creature targetCreature, float errorChance, float minDeviationPercent, float maxDeviationPercent);
}

