using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class Food : IFood
{
    public string FoodName { get; private set; }
    public int CurrentEnergy { get; private set; }
    public int StartEnergy { get; private set; }

    private Action<Food> _onEnergyLost;
    private Action<Food> _onOutOfEnergy;

    private Food() { } // Stops anyone creating a constructor for food. Have to go through the FoodPrototype

    public int GetEnergy(int consumeAmount)
    {
        var returnEnergy = 0;

        if (consumeAmount >= CurrentEnergy) {
            returnEnergy = CurrentEnergy;

            CurrentEnergy = 0;

            if (_onOutOfEnergy != null)
                _onOutOfEnergy(this);
        } else {
            returnEnergy = consumeAmount;

            CurrentEnergy -= consumeAmount;

            if (_onEnergyLost != null)
                _onEnergyLost(this);
        }
        
        return returnEnergy;
    }

    public static Food CreateNewInstance(Food originalFood)
    {
        var foodCopy = new Food();

        foodCopy.FoodName = originalFood.FoodName;
        foodCopy.StartEnergy = originalFood.StartEnergy;
        foodCopy.CurrentEnergy = originalFood.CurrentEnergy;
        foodCopy._onEnergyLost = originalFood._onEnergyLost;
        foodCopy._onOutOfEnergy = originalFood._onOutOfEnergy;

        return foodCopy;
    }

    public static Food CreateFoodPrototype(string foodName, int initialEnergy, Action<Food> _energyLostCallback, Action<Food> _outOfEnergyCallback)
    {
        var foodPrototype = new Food();

        foodPrototype.FoodName = foodName;
        foodPrototype.StartEnergy = initialEnergy;
        foodPrototype.CurrentEnergy = initialEnergy;
        foodPrototype._onEnergyLost = _energyLostCallback;
        foodPrototype._onOutOfEnergy = _outOfEnergyCallback;

        return foodPrototype;
    }

    public void RegisterOnFoodOutOfEnergyCallback(Action<Food> callback)
    {
        _onOutOfEnergy += callback;
    }
    public void RegisterOnEnergyLostCallback(Action<Food> callback)
    {
        _onEnergyLost += callback;
    }
}