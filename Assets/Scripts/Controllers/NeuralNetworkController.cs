using UnityEngine;
using System.Linq;
using NeuralNetwork;
using System.Collections;
using System.Diagnostics;

using Debug = UnityEngine.Debug;
using System.Threading;

public class NeuralNetworkController : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 60)]
    private float _updatesPerSecond;

    [SerializeField]
    private float _baseActionCost;

    [SerializeField]
    private float _sensorWeight;
    [SerializeField]
    private float _actionWeight;
    [SerializeField]
    private float _hiddenNodeWeight;


    [SerializeField]
    private float _sensorEnergy;
    [SerializeField]
    private float _actionEnergy;
    [SerializeField]
    private float _hiddenNodeEnergy;

    private NeuralNetworkManager _networkManager;

    public NeuralNetworkManager ActiveNetwork { get { return _networkManager; } }
    
    private volatile bool _finishedProcessing = false;
    
    private void Start()
    {
        int maxNetworks = WorldSettings.MaxCreatures;

        var inputManager = new SensorManager(maxNetworks);
        var outputManager = new ActionManager(maxNetworks);

        _networkManager = new NeuralNetworkManager(maxNetworks, inputManager, outputManager);

        SetStaticVariables();

        StartCoroutine("ProcessNetworksCoroutine");
    }

    private IEnumerator ProcessNetworksCoroutine()
    {
        NetworkData inputData = null;
        NetworkData outputData = null;

        var timer = Stopwatch.StartNew();

        while (true) { 
            if (_updatesPerSecond <= 0)
                throw new NetworkException("Updates per second cannot be less than 0");

            var updateWait = (1.0f / _updatesPerSecond);

            Profiler.BeginSample("Input");
            inputData = _networkManager.GetInputs();
            Profiler.EndSample();

            ThreadPool.QueueUserWorkItem((arg) => {
                _finishedProcessing = false;

                outputData = _networkManager.ProcessNetworksRange(inputData, 0, _networkManager.ActiveNetworks);

                _finishedProcessing = true;
            });

            timer.Reset();
            timer.Start();

            StartCoroutine("WaitForFinishedProcessing");

            Profiler.BeginSample("Output");
            _networkManager.ProcessOutputs(outputData);
            Profiler.EndSample();

            timer.Stop();

            var processTime = timer.ElapsedMilliseconds / 1000.0f;

            yield return new WaitForSeconds(updateWait - processTime);
        }
    }

    private IEnumerator WaitForFinishedProcessing()
    {
        do {
            yield return null;
        } while (_finishedProcessing == false);
    }

    private void OnValidate()
    {
        SetStaticVariables();
    }

    private void SetStaticVariables()
    {
        NetworkSettings.ActiveNetwork = _networkManager;
        NetworkSettings.NetworkUpdatesPerSecond = _updatesPerSecond;
        NetworkSettings.BaseActionCost = _baseActionCost;

        NetworkSettings.SensorWeight = _sensorWeight;
        NetworkSettings.ActionWeight = _actionWeight;
        NetworkSettings.HiddenNodeWeight = _hiddenNodeWeight;

        NetworkSettings.SensorEnergy = _sensorEnergy;
        NetworkSettings.ActionEnergy = _actionEnergy;
        NetworkSettings.HiddenNodeEnergy = _hiddenNodeEnergy;
    }
}