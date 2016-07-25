using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NeuralNetwork;

public class DNACloner : CreatureAction
{
    public DNACloner(Creature targetCreature) : base(targetCreature) { }

    public override void DoOutput(float outputStrength)
    {
        _creature.UseEnergy(NetworkSettings.BaseActionCost);

        if (outputStrength > 0.5f || _creature.CanReplicate() == false)
            return;

        Debug.Log("Creature cloned");

        _creature.UseEnergy(_creature._startingEnergy - (_creature._startingEnergy) * 0.1f);

        var activeWorld = WorldSettings.ActiveWorld;
        var activeNetwork = NetworkSettings.ActiveNetwork;
        var linkManager = WorldSettings.LinkManager;
        
        int oldNetworkID = linkManager.GetNetworkID(_creature.ID);
        var oldNetwork = activeNetwork.GetNetwork(oldNetworkID);

        var newNetwork = oldNetwork.CloneNetwork(0.05f);

        float mass = linkManager.GetNetworkMass(newNetwork.Inputs.Length, newNetwork.Outputs.Length, newNetwork.HiddenLayer.Length);
        float energy = linkManager.GetNetworkEnergy(newNetwork.Inputs.Length, newNetwork.Outputs.Length, newNetwork.HiddenLayer.Length);
        
        var creature = activeWorld.CreateNewCreature(mass, energy, ValueMutator.MutateFlat(_creature.Color, 0.05f, -0.1f, 0.1f));

        if (PlaceCreature(creature) == false)
            return;
        
        newNetwork.UpdateTargetCreature(creature);

        var networkID = activeNetwork.AddNetwork(newNetwork);

        linkManager.AddLink(creature, networkID);
    }

    public override CreatureAction ImperfectCopy(Creature targetCreature, float errorChance, float minDeviationPercent, float maxDeviationPercent)
    {
        return new DNACloner(targetCreature);
    }
    public override CreatureAction PerfectCopy(Creature targetCreature)
    {
        return new DNACloner(targetCreature);
    }

    public override void RandomizeVariables() { } 

    private bool PlaceCreature(Creature creature)
    {
        var newplacement = PlacementOffset(_creature.PosX, _creature.PosY);

        if (newplacement == new Vector2(-1, -1)) {
            return false;
        }

        creature.MoveTo(newplacement);

        WorldSettings.ActiveWorld.AddCreatureToWorld(creature);

        return true;
    }
    private Vector2 PlacementOffset(int xPos, int yPos)
    {
        var activeWorld = WorldSettings.ActiveWorld;

        for (int y = -1; y < 2; y++) {
            for (int x = -1; x < 2; x++) {
                if (xPos + x >= WorldSettings.WorldWidth || yPos + y >= WorldSettings.WorldHeight ||
                    xPos + x < 0 || yPos + y < 0)
                    continue;

                if (activeWorld.GetTileAt(xPos + x, yPos + y).ContainsCreature() == false)
                    return new Vector2(xPos + x, yPos + y);
            }
        }

        return new Vector2(-1, -1);
    }
}