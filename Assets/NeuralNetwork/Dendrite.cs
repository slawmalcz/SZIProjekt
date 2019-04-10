namespace Assets.NeuralNetwork {
    class Dendrite {
        public Dendrite() {
            InputPulse = new Pulse();
        }

        public Pulse InputPulse { get; set; }

        public double SynapticWeight { get; set; }

        public bool Learnable { get; set; } = true;
    }
}
