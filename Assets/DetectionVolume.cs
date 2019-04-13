using UnityEngine;

namespace Assets {
    class DetectionVolume : PerceptionVolume {
        public Material detectionOn;
        public Material detectionOff;

        public bool IsObstructed => detectedObjects.Count > 0;
        public float FillingPercentage => CheckFillingPercentage();

        private void Update() {
            if(IsObstructed) {
                GetComponent<Renderer>().material = detectionOn;
            } else {
                GetComponent<Renderer>().material = detectionOff;
            }
        }

        private float CheckFillingPercentage() {
            var ret = perceptionFustum;
            if(detectedObjects.Count > 0) {
                foreach(var detected in detectedObjects) {
                    var toDelete = ret - new Utilities.BoxParameters(detected.GetComponent<BoxCollider>());
                    ret -= new Utilities.BoxParameters(detected.GetComponent<BoxCollider>());
                }
            }
            return ret.Volume / perceptionFustum.Volume;
        }
    }
}
