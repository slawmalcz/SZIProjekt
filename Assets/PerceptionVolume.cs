using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionVolume : MonoBehaviour {
    public Collider perceptionFustum;
    public List<GameObject> detectedObjects;

    private void Awake() {
        detectedObjects = new List<GameObject>();
        perceptionFustum = GetComponent<Collider>();
    }

    protected void OnTriggerEnter(Collider other) {
        detectedObjects.Add(other.gameObject);
    }

    protected void OnTriggerExit(Collider other) {
        detectedObjects.Remove(other.gameObject);
    }
}
