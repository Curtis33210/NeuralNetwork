using UnityEngine;
using System.Collections.Generic;

public class FoodFactory
{
    private World _activeWorld;

    private List<Food> _foodPrototypes;

    public FoodFactory(World world)
    {
        _activeWorld = world;

        _foodPrototypes = new List<Food>();
    }

    public void AddNewFoodPrototype(Food newFood)
    {
        _foodPrototypes.Add(newFood);
    }

    public Food SpawnNewRandomFood()
    {
        if (_foodPrototypes == null || _foodPrototypes.Count == 0) {
            Debug.LogError("Food Prototypes is null or empty. Add food types before spawning any");
            return null;
        }

        var numFoodTypes = _foodPrototypes.Count - 1;
        var spawnFoodType = Random.Range(0, numFoodTypes);

        var newFood = Food.CreateNewInstance(_foodPrototypes[spawnFoodType]);
        
        return newFood;
    }
}