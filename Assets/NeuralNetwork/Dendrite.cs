using System;
using System.Xml.Serialization;

namespace Assets.NeuralNetwork {
    public class Dendrite {
        public int Id { get; set; }
        private static Random random;
        private static Random Random {
            get {
                if(random == null) {
                    random = new Random();
                }
                return random;
            }
        }
        public readonly int NeuronId;

        [XmlIgnoreAttribute]
        public Pulse InputPulse { get; set; }
        public double SynapticWeight { get; set; }

        public Dendrite(int id, int neuronId, double synapticWeight) {
            InputPulse = new Pulse();
            Id = id;
            NeuronId = neuronId;
            SynapticWeight = synapticWeight;
        }

        public Dendrite(int neuronId, Func<int> idAssigner) : this(idAssigner.Invoke(), neuronId, Random.NextDouble()) { }

        //TEST ONLY
        public Dendrite() : this(-1, -1, 0) { }
    }
}
