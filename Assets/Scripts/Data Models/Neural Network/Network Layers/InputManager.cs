using UnityEngine;
using System.Collections.Generic;
using NeuralNetwork;

public abstract class InputManager
{
    protected NetworkData _inputData;

    protected IInput[][] _inputs;

    protected int _activeNetworks;
    
    protected InputManager(int maxNetworks)
    {
        _inputData = new NetworkData(maxNetworks);

        _inputs = new IInput[maxNetworks][];

        _activeNetworks = 0;
    }

    public int AddNetwork(IInput[] inputs)
    {
        var networkID = _activeNetworks++;

        _inputs[networkID] = inputs;

        _inputData.AddNetwork(networkID, inputs.Length);

        return networkID;

    }
    public void RemoveNetwork(int networkID)
    {
        _activeNetworks--;

        if (_activeNetworks != networkID) {
            _inputs[networkID] = _inputs[_activeNetworks];
            _inputData.ReplaceNetwork(networkID, _activeNetworks);
        }

        _inputs[_activeNetworks] = null;
    }

    public IInput[] GetNetwork(int networkID)
    {
        return _inputs[networkID];
    }

    public abstract NetworkData GetInputData();
}