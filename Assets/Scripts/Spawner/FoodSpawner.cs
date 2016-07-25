using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField]
    private bool _spawnFood;
    [SerializeField]
    private int _spawnPerSecond;

    private FoodFactory _foodFactory;

    private void Start()
    {
        _foodFactory = new FoodFactory(WorldSettings.ActiveWorld);

        CreateFoodPrototypes(_foodFactory);

        StartCoroutine("SpawnFood");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            _spawnFood = !_spawnFood;
    }

    private IEnumerator SpawnFood()
    {
        while (true) {
            if (_spawnFood) {
                for (int i = 0; i < _spawnPerSecond; i++) {
                    AddFoodToWorld();
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void AddFoodToWorld()
    {
        var food = _foodFactory.SpawnNewRandomFood();
        
        int xPos = Random.Range(0, WorldSettings.WorldWidth);
        int yPos = Random.Range(0, WorldSettings.WorldHeight);
        
        if (WorldSettings.ActiveWorld.GetTileAt(xPos, yPos).GetFood() == null)
            WorldSettings.ActiveWorld.GetTileAt(xPos, yPos).AddFood(food);
    }

    private void CreateFoodPrototypes(FoodFactory factory)
    {
        factory.AddNewFoodPrototype(TestFood());
    }

    private Food TestFood()
    {
        return Food.CreateFoodPrototype("TestFood", 7500, null, null);
    }
}