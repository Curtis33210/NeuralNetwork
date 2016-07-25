using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class CreatureAction : IOutput
{
    protected Creature _creature;
    
    protected CreatureAction(Creature targetCreature)
    {
        _creature = targetCreature;
    }

    public void UpdateCreature(Creature newCreature)
    {
        _creature = newCreature;
    }
    public abstract void DoOutput(float outputStrength);
    public abstract void RandomizeVariables();
    
    public abstract CreatureAction PerfectCopy(Creature targetCreature);
    public abstract CreatureAction ImperfectCopy(Creature targetCreature, float errorChance, float minDeviationPercent, float maxDeviationPercent);
}