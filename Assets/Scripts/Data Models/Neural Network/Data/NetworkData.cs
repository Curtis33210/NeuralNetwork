using UnityEngine;
using System;

namespace NeuralNetwork
{
    using Random = UnityEngine.Random;

    public class NetworkData
    {
        private float[][] _abstractData;
        
        public int MaxNetworks { get { return _abstractData.Length; } }
        
        private const float MinFillValue = 0;
        private const float MaxFillValue = 0;

        /// <summary>
        /// Initializes all the values in the data array to the default using the specified sizes
        /// </summary>
        /// <param name="maxNetworks">Maximum number of networks that can be active at one time</param>
        public NetworkData(int maxNetworks)
        {
            _abstractData = new float[maxNetworks][];
        }

        /// <summary>
        /// Makes the values of the array equal to the supplied array's
        /// </summary>
        /// <param name="dataPreset">Array of Arrays to set this to</param>
        public NetworkData(float[][] dataPreset)
        {
            _abstractData = dataPreset;
        }
        
        public void AddNetwork(int networkID, int size)
        {
            _abstractData[networkID] = new float[size];
        }

        public void ReplaceNetwork(int networkA, int networkB)
        {
            _abstractData[networkA] = _abstractData[networkB];
            _abstractData[networkB] = null;
        }

        /// <summary>
        /// Test method to fill the _abstractData with random data.
        /// TODO: Add proper reference
        /// </summary>
        public void FillRandom()
        {
            var max = MaxNetworks;

            for (int i = 0; i < max; i++) {
                FillArray(_abstractData[i], MinFillValue, MaxFillValue);
            }
        }

        public float[] this[int key] {
            get { return _abstractData[key]; }
            set { _abstractData[key] = value; }
        }

        public float this[int i, int j] {
            get { return _abstractData[i][j]; }
            set { _abstractData[i][j] = value; }
        }

        /// <summary>
        /// Fills the supplied array with random values between two values (inclusive). 
        /// Note: This overrides any exisiting values
        /// </summary>
        /// <param name="floatArray">Array to be filled</param>
        /// <param name="min">Lowest value possible in the array</param>
        /// <param name="max">Highest value possible in the array</param>
        private void FillArray(float[] floatArray, float min, float max)
        {
            int arrayLength = floatArray.Length;

            for (int i = 0; i < arrayLength; i++) {
                floatArray[i] = Random.Range(min, max);
            }
        }

        /// <summary>
        /// Creates a float array of random values between 0 and 1
        /// </summary>
        /// <param name="size">Size of the array to generate</param>
        /// <returns>The random float array generated</returns>
        private float[] GenerateFloatArray(int size)
        {
            var floatArray = new float[size];

            for (int i = 0; i < size; i++) {
                floatArray[i] = Random.Range(0.0f, 1.0f);
            }

            return floatArray;
        }

        /// <summary>
        /// Creates a float array of random values between the specifed values
        /// </summary>
        /// <param name="size">Size of the array to generate</param>
        /// <param name="min">Minimum value possible in the array</param>
        /// <param name="max">Maximum value possible in the array</param>
        /// <returns>The random float array generated</returns>
        private float[] GenerateFloatArray(int size, float min, float max)
        {
            var floatArray = new float[size];

            for (int i = 0; i < size; i++) {
                floatArray[i] = Random.Range(min, max);
            }

            return floatArray;
        }
    }
}