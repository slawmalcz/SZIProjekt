using Assets.Utilities;
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
        if(other.gameObject.CompareTag("Ground")) return;
        detectedObjects.Add(other.gameObject);
    }

    protected void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Ground")) return;
        detectedObjects.Remove(other.gameObject);
    }
}
