using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.NeuralNetwork {
    public class Neuron {
        private const double ACTIVATION_TRESHOLD = 1;

        public int Id { get; set; }

        public List<Dendrite> Dendrites { get; set; }

        [XmlIgnoreAttribute]
        public Pulse OutputPulse { get; set; }

        public Neuron(int id, List<Dendrite> dendrites) {
            Dendrites = dendrites;
            OutputPulse = new Pulse();
            Id = id;
        }

        public Neuron(Func<int> idAssigner) : this(idAssigner.Invoke(), new List<Dendrite>()) { }

        public Neuron() : this(-1, new List<Dendrite>()) { }

        public void Fire() => OutputPulse.Value = Activation(Sum());

        //TODO:: This implicate that this neural network is binary
        private double Activation(double input) => input / Dendrites.Count;

        private double Sum() {
            double computeValue = 0.0f;
            foreach(var d in Dendrites)
                computeValue += d.InputPulse.Value * d.SynapticWeight;
            return computeValue;
        }

    }
}
