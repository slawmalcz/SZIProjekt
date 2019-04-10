using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Percepton : MonoBehaviour {
    private int QUANTIFICATION_DEGREE = 10;
    public float XOfset = 0.75f;
    public float YOfset = 0.1f;
    public float ZOfset = 0.25f;

    public bool[,] byteColisionTable {
        get {
            var temp = new bool[QUANTIFICATION_DEGREE, QUANTIFICATION_DEGREE];
            for(var i = 0; i < QUANTIFICATION_DEGREE; i++)
                for(var j = 0; j < QUANTIFICATION_DEGREE; j++)
                    temp[i, j] = perceptionVolumes[i, j].IsObstructed;
            return temp;
        }
    }
    public GameObject bitLayerObject;
    private DetectionVolume[,] perceptionVolumes;

    public Material detectionOn;
    public Material detectionOff;

    // Use this for initialization
    void Start() {
        perceptionVolumes = new DetectionVolume[QUANTIFICATION_DEGREE, QUANTIFICATION_DEGREE];
        GenerateBiteTable();
    }

    // Update is called once per frame
    void Update() {

    }

    private void GenerateBiteTable() {
        var colliderPosition = transform.position;
        var colliderSize = new Vector3(5, 1, 5);
        var mesuringColiders = new BoxCollider[QUANTIFICATION_DEGREE, QUANTIFICATION_DEGREE];
        for(int i = 0; i < QUANTIFICATION_DEGREE; i++) {
            for(int j = 0; j < QUANTIFICATION_DEGREE; j++) {
                var temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                temp.name = ("PerceptionGrid[" + i + "][" + j + "]");
                temp.transform.parent = bitLayerObject.transform;
                temp.transform.position = new Vector3(
                    colliderPosition.x + (i * colliderSize.x / QUANTIFICATION_DEGREE) + XOfset,
                    colliderPosition.y + YOfset,
                    colliderPosition.z + (j * colliderSize.z / QUANTIFICATION_DEGREE) - colliderSize.z / 2 + ZOfset
                    );
                temp.transform.localScale = new Vector3(colliderSize.x / QUANTIFICATION_DEGREE, colliderSize.y, colliderSize.z / QUANTIFICATION_DEGREE);
                mesuringColiders[i, j] = temp.GetComponent<BoxCollider>();
                mesuringColiders[i, j].isTrigger = true;
                perceptionVolumes[i, j] = temp.AddComponent<DetectionVolume>();
                perceptionVolumes[i, j].detectionOn = detectionOn;
                perceptionVolumes[i, j].detectionOff = detectionOff;
            }
        }
    }
}
