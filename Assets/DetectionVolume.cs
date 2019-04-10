using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
    class DetectionVolume : PerceptionVolume{
        public Material detectionOn;
        public Material detectionOff;

        public bool IsObstructed {
            get {
                return this.detectedObjects.Count > 0;
            }
        }

        private void Update() {
            if(IsObstructed) {
                GetComponent<Renderer>().material = detectionOn;
            }else {
                GetComponent<Renderer>().material = detectionOff;
            }
        }
    }
}
