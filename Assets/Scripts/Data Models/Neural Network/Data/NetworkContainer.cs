using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

namespace NeuralNetwork
{
    public class NetworkContainer
    {
        public IInput[] Inputs { get; private set; }
        public NodeData[] HiddenLayer { get; private set; }
        public NodeData[] HiddenOutputLayer { get; private set; }
        public IOutput[] Outputs { get; private set; }

        public NetworkContainer(IInput[] inputs, NodeData[] hiddenLayer, NodeData[] hiddenOutputLayer, IOutput[] outputs)
        {
            Inputs = inputs;
            HiddenLayer = hiddenLayer;
            HiddenOutputLayer = hiddenOutputLayer;
            Outputs = outputs;
        }

        public NetworkContainer CloneNetwork(float errorChance)
        {
            IInput[] newInputs = new IInput[Inputs.Length];
            IOutput[] newOutputs = new IOutput[Outputs.Length];
            NodeData[] newHiddenLayer = new NodeData[HiddenLayer.Length];
            NodeData[] newHiddenOutputLayer = new NodeData[HiddenOutputLayer.Length];

            for (int i = 0; i < Inputs.Length; i++) {
                newInputs[i] = ((CreatureSensor)Inputs[i]).ImperfectCopy(null, errorChance, -0.2f, 0.2f);
            }
            for (int i = 0; i < Outputs.Length; i++) {
                newOutputs[i] = ((CreatureAction)Outputs[i]).ImperfectCopy(null, errorChance, -0.2f, 0.2f);
            }
            for (int i = 0; i < HiddenLayer.Length; i++) {
                newHiddenLayer[i] = HiddenLayer[i];
            }
            for (int i = 0; i < HiddenOutputLayer.Length; i++) {
                newHiddenOutputLayer[i] = HiddenOutputLayer[i];
            }

            var newNetwork = new NetworkContainer(Inputs, HiddenLayer, HiddenOutputLayer, Outputs);

            //newNetwork.ModifyInputs();
            //newNetwork.ModifyOutputs();
            //newNetwork.ModifyHiddenLayers();

            return newNetwork;
        }

        public void UpdateTargetCreature(Creature newCreature)
        {
            for (int i = 0; i < Inputs.Length; i++) {
                ((CreatureSensor)Inputs[i]).UpdateCreature(newCreature);
            }

            for (int i = 0; i < Outputs.Length; i++) {
                ((CreatureAction)Outputs[i]).UpdateCreature(newCreature);
            }
        }

        private void ModifyInputs()
        {
            var deleteIndexes = DeleteAmount(0.05f, Inputs.Length);
            var numNewInputs = AddAmount(0.05f);

            if (deleteIndexes != null) {
                for (int i = 0; i < deleteIndexes.Length; i++) {
                    Inputs[deleteIndexes[i]] = Inputs[Inputs.Length - 1 - i];

                    Inputs[Inputs.Length - 1 - i] = null;

                    for (int j = 0; j < HiddenLayer.Length; j++) {
                        HiddenLayer[j].DeleteWeight(deleteIndexes[i]);
                    }
                }
            }

            for (int i = 0; i < numNewInputs; i++) {

            }
            
        }

        private void ModifyOutputs()
        {

        }

        private void ModifyHiddenLayers()
        {

        }

        private int AddAmount(float addChance)
        {
            int addAmount = 0;

            while (Random.Range(0, 1.0f) > 1 - addChance) {
                addAmount++;
            }

            return addAmount;
        }
        private int[] DeleteAmount(float deleteChance, int length)
        {
            int deleteAmount = AddAmount(deleteChance);

            if (deleteAmount == 0)
                return null;

            var deleteIndexes = new int[deleteAmount];

            for (int i = 0; i < deleteAmount; i++) {
                int deleteIndex;

                do {
                    deleteIndex = Random.Range(0, length);
                } while (deleteIndexes.Contains(deleteIndex));

                deleteIndexes[i] = deleteIndex;
            }

            return deleteIndexes;
        }
    }
}