using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.NeuralNetwork {
    public class NeuralLayer {
        public List<Neuron> Neurons { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        private Func<int> neuronIdAssigner;


        public NeuralLayer(List<Neuron> neurons, List<List<Dendrite>> dendrits, Func<int> neuronIdAssigner, string name = "") {
            Neurons = neurons;
            this.neuronIdAssigner = neuronIdAssigner;
            foreach(var neuron in Neurons)
                neuron.Dendrites = dendrits.Find(x => x.Any(y => y.NeuronId == neuron.Id));
            Name = name;
        }

        public NeuralLayer(int count, Func<int> neuronIdAssigner, string name = "") {
            Neurons = new List<Neuron>();
            this.neuronIdAssigner = neuronIdAssigner;
            for(var i = 0; i < count; i++)
                Neurons.Add(new Neuron(neuronIdAssigner));
            Name = name;
        }

        public NeuralLayer() { }

        public void Forward() => Neurons.ForEach(x => x.Fire());
    }
}
