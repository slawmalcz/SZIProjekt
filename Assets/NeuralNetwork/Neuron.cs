using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.NeuralNetwork {
    class Neuron {
        public List<Dendrite> Dendrites { get; set; }

        public Pulse OutputPulse { get; set; }

        public Neuron() {
            Dendrites = new List<Dendrite>();
            OutputPulse = new Pulse();
        }

        public void Fire() {
            OutputPulse.Value = Sum();

            OutputPulse.Value = Activation(OutputPulse.Value);
        }

        public void UpdateWeights(double new_weights) {
            foreach(var terminal in Dendrites) {
                terminal.SynapticWeight = new_weights;
            }
        }

        private double Sum() {
            double computeValue = 0.0f;
            foreach(var d in Dendrites) {
                computeValue += d.InputPulse.Value * d.SynapticWeight;
            }

            return computeValue;
        }

        private double Activation(double input) {
            double threshold = 1;
            return input <= threshold ? 0 : threshold;
        }
    }
}
