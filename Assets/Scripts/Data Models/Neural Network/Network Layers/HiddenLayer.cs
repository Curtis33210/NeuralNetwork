using UnityEngine;
using System.Threading;
using System;

namespace NeuralNetwork
{
    public class HiddenLayer
    {
        protected NodeData[][] _nodeArray;
        protected NetworkData _outputData;

        private int _numActiveThreads = 0;
        private ManualResetEvent _resetEvent;

        private int _activeNetworks = 0;

        public HiddenLayer(int maxNetworks)
        {
            _nodeArray = new NodeData[maxNetworks][];

            _outputData = new NetworkData(maxNetworks);

            _resetEvent = new ManualResetEvent(false);
        }
        
        public int AddNetwork(NodeData[] nodeData)
        {
            int networkID = _activeNetworks++;

            _outputData.AddNetwork(networkID, nodeData.Length);

            _nodeArray[networkID] = nodeData;

            return networkID;
        }
        public int AddNetwork(int numNodes, int numInputs, bool randomizeValues = false)
        {
            var nodeData = CreatureNodeLayer(numNodes, numInputs, randomizeValues);

            return AddNetwork(nodeData);
        }
        public void RemoveNetwork(int networkID)
        {
            if (networkID > _nodeArray.Length || _nodeArray[networkID] == null) {
                Debug.LogError("Network ID is out of bounds or not active and so cannot be removed (" + networkID + ")");
                return;
            }

            _activeNetworks--;

            if (networkID != _activeNetworks) { // Swap the network to be removed with the last network
                _nodeArray[networkID] = _nodeArray[_activeNetworks];
                _outputData.ReplaceNetwork(networkID, _activeNetworks);
            }

            _nodeArray[_activeNetworks] = null;
        }
        
        public NodeData[] GetNetwork(int networkID)
        {
            return _nodeArray[networkID];
        }

        public NodeData[] CreatureNodeLayer(int numOfNodes, int numOfWeights, bool randomizeLayer = false)
        {
            var nodeData = new NodeData[numOfNodes];

            for (int i = 0; i < numOfNodes; i++) {
                nodeData[i] = new NodeData(numOfWeights, randomizeLayer, randomizeLayer);
            }

            return nodeData;
        }

        public NetworkData ProcessData(NetworkData inputData)
        {
            return InternalProcessDataMultiThread(inputData, 0, _activeNetworks);
        }
        public NetworkData ProcessDataRange(NetworkData inputData, int start, int end)
        {
            return InternalProcessDataMultiThread(inputData, start, end);
        }

        private NetworkData InternalProcessDataMultiThread(NetworkData inputData, int startIndex, int endIndex)
        {
            if (_activeNetworks == 0)
                return _outputData;

            _resetEvent.Reset();

            int incrementVal = (int)Mathf.Ceil(_activeNetworks / 8.0f);

            for (int i = startIndex; i < endIndex; i = i + incrementVal) {
                if (i >= _activeNetworks)
                    break;

                Interlocked.Increment(ref _numActiveThreads);

                var threadStartIndex = i;
                var threadEndIndex = (threadStartIndex + incrementVal < _activeNetworks) ? threadStartIndex + incrementVal : _activeNetworks;

                ThreadPool.QueueUserWorkItem((arg) => {
                    InternalProcessDataRange(inputData, threadStartIndex, threadStartIndex + incrementVal);

                    if (Interlocked.Decrement(ref _numActiveThreads) == 0)
                        _resetEvent.Set();
                });
            }

            _resetEvent.WaitOne(50);

            return _outputData;
        }
        private NetworkData InternalProcessDataSingleThread(NetworkData inputData)
        {
            InternalProcessDataRange(inputData, 0, _activeNetworks);

            return _outputData;
        }

        private void InternalProcessDataRange(NetworkData inputData, int startIndex, int endIndex)
        {
            for (int networkID = startIndex; networkID < endIndex; networkID++) {
                if (networkID >= _activeNetworks)
                    break;

                int numOfHiddenNodes = _nodeArray[networkID].Length;

                for (int hiddenNodeID = 0; hiddenNodeID < numOfHiddenNodes; hiddenNodeID++) {
                    var numInputs = inputData[networkID].Length;
                    var nodeSum = _nodeArray[networkID][hiddenNodeID].Bias;

                    for (int inputNodeID = 0; inputNodeID < numInputs; inputNodeID++) {
                        nodeSum = inputData[networkID, inputNodeID] * this[networkID, hiddenNodeID, inputNodeID];
                    }

                    _outputData[networkID, hiddenNodeID] = (nodeSum / (1 + Mathf.Abs(nodeSum))) + 0.5f; // Fast Sigmoid function. **Estimate**
                }
            }
        }

        private NodeData[] this[int key] {
            get { return _nodeArray[key]; }
            set { _nodeArray[key] = value; }
        }
        private NodeData this[int i, int j] {
            get { return _nodeArray[i][j]; }
            set { _nodeArray[i][j] = value; }
        }
        private float this[int i, int j, int k] {
            get { return _nodeArray[i][j][k]; }
            set { _nodeArray[i][j][k] = value; }
        }
    }	
}