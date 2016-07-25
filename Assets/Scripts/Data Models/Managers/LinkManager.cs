using UnityEngine;
using System.Collections.Generic;
using System;
using NeuralNetwork;

/// <summary>
/// Manages the links between the creatures and the neural networks. 
/// This is self-managing once a link is added. 
/// </summary>
public class LinkManager
{
    private int[] _networkIDByCreatureID;
    private Creature[] _creatureByNetworkID;

    private NeuralNetworkManager _networkManager;
    
    public LinkManager(int maxCreatures) 
    {
        _networkManager = NetworkSettings.ActiveNetwork;

        _networkIDByCreatureID = new int[maxCreatures];
        _creatureByNetworkID = new Creature[maxCreatures];

        WorldSettings.LinkManager = this;
        WorldSettings.ActiveWorld.RegisterCreatureRemovedFromWorldCallback(OnCreatureDied);
    }
    
    public void AddLink(Creature creature, int networkID)
    {
        _creatureByNetworkID[networkID] = creature;
        _networkIDByCreatureID[creature.ID] = networkID;
    }

    public int GetNetworkID(int creatureID)
    {
        return _networkIDByCreatureID[creatureID];
    }

    public int GetNetworkID(Creature creature)
    {
        return GetNetworkID(creature.ID);
    }

    public Creature GetCreature(int networkID)
    {
        return _creatureByNetworkID[networkID];
    }

    public int GetCreatureID(int networkID)
    {
        return GetCreature(networkID).ID;
    }

    public float GetNetworkMass(int numSensors, int numActions, int numHiddenNodes)
    {
        return (numSensors * NetworkSettings.SensorWeight) + (numSensors * NetworkSettings.ActionWeight) + (numSensors * NetworkSettings.HiddenNodeWeight);
    }
    public float GetNetworkMass(NeuralNetwork.NetworkContainer network)
    {
        var numSensors = network.Inputs.Length;
        var numActions = network.HiddenLayer.Length;
        var numHiddenNodes = network.Outputs.Length;

        return GetNetworkMass(numSensors, numActions, numHiddenNodes);
    }

    public float GetNetworkEnergy(int numSensors, int numActions, int numHiddenNodes)
    {
        return (numSensors * NetworkSettings.SensorEnergy) + (numSensors * NetworkSettings.ActionEnergy) + (numSensors * NetworkSettings.HiddenNodeEnergy);
    }
    public float GetNetworkEnergy(NeuralNetwork.NetworkContainer network)
    {
        var numSensors = network.Inputs.Length;
        var numActions = network.HiddenLayer.Length;
        var numHiddenNodes = network.Outputs.Length;

        return GetNetworkEnergy(numSensors, numActions, numHiddenNodes);
    }

    private void OnCreatureDied(Creature creature)
    {
        var deletedNetworkID = GetNetworkID(creature);

        var lastNetworkID = _networkManager.LastActiveNetwork;
        var lastCreature = GetCreature(lastNetworkID);
        
        _networkManager.RemoveNetwork(deletedNetworkID);

        _networkIDByCreatureID[lastCreature.ID] = deletedNetworkID;
        _creatureByNetworkID[deletedNetworkID] = lastCreature;

        _networkIDByCreatureID[creature.ID] = -1;
        _creatureByNetworkID[lastNetworkID] = null;
    }
}
