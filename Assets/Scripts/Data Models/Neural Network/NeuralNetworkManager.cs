using UnityEngine;
using System.Collections.Generic;
using System;

namespace NeuralNetwork
{
    /// <summary>
    /// Data size manager of the Nerual network. This manages all the layers and any interactions between the layers.\n
    /// All the layers in this manager have no direct interaction to the Unity game world. Any interaction is done through the
    /// Input and Output neurons. 
    /// </summary>
    public class NeuralNetworkManager
    {
        private int _maxNetworks;
        private int _activeNetworks;

        public int ActiveNetworks { get { return _activeNetworks; } }
        public int LastActiveNetwork { get { return _activeNetworks - 1; } }

        private object _networkLock;

        private InputManager _inputManager;
        private OutputManager _outputManager;
        
        private HiddenLayer _hiddenLayer; // May be changed to an array later on to allow for varying amounts of hidden layers
        private HiddenLayer _outputLayer;

        public NeuralNetworkManager(int maxNetworks, InputManager inputManager, OutputManager outputManager)
        {
            _maxNetworks = maxNetworks;

            _hiddenLayer = new HiddenLayer(maxNetworks);
            _outputLayer = new HiddenLayer(maxNetworks);

            _inputManager = inputManager;
            _outputManager = outputManager;

            _networkLock = new object();
        }
        
        public NetworkData ProcessNetworksRange(NetworkData inputData, int startIndex, int endIndex)
        {
            lock (_networkLock) {
                var data = _hiddenLayer.ProcessDataRange(inputData, startIndex, endIndex);

                data = _outputLayer.ProcessDataRange(data, startIndex, endIndex);

                return data;
            }
        }

        public NetworkData GetInputs()
        {
            lock (_networkLock) {
                return _inputManager.GetInputData();
            }
        }

        public void ProcessOutputs(NetworkData _outputData)
        {
            lock (_networkLock) {
                _outputManager.ProcessOutputs(_outputData);
            }
        }

        public int AddNetwork(IInput[] inputs, IOutput[] outputs, NodeData[] hiddenNodes1, NodeData[] hiddenNodes2)
        {
            lock (_networkLock) {
                if (_activeNetworks >= _maxNetworks) {
                    // NOTE: Perhaps the networks could be added to a queue? That way everything does get spawned and not just ignored
                    Debug.LogError("Number of networks is exceeding the maximum allowed active networks. \nMax (" + _maxNetworks + ")");
                    return -1;
                }

                _activeNetworks++;

                var inputID = _inputManager.AddNetwork(inputs);
                var hiddenLayer = _hiddenLayer.AddNetwork(hiddenNodes1);
                var outputLayer = _outputLayer.AddNetwork(hiddenNodes2);
                var outputID = _outputManager.AddNetwork(outputs);

                if (inputID != hiddenLayer || hiddenLayer != outputID) {
                    Debug.LogError("Network ID mismatches. This should not be happening for any reason so there is a problem with the logic.\n" +
                                               "InputID: " + inputID + ", HiddenID: " + hiddenLayer + ", OutputID: " + outputID);
                }

                return hiddenLayer;
            }
        }
        public int AddNetwork(NetworkContainer network)
        {
            return AddNetwork(network.Inputs, network.Outputs, network.HiddenLayer, network.HiddenOutputLayer);
        }
        public int AddNetwork(IInput[] inputs, IOutput[] outputs, int numHiddenNodes)
        {
            var hiddenLayer1 = _hiddenLayer.CreatureNodeLayer(numHiddenNodes, inputs.Length, true);
            var hiddenLayer2 = _outputLayer.CreatureNodeLayer(outputs.Length, numHiddenNodes, true);

            return AddNetwork(inputs, outputs, hiddenLayer1, hiddenLayer2);
        }

        public void RemoveNetwork(int networkID)
        {
            lock (_networkLock) {
                if (networkID >= _maxNetworks || networkID < 0) {
                    Debug.LogError("The network trying to be removed is outside the bounds of the network");
                    return;
                }

                _inputManager.RemoveNetwork(networkID);
                _hiddenLayer.RemoveNetwork(networkID);
                _outputLayer.RemoveNetwork(networkID);
                _outputManager.RemoveNetwork(networkID);

                _activeNetworks--;
            }
        }

        public NetworkContainer GetNetwork(int networkID)
        {
            return new NetworkContainer(_inputManager.GetNetwork(networkID), _hiddenLayer.GetNetwork(networkID), 
                                    _outputLayer.GetNetwork(networkID), _outputManager.GetNetwork(networkID));
        }
    }

}
