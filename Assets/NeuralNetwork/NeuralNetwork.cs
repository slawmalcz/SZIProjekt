using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.NeuralNetwork {
    public class NeuralNetwork {
        private const double MUTATOR_PERCENTAGE = 0.1;
        private NetworkModel model;

        private NeuralLayer inputLayer;
        private NeuralLayer outputLayer;
        private List<NeuralLayer> hiddenLayers;

        public NeuralNetwork(int inputNodeCount, int outputNodeCount) {
            model = new NetworkModel(DendriteIdAssigner);
            inputLayer = new NeuralLayer(inputNodeCount, NeuronIdAssigner, "INPUT LAYER");
            outputLayer = new NeuralLayer(outputNodeCount, NeuronIdAssigner, "OUTPUT LAYER");
            hiddenLayers = new List<NeuralLayer>();
        }

        public void AddNextLayer(NeuralLayer nextLayer) => hiddenLayers.Add(nextLayer);

        public void Build() {
            model.AddLayer(inputLayer);
            hiddenLayers.ForEach(x => model.AddLayer(x));
            model.AddLayer(outputLayer);
            model.Build();
        }

        public List<double> AskNetwork(List<float> inputVector) => model.AskNetwork(inputVector);

        //STATIC MUTATROS AND CROSSBREEDERS

        public static NeuralNetwork CrossBread(NeuralNetwork father, NeuralNetwork mother) {
            if(father == mother) {
                var tempModel = father.Clone();
                for(var layerCounter = 0; layerCounter < tempModel.model.Layers.Count; layerCounter++) {
                    for(var neuronCounter = 0; neuronCounter < father.model.Layers[layerCounter].Neurons.Count; neuronCounter++) {
                        var handledNeuron = father.model.Layers[layerCounter].Neurons[neuronCounter];
                        tempModel.model.Layers[layerCounter].Neurons[neuronCounter].bias =
                            (handledNeuron.bias + mother.model.Layers[layerCounter].Neurons[neuronCounter].bias) / 2;
                        for(var dendriteCounter = 0; dendriteCounter < handledNeuron.Dendrites.Count; dendriteCounter++) {
                            var handledDendrite = handledNeuron.Dendrites[dendriteCounter];
                            tempModel.model.Layers[layerCounter].Neurons[neuronCounter].Dendrites[dendriteCounter].SynapticWeight =
                                (handledDendrite.SynapticWeight + mother.model.Layers[layerCounter].Neurons[neuronCounter].Dendrites[dendriteCounter].SynapticWeight) / 2;
                        }
                    }
                }
                return tempModel;
            } else {
                Debug.LogError("Given network are difrent problem class. [They are not Eq]");
                throw new Exception("Given network are difrent problem class. [They are not Eq]");
            }
        }

        public static NeuralNetwork Mutate(NeuralNetwork network, double mutationPercentage = MUTATOR_PERCENTAGE, float mutationForce = 0.1f) {
            var ret = network.Clone();
            var random = new System.Random();
            for(var layerCounter = 0; layerCounter < ret.model.Layers.Count; layerCounter++) {
                var handeldLayer = ret.model.Layers[layerCounter];
                for(var neuronCounter = 0; neuronCounter < handeldLayer.Neurons.Count; neuronCounter++) {
                    var handledNeuron = handeldLayer.Neurons[neuronCounter];
                    if(random.NextDouble() < MUTATOR_PERCENTAGE) {
                        var mutator = (random.NextDouble() * 2 - 1) * mutationForce;
                        ret.model.Layers[layerCounter].Neurons[neuronCounter].bias = handledNeuron.bias + handledNeuron.bias * mutator;
                    }
                    for(var dendriteCounter = 0; dendriteCounter < handledNeuron.Dendrites.Count; dendriteCounter++) {
                        if(random.NextDouble() < MUTATOR_PERCENTAGE) {
                            var handledDendrite = handledNeuron.Dendrites[dendriteCounter];
                            var mutator = (random.NextDouble() * 2 - 1) * mutationForce;
                            ret.model.Layers[layerCounter].Neurons[neuronCounter].Dendrites[dendriteCounter].SynapticWeight = handledDendrite.SynapticWeight + handledDendrite.SynapticWeight * mutator;
                        }
                    }
                }
            }
            return ret;
        }

        //ID ASSIGNERS

        private int nextNeuronId = 0;
        public int NeuronIdAssigner() => nextNeuronId++ - 1;
        private int nextDendriteId = 0;
        public int DendriteIdAssigner() => nextDendriteId++ - 1;

        // SERIALIZATION AND DESERIALIZATION

        public void Serialize(string fileName) {
            var serializer = new XmlSerializer(typeof(NetworkModel));
            TextWriter writer = new StreamWriter(fileName);
            serializer.Serialize(writer, model);
            writer.Close();
        }

        public static NeuralNetwork Deserialize(string fileName) {
            var serializer = new XmlSerializer(typeof(NetworkModel));
            serializer.UnknownNode += new
            XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new
            XmlAttributeEventHandler(Serializer_UnknownAttribute);
            var fs = new FileStream(fileName, FileMode.Open);
            var model = (NetworkModel)serializer.Deserialize(fs);
            var ret = new NeuralNetwork(model.Layers.First().Neurons.Count, model.Layers.Last().Neurons.Count);
            foreach(var layer in model.Layers) {
                if(model.Layers.First() == layer || model.Layers.Last() == layer) continue;
                ret.AddNextLayer(new NeuralLayer(layer.Neurons.Count, ret.NeuronIdAssigner, layer.Name));
            }
            ret.Build();
            //assign new weights
            for(var i = 0; i < model.Layers.Count; i++) {
                for(var j = 0; j < model.Layers[i].Neurons.Count; j++) {
                    ret.model.Layers[i].Neurons[j].bias = model.Layers[i].Neurons[j].bias;
                    for(var k = 0; k < model.Layers[i].Neurons[j].Dendrites.Count; k++)
                        ret.model.Layers[i].Neurons[j].Dendrites[k].SynapticWeight = model.Layers[i].Neurons[j].Dendrites[k].SynapticWeight;
                }
            }
            return ret;
        }

        protected static void Serializer_UnknownNode(object sender, XmlNodeEventArgs e) => Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);

        protected static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e) => Console.WriteLine("Unknown attribute " + e.Attr.Name + "='" + e.Attr.Value + "'");

        // Operator Overrides

        public NeuralNetwork Clone() {
            var model = this.model;
            var ret = new NeuralNetwork(model.Layers.First().Neurons.Count, model.Layers.Last().Neurons.Count);
            foreach(var layer in model.Layers) {
                if(model.Layers.First() == layer || model.Layers.Last() == layer) continue;
                ret.AddNextLayer(new NeuralLayer(layer.Neurons.Count, ret.NeuronIdAssigner, layer.Name));
            }
            ret.Build();
            //assign new weights
            for(var i = 0; i < model.Layers.Count; i++) {
                for(var j = 0; j < model.Layers[i].Neurons.Count; j++) {
                    ret.model.Layers[i].Neurons[j].bias = model.Layers[i].Neurons[j].bias;
                    for(var k = 0; k < model.Layers[i].Neurons[j].Dendrites.Count; k++)
                        ret.model.Layers[i].Neurons[j].Dendrites[k].SynapticWeight = model.Layers[i].Neurons[j].Dendrites[k].SynapticWeight;
                }
            }
            return ret;
        }

        public static bool operator ==(NeuralNetwork x, NeuralNetwork y) {
            return x.Equals(y);
        }
        public static bool operator !=(NeuralNetwork x, NeuralNetwork y) {
            return !x.Equals(y);
        }

        public override bool Equals(object value) {
            var other = value as NeuralNetwork;
            //TODO:: That's a lie
            //return true;
            return !System.Object.ReferenceEquals(null, other)
            && model.Layers.Count == other.model.Layers.Count //Layers count equality
            && model.Layers.Sum(x => x.Neurons.Count) == other.model.Layers.Sum(x => x.Neurons.Count);//Neuron count equality
            //&& model.Layers.Sum(x => x.Neurons.Sum(y => y.Dendrites.Count)) == other.model.Layers.Sum(x => x.Neurons.Sum(y => y.Dendrites.Count)) // Dendrites count equality
            //&& model.Layers.Any(x => other.model.Layers.Any(y => y.Neurons.Count == x.Neurons.Count && y.Name == x.Name)); // Neurons in Layers Count
        }
    }
}
