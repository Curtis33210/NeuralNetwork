using UnityEngine;
using NeuralNetwork;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {

    private World _activeWorld;
    public World ActiveWorld { get { return _activeWorld; } }

    [SerializeField]
    private int _worldWidth, _worldHeight, _maxCreatures;

    public int MaxCreatures { get { return _maxCreatures; } }

    private void Awake()
    {
        _activeWorld = new World(_worldWidth, _worldHeight, _maxCreatures);

        WorldSettings.ActiveWorld = _activeWorld;
        WorldSettings.MaxCreatures = _maxCreatures;

        WorldSettings.WorldWidth = _worldWidth;
        WorldSettings.WorldHeight = _worldHeight;
    }

    private void Update()
    {
        _activeWorld.UpdateWorld(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.R))
            RandomizeTiles();

        if (Input.GetKeyDown(KeyCode.K))
            ActiveWorld.RemoveAllActiveCreatures();

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (Time.timeScale == 1)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }

    private void RandomizeTiles()
    {
        for (int y = 0; y < _activeWorld.MapHeight; y++) {
            for (int x = 0; x < _activeWorld.MapWidth; x++) {
                _activeWorld.GetTileAt(x, y).ChangeTileType((TileType)Random.Range(0, 4));
            }
        }
    }
}