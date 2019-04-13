using Assets.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionVolume : MonoBehaviour {
    public BoxParameters perceptionFustum;
    public List<GameObject> detectedObjects;

    private void Awake() {
        detectedObjects = new List<GameObject>();
        perceptionFustum = new BoxParameters(GetComponent<BoxCollider>());
    }

    protected void OnTriggerEnter(Collider other) {
        detectedObjects.Add(other.gameObject);
    }

    protected void OnTriggerExit(Collider other) {
        detectedObjects.Remove(other.gameObject);
    }
}
