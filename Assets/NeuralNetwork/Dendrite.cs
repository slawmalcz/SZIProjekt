using System;
using System.Xml.Serialization;

namespace Assets.NeuralNetwork {
    public class Dendrite {
        private static Random random;
        private static Random Random {
            get {
                if(random == null)
                    random = new Random();
                return random;
            }
        }
        public int DendriteId { get; set; }
        public readonly int neuronId;

        [XmlIgnoreAttribute]
        public Pulse InputPulse { get; set; }
        public double SynapticWeight { get; set; }

        private Dendrite(int id, int neuronId, double synapticWeight) {
            InputPulse = new Pulse();
            DendriteId = id;
            this.neuronId = neuronId;
            SynapticWeight = synapticWeight;
        }

        public Dendrite(int neuronId, Func<int> idAssigner) : this(idAssigner.Invoke(), neuronId, Random.NextDouble()) { }

        public Dendrite() : this(-1, -1, 0) { }
    }
}
