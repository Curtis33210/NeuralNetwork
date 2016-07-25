using UnityEngine;
using NeuralNetwork;
using System.Collections.Generic;
using System.Collections;

public class CreatureSpawner : MonoBehaviour
{
    [SerializeField]
    private int _spawnNumber;
    [SerializeField]
    private int _maxSensors;
    [SerializeField]
    private int _maxActions;

    private bool _spawnCreatures;

    private World _activeWorld;
    private NeuralNetworkManager _networkManager;

    private LinkManager _linkManager;

    private CreatureAction[] _creatureActions;
    private CreatureSensor[] _creatureSensors;

    private void Start()
    {
        _activeWorld = WorldSettings.ActiveWorld;
        _networkManager = NetworkSettings.ActiveNetwork;

        AddCreatureActions();
        AddCreatureSensors();

        _linkManager = new LinkManager(WorldSettings.MaxCreatures);
        StartCoroutine("SpawnCreaturesInterval");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) 
            _spawnCreatures = !_spawnCreatures;
        
        if (Input.GetKeyDown(KeyCode.T)) 
            SpawnTestCreature();
    }
    
    private void SpawnTestCreature()
    {
        var creature = _activeWorld.CreateNewCreature(10, 30000, Color.black);
        
        creature.MoveTo(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        _activeWorld.AddCreatureToWorld(creature);

        var sensors = new CreatureSensor[] {
            new RandomizeSensor(creature)
        };
        var actions = new CreatureAction[] {
            new Mouth(creature)
        };

        int networkID = _networkManager.AddNetwork(sensors, actions, 50);

        _linkManager.AddLink(creature, networkID);
    }

    private Creature CreateCreature(int numSensors, int numActions)
    {
        var creature = _activeWorld.CreateNewCreature(10, 30000);

        int xPos = Random.Range(0, WorldSettings.WorldWidth);
        int yPos = Random.Range(0, WorldSettings.WorldHeight);

        do {
            xPos = Random.Range(0, WorldSettings.WorldWidth);
            yPos = Random.Range(0, WorldSettings.WorldHeight);
        } while (_activeWorld.GetTileAt(xPos, yPos).ContainsCreature());

        creature.MoveTo(xPos, yPos);

        _activeWorld.AddCreatureToWorld(creature);

        var sensors = new CreatureSensor[numSensors];
        var actions = new CreatureAction[numActions];

        actions[0] = new DNACloner(creature);

        for (int i = 0; i < numSensors; i++) {
            var random = Random.Range(0, _creatureSensors.Length);

            sensors[i] = _creatureSensors[random].PerfectCopy(creature);
            sensors[i].RandomizeVariables();
        }

        for (int i = 1; i < numActions; i++) {
            var random = Random.Range(0, _creatureActions.Length);

            actions[i] = _creatureActions[random].PerfectCopy(creature);
            actions[i].RandomizeVariables();
        }

        int networkID = _networkManager.AddNetwork(sensors, actions, 50);

        _linkManager.AddLink(creature, networkID);

        return creature;
    }

    private Creature CreateRandomCreature()
    {
        var numSensors = Random.Range(1, _maxSensors);
        var numActions = Random.Range(1, _maxActions);

        return CreateCreature(numSensors, numActions + 1);
    }

    private IEnumerator SpawnCreaturesInterval()
    {
        while (true) {
            if (_spawnCreatures) {
                for (int i = 0; i < _spawnNumber; i++) {
                    CreateRandomCreature();
                }
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void AddCreatureActions()
    {
        _creatureActions = new CreatureAction[] {
            new BasicMovement(null),
            new BasicRotator(null),
            new Mouth(null)
        };

        for (int i = 0; i < _creatureActions.Length; i++) {
            _creatureActions[i].RandomizeVariables();
        }
    }

    private void AddCreatureSensors()
    {
        _creatureSensors = new CreatureSensor[] {
            new RandomizeSensor(null)
        };

        for (int i = 0; i < _creatureSensors.Length; i++) {
            _creatureSensors[i].RandomizeVariables();
        }
    }
}