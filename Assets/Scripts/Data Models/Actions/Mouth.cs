using UnityEngine;
using System.Collections.Generic;
using System;

public class Mouth : CreatureAction
{
    public static float EatingCost = 0.5f;
    public static int EatingAmount = 2500;

    private float _actionCost;


    public Mouth(Creature targetCreature) : base(targetCreature)
    {
        if (targetCreature == null)
            return;

        _actionCost = NetworkSettings.BaseActionCost * EatingCost * _creature.Mass;
    }

    public override void DoOutput(float outputStrength)
    {
        _creature.UseEnergy(NetworkSettings.BaseActionCost);
        
        var food = WorldSettings.ActiveWorld.GetTileAt(_creature.PosX, _creature.PosY).GetFood();

        if (food != null)
            _creature.RecieveEnergy(food.GetEnergy(EatingAmount));

        _creature.UseEnergy(_actionCost);
    }

    public override CreatureAction ImperfectCopy(Creature targetCreature, float errorChance, float minDeviationPercent, float maxDeviationPercent)
    {
        return PerfectCopy(targetCreature);
    }

    public override CreatureAction PerfectCopy(Creature targetCreature)
    {
        return new Mouth(targetCreature);
    }

    public override void RandomizeVariables() { }
}