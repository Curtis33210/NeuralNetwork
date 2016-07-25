using UnityEngine;
using System.Collections.Generic;
using NeuralNetwork;

public static class NetworkSettings
{
    public static NeuralNetworkManager ActiveNetwork { get; set; }

    public static LinkManager LinkManager { get; set; }

    public static float NetworkUpdatesPerSecond { get; set; }

    public static float BaseActionCost { get; set; }

    public static float SensorWeight { get; set; }
    public static float ActionWeight { get; set; }
    public static float HiddenNodeWeight { get; set; }
    
    public static float SensorEnergy { get; set; }
    public static float ActionEnergy { get; set; }
    public static float HiddenNodeEnergy { get; set; }
}