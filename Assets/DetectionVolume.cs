using UnityEngine;

namespace Assets {
    class DetectionVolume : PerceptionVolume {
        public Material detectionOn;
        public Material detectionOff;

        public bool IsObstructed => detectedObjects.Count > 0;
        public float FillingPercentage => CheckFillingPercentage();

        private Renderer rendererReference;

        private void Start() => rendererReference = GetComponent<Renderer>();

        private void Update() => rendererReference.material = (IsObstructed) ? detectionOn : detectionOff;

        private float CheckFillingPercentage() {
            var ret = perceptionFustum;
            if(detectedObjects.Count > 0)
                foreach(var detected in detectedObjects)
                    ret -= new Utilities.BoxParameters(detected.GetComponent<BoxCollider>());
            return ret.Volume / perceptionFustum.Volume;
        }
    }
}
