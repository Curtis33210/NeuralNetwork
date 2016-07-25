using System;

namespace NeuralNetwork
{
    /// <summary>
    /// Exception thrown when any error is found inside the Nerual Network.
    /// </summary>
    public class NetworkException : Exception
    {
        public NetworkException() { }

        public NetworkException(string message) : base(message) { }

        public NetworkException(string message, Exception inner) : base(message, inner) { }
    }
}
