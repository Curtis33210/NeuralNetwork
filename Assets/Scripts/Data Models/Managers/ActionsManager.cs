using UnityEngine;
using System.Collections.Generic;
using NeuralNetwork;
using System;

public class ActionManager : OutputManager
{
    public ActionManager(int maxNetworks) : base(maxNetworks) { }

    public override void ProcessOutputs(NetworkData outputData)
    {
        if (outputData == null)
            return;

        var numNetworks = _activeNetworks;

        for (int networkID = 0; networkID < numNetworks; networkID++) {
            var numOutputs = _outputs[networkID].Length;
            var networkOutpus = _outputs[networkID];

            for (int outputID = 0; outputID < numOutputs; outputID++) {
                networkOutpus[outputID].DoOutput(outputData[networkID, outputID]);
            }
        }
    }
}