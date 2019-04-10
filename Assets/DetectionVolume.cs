using UnityEngine;

namespace Assets {
    class DetectionVolume : PerceptionVolume {
        public Material detectionOn;
        public Material detectionOff;

        public bool IsObstructed => detectedObjects.Count > 0;

        private void Update() {
            if(IsObstructed) {
                GetComponent<Renderer>().material = detectionOn;
            } else {
                GetComponent<Renderer>().material = detectionOff;
            }
        }

        private float CheckFillingPercentage() {
            var ret = 0.0f;
            if(detectedObjects.Count > 0) {

            }
            return ret;
        }
    }
}
