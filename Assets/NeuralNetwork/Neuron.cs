using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.NeuralNetwork {
    public class Neuron {
        public int Id { get; set; }
        public List<Dendrite> Dendrites { get; set; }
        public double bias = 0;
        [XmlIgnore] public Pulse OutputPulse { get; set; }

        private Neuron(int id, List<Dendrite> dendrites) {
            Dendrites = dendrites;
            OutputPulse = new Pulse();
            bias = new Random(id).NextDouble();
            Id = id;
        }

        public Neuron(Func<int> idAssigner) : this(idAssigner.Invoke(), new List<Dendrite>()) { }

        public Neuron() : this(-1, new List<Dendrite>()) { }

        public void Fire() => OutputPulse.Value = Activation(Sum());

        private double Activation(double input) => 1.0 / (1.0 + Math.Pow(Math.E, (-1) * Sum()));

        private double Sum() {
            double computeValue = 0.0f;
            foreach(var d in Dendrites)
                computeValue += d.InputPulse.Value * d.SynapticWeight;
            return computeValue + bias;
        }
    }
}
