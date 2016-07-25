using UnityEngine;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

namespace NeuralNetwork
{
    public class NodeData
    {
        public float Bias { get; private set; }
        private float[] _weights;

        private const float DefaultMinBias = -2.0f;
        private const float DefaultMaxBias = 2.0f;

        private const float DefaultMinWeight = -1.0f;
        private const float DefaultMaxWeight = 1.0f;

        /// <summary>
        /// Creates the node, initializing internal values.
        /// </summary>
        /// <param name="numOfWeights">Number of weights this node requires</param>
        /// <param name="fillRandom">Whether the Bias and Weights should be filled with random values</param>
        public NodeData(int numOfWeights)
        {
            Bias = 0;
            _weights = new float[numOfWeights];
        }

        /// <summary>
        /// Creates the node, initializing internal values.
        /// </summary>
        /// <param name="numOfWeights">Number of weights this node requires</param>
        /// <param name="randomizeBias">Whether the bias should be initialized to a random value</param>
        /// <param name="randomizeWeights">Whether the weights should be initialized to a random values</param>
        public NodeData(int numOfWeights, bool randomizeBias, bool randomizeWeights)
        {
            Bias = 0;
            _weights = new float[numOfWeights];

            if (randomizeBias)
                RandomizeBias(DefaultMinBias, DefaultMaxBias);

            if (randomizeWeights)
                RandomizeWeights(DefaultMinWeight, DefaultMaxWeight);
        }

        /// <summary>
        /// Deletes the weight specified for this node.
        /// </summary>
        /// <param name="weightID">The weight ID corrisponds to the input ID</param>
        public void DeleteWeight(int weightID)
        {
            _weights[weightID] = _weights[_weights.Length - 1]; // Moves the last item to the deleted index

            Array.Resize(ref _weights, _weights.Length - 1); // Trims the last item off
        }

        /// <summary>
        /// Sets the Bias to a random value between the min and max (Inclusive)
        /// </summary>
        /// <param name="min">Minimum value the bias can be</param>
        /// <param name="max">Maximum value the bias can be</param>
        public void RandomizeBias(float min, float max)
        {
            Bias = Random.Range(min, max);
        }

        /// <summary>
        /// Sets the weights to random values between the min and max (Inclusive)
        /// </summary>
        /// <param name="min">Minimum value the bias can be</param>
        /// <param name="max">Maximum value the bias can be</param>
        public void RandomizeWeights(float min, float max)
        {
            for (int i = 0; i < _weights.Length; i++) {
                _weights[i] = Random.Range(min, max);
            }
        }

        public float this[int key] {
            get { return _weights[key]; }
            set { _weights[key] = value; }
        }
    }

}