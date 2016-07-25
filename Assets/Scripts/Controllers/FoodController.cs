using UnityEngine;
using System.Collections.Generic;

public class FoodController : MonoBehaviour
{
    [SerializeField]
    private Transform _foodContainer;
    [SerializeField]
    private Sprite _testFoodSprite;

    private Dictionary<Food, GameObject> _activeFoods;

    private void Start()
    {
        var activeWorld = WorldSettings.ActiveWorld;

        _activeFoods = new Dictionary<Food, GameObject>();

        for (int y = 0; y < activeWorld.MapHeight; y++) {
            for (int x = 0; x < activeWorld.MapWidth; x++) {
                var tile = activeWorld.GetTileAt(x, y);

                tile.RegisterOnFoodAddedCallback(OnFoodAdded);

                if (tile.GetFood() != null)
                    OnFoodAdded(tile);
            }
        }
    }

    private void OnFoodAdded(Tile tile)
    {
        var food = tile.GetFood();
        food.RegisterOnFoodOutOfEnergyCallback(OnFoodOutOfEnergy);
        food.RegisterOnEnergyLostCallback(OnLostEnergy);

        var gameObject = new GameObject("Food: " + food.FoodName);

        gameObject.transform.SetParent(_foodContainer);
        gameObject.transform.position = new Vector3(tile.PosX, tile.PosY);

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _testFoodSprite;
        spriteRenderer.sortingLayerName = "Food";
        spriteRenderer.color = Color.green;

        _activeFoods.Add(food, gameObject);
    }

    private void OnLostEnergy(Food food)
    {
        var percentage = food.CurrentEnergy / (float)food.StartEnergy;
        
        var oldColor = _activeFoods[food].GetComponent<SpriteRenderer>().color;

        var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, percentage);

        _activeFoods[food].GetComponent<SpriteRenderer>().color = newColor;
    }

    private void OnFoodOutOfEnergy(Food food)
    {
        Destroy(_activeFoods[food]);

        _activeFoods.Remove(food);
    }
}