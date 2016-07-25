using UnityEngine;
using System.Collections.Generic;

public static class WorldSettings
{
    public static World ActiveWorld { get; set; }

    public static LinkManager LinkManager { get; set; }

    public static int MaxCreatures { get; set; }

    public static int WorldWidth { get; set; }
    public static int WorldHeight { get; set; }
}