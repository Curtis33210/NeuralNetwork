using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class TileSpriteController : MonoBehaviour
{
    private World ActiveWorld { get { return WorldSettings.ActiveWorld; } }

    [SerializeField]
    private Transform _tileContainer;

    [SerializeField]
    private Sprite _tileSpriteNotFound;

    private const string tilesPath = @"Tiles/";

    private Sprite[] _spriteArray;

    private GameObject[,] _tileObjects;

    private void Start()
    {
        _spriteArray = new Sprite[(int)TileType.Count];

        LoadSprites();

        _tileObjects = new GameObject[ActiveWorld.MapWidth, ActiveWorld.MapHeight];

        InitializeAlreadyActiveSprites();
    }

    private void InitializeAlreadyActiveSprites()
    {
        var spriteRenderer = new Type[] { typeof(SpriteRenderer) };

        for (int y = 0; y < ActiveWorld.MapHeight; y++) {
            for (int x = 0; x < ActiveWorld.MapWidth; x++) {
                Tile curTile = ActiveWorld.GetTileAt(x, y);

                var gameObject = new GameObject("Tile (" + x + ", " + y + ")", spriteRenderer);
                gameObject.transform.SetParent(_tileContainer);
                gameObject.transform.position = new Vector3(x, y);
                gameObject.isStatic = true;

                gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "World Tile";

                curTile.RegisterTileTypeChangedCallback(UpdateTileSprite);

                _tileObjects[x, y] = gameObject;

                UpdateTileSprite(curTile);
            }
        }
    }

    private void UpdateTileSprite(Tile changedTile)
    {
        if (changedTile.Type == TileType.Count)
            throw new UnauthorizedAccessException("Tile is trying to be changed to the type of 'Count' which is not actually a tile");

        var gameObject = _tileObjects[changedTile.PosX, changedTile.PosY];

        Sprite newSprite;
        
        if (changedTile.Type == TileType.None) 
            newSprite = null;
        else 
            newSprite = _spriteArray[(int)changedTile.Type];
        
        gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    private void LoadSprites()
    {
        var tileTypes = Enum.GetValues(typeof(TileType)).Cast<TileType>();

        foreach (var tileType in tileTypes) {
            if (tileType == TileType.None || tileType == TileType.Count)
                continue;

            var sprite = Resources.Load<Sprite>(tilesPath + tileType.ToString());

            if (sprite == null) {
                sprite = _tileSpriteNotFound;
                Debug.LogError("A sprite for Tile Type: " + tileType + " was not found");
            }

            _spriteArray[(int)tileType] = sprite;
        }
    }
}