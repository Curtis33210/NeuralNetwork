using UnityEngine;
using System.Collections.Generic;
using NeuralNetwork;

public abstract class OutputManager
{
    protected IOutput[][] _outputs;

    protected int _activeNetworks;

    protected OutputManager(int maxNetworks)
    {
        _outputs = new IOutput[maxNetworks][];

        _activeNetworks = 0;
    }

    public int AddNetwork(IOutput[] outputs)
    {
        var networkID = _activeNetworks++;

        _outputs[networkID] = outputs;
        
        return networkID;

    }
    public void RemoveNetwork(int networkID)
    {
        _activeNetworks--;

        if (_activeNetworks != networkID)
            _outputs[networkID] = _outputs[_activeNetworks];

        _outputs[_activeNetworks] = null;
    }

    public IOutput[] GetNetwork(int networkID)
    {
        return _outputs[networkID];
    }

    public abstract void ProcessOutputs(NetworkData outputData);
}