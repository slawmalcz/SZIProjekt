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
            model = new NetworkModel() {
                dendriteIdAssigner = DendriteIdAssigner
            };
            inputLayer = new NeuralLayer(inputNodeCount, NeuronIdAssigner, "INPUT LAYER");
            outputLayer = new NeuralLayer(outputNodeCount, NeuronIdAssigner, "OUTPUT LAYER");
            hiddenLayers = new List<NeuralLayer>();
        }

        public void AddNextLayer(NeuralLayer nextLayer) => hiddenLayers.Add(nextLayer);

        public void Build(bool debugMode = false, bool fullInspect = false) {
            model.AddLayer(inputLayer);
            hiddenLayers.ForEach(x => model.AddLayer(x));
            model.AddLayer(outputLayer);
            model.Build();
            if(debugMode) {
                if(fullInspect) {

                } else {
                    Debug.Log("Previev of builded network:");
                    Debug.Log(model.Print());
                }
            }
        }

        public List<double> AskNetwork(List<float> inputVector) => model.AskNetwork(inputVector);

        public double NetworkWeightSum() => model.Layers.Sum(x => x.Neurons.Sum(y => y.Dendrites.Sum(z => z.SynapticWeight)));

        //STATIC MUTATROS AND CROSSBREEDERS

        public static NeuralNetwork CrossBread(NeuralNetwork father, NeuralNetwork mother) {
            if(father == mother) {
                var tempModel = father;
                for(var layerCounter = 0; layerCounter < tempModel.model.Layers.Count; layerCounter++) {
                    var handeldLayer = tempModel.model.Layers[layerCounter];
                    for(var neuronCounter = 0; neuronCounter < handeldLayer.Neurons.Count; neuronCounter++) {
                        var handledNeuron = handeldLayer.Neurons[neuronCounter];
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

        public static NeuralNetwork Mutate(NeuralNetwork network,double mutationPercentage = MUTATOR_PERCENTAGE,float mutationForce = 2) {
            var ret = network;
            var random = new System.Random();
            for(var layerCounter = 0; layerCounter < ret.model.Layers.Count; layerCounter++) {
                var handeldLayer = ret.model.Layers[layerCounter];
                for(var neuronCounter = 0; neuronCounter < handeldLayer.Neurons.Count; neuronCounter++) {
                    var handledNeuron = handeldLayer.Neurons[neuronCounter];
                    for(var dendriteCounter = 0; dendriteCounter < handledNeuron.Dendrites.Count; dendriteCounter++) {
                        var handledDendrite = handledNeuron.Dendrites[dendriteCounter];

                        //Decide if element shoud be modified
                        if(random.NextDouble() > MUTATOR_PERCENTAGE) {
                            //mutate element
                            var mutator = (random.NextDouble() * 2 - 1) * mutationForce;
                            ret.model.Layers[layerCounter].Neurons[neuronCounter].Dendrites[dendriteCounter].SynapticWeight = handledDendrite.SynapticWeight * mutator;
                        }
                    }
                }
            }
            return ret;
        }

        //ID ASSIGNERS

        private int nextNeuronId = 0;
        public int NeuronIdAssigner() {
            nextNeuronId++;
            return nextNeuronId - 1;
        }
        private int nextDendriteId = 0;
        public int DendriteIdAssigner() {
            nextDendriteId++;
            return nextDendriteId - 1;
        }

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
            var ret = new NeuralNetwork(0, 0) {
                model = model
            };
            return ret;
        }

        protected static void Serializer_UnknownNode(object sender, XmlNodeEventArgs e) => Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);

        protected static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e) => Console.WriteLine("Unknown attribute " + e.Attr.Name + "='" + e.Attr.Value + "'");

        // Operator Overrides

        public static bool operator ==(NeuralNetwork x, NeuralNetwork y) {
            return x.Equals(y);
        }
        public static bool operator !=(NeuralNetwork x, NeuralNetwork y) {
            return !x.Equals(y);
        }

        public override bool Equals(object value) {
            var other = value as NeuralNetwork;
            //TODO:: That's a lie
            return true;
                /*!System.Object.ReferenceEquals(null, other)
                && model.Layers.Count == other.model.Layers.Count //Layers count equality
                && model.Layers.Sum(x => x.Neurons.Count) == other.model.Layers.Sum(x => x.Neurons.Count)//Neuron count equality
                && model.Layers.Sum(x => x.Neurons.Sum(y => y.Dendrites.Count)) == other.model.Layers.Sum(x => x.Neurons.Sum(y => y.Dendrites.Count)) // Dendrites count equality
                && model.Layers.Any(x => other.model.Layers.Any(y => y.Neurons.Count == x.Neurons.Count && y.Name == x.Name)); // Neurons in Layers Count
                */
        }
    }
}
