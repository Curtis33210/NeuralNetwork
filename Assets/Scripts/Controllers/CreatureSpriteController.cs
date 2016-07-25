using UnityEngine;
using System.Collections.Generic;

public class CreatureSpriteController : MonoBehaviour
{
    [SerializeField]
    private Sprite _creatureSprite;
    [SerializeField]
    private Transform _creatureContainer;

    private GameObject[] _creatureArray;
    
    private void Start()
    {
        _creatureArray = new GameObject[WorldSettings.MaxCreatures];

        WorldSettings.ActiveWorld.RegisterCreatureAddedToWorldCallback(OnCreatureAdded);
        WorldSettings.ActiveWorld.RegisterCreatureRemovedFromWorldCallback(OnCreatureRemoved);

        var activeCreatures = WorldSettings.ActiveWorld.GetAllActiveCreatures();
        
        for (int i = 0; i < activeCreatures.Length; i++) {
            OnCreatureAdded(activeCreatures[i]);
        }
    }

    private void OnCreatureAdded(Creature newCreature)
    {
        var gameObject = new GameObject("Creature (" + newCreature.ID + ")");

        gameObject.transform.SetParent(_creatureContainer);
        gameObject.transform.position = new Vector3(newCreature.PosX, newCreature.PosY);
        gameObject.transform.localScale = new Vector3(0.8f, 1, 1);

        var spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _creatureSprite;
        spriteRenderer.sortingLayerName = "Creature";
        spriteRenderer.color = newCreature.Color;

        newCreature.RegisterDirectionChangedCallback(OnCreatureRotated);
        newCreature.RegisterPositionChangedCallback(OnCreatureMoved);

        _creatureArray[newCreature.ID] = gameObject;
    }
    private void OnCreatureRemoved(Creature newCreature)
    {
        Destroy(_creatureArray[newCreature.ID]);

        _creatureArray[newCreature.ID] = null;
    }

    private void OnCreatureRotated(Creature newCreature)
    {
        var creatureTransform = _creatureArray[newCreature.ID].transform;

        switch (newCreature.FacingDirection) {
            case Direction.North:
                creatureTransform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.East:
                creatureTransform.rotation = Quaternion.Euler(0, 0, 270);
                break;
            case Direction.South:
                creatureTransform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Direction.West:
                creatureTransform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }
    private void OnCreatureMoved(Creature creature)
    {
        var creatureGameObject = _creatureArray[creature.ID];

        if (creatureGameObject == null)
            return;

        creatureGameObject.transform.position = new Vector3(creature.PosX, creature.PosY);
    }
}