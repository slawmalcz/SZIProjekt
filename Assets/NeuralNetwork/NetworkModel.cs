using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.NeuralNetwork {
    [XmlRoot("NeuralNetwork", IsNullable = false)]
    public class NetworkModel {
        public List<NeuralLayer> Layers { get; set; }
        public bool IsBuild { get; set; }
        [XmlIgnoreAttribute]
        public Func<int> dendriteIdAssigner;

        public NetworkModel() => Layers = new List<NeuralLayer>();

        public void AddLayer(NeuralLayer layer) {
            if(IsBuild) return;
            if(Layers.Count == 0)
                layer.Neurons.ForEach(x => x.Dendrites.Add(new Dendrite(x.Id, dendriteIdAssigner)));
            Layers.Add(layer);
        }

        public void Build() {
            for(var i = 0; i < Layers.Count - 1; i++) {
                CreateNetwork(Layers[i], Layers[i + 1]);
            }
            IsBuild = true;
        }

        public List<double> AskNetwork(List<float> inputVector) {
            if(Layers.First().Neurons.Count != inputVector.Count)
                Debug.LogError($"Network have {Layers.First().Neurons.Count} inputs. But input vector have {inputVector.Count} positions");
            //Puting input data in first layer
            for(var i = 0; i < inputVector.Count; i++)
                Layers.First().Neurons[i].Dendrites.First().InputPulse = new Pulse() { Value = inputVector[i] };
            //Pushing data throu network
            Layers.ForEach(x => x.Forward());
            //Reading outputvalue
            return Layers.Last().Neurons.Select(x => x.OutputPulse.Value).ToList();
        }

        public string Print() {
            var ret = "";
            ret = string.Format("{0,10}|{1,10} \n", "Name", "Neurons");

            foreach(var element in Layers)
                ret += string.Format("{0,10}|{1,10} \n", element.Name, element.Neurons.Count);
            return ret;
        }

        private void CreateNetwork(NeuralLayer connectingFrom, NeuralLayer connectingTo) {
            foreach(var to in connectingTo.Neurons) {
                to.Dendrites = new List<Dendrite>();
                foreach(var from in connectingFrom.Neurons) {
                    to.Dendrites.Add(new Dendrite(from.Id, dendriteIdAssigner) { InputPulse = from.OutputPulse });
                }
            }
        }
    }
}
