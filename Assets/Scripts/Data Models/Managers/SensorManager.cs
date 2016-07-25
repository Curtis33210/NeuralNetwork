using UnityEngine;
using System.Collections.Generic;
using NeuralNetwork;
using System;

public class SensorManager : InputManager
{
    public SensorManager(int maxNetworks) : base(maxNetworks) { }
    
    public override NetworkData GetInputData()
    {
        for (int networkID = 0; networkID < _activeNetworks; networkID++) {
            var numOfSensors = _inputs[networkID].Length;

            for (int sensorIndex = 0; sensorIndex < numOfSensors; sensorIndex++) {
                _inputData[networkID, sensorIndex] = _inputs[networkID][sensorIndex].GetInput();
            }
        }

        return _inputData;
    }
}