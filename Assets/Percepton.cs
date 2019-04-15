using Assets;
using System.Collections.Generic;
using UnityEngine;

public class Percepton : MonoBehaviour {
    public int quantificationDegree = 10;
    public float XOfset = 0.75f;
    public float YOfset = 0.1f;
    public float ZOfset = 0.25f;

    public Vector4[,] NeuralData {
        get {
            var temp = new Vector4[quantificationDegree, quantificationDegree];
            for(var i = 0; i < quantificationDegree; i++)
                for(var j = 0; j < quantificationDegree; j++) {
                    var relativePosition = transform.InverseTransformPoint(perceptionVolumes[i, j].perceptionFustum.position);
                    temp[i, j] = new Vector4(relativePosition.x, relativePosition.y, relativePosition.z, perceptionVolumes[i, j].FillingPercentage);
                }
            return temp;
        }
    }


    public GameObject bitLayerObject;
    private DetectionVolume[,] perceptionVolumes;

    public Material detectionOn;
    public Material detectionOff;

    void Start() {
        perceptionVolumes = new DetectionVolume[quantificationDegree, quantificationDegree];
        GenerateDetectionGrid();
    }

    private void GenerateDetectionGrid() {
        var colliderPosition = transform.position;
        var colliderSize = new Vector3(5, 1, 5);
        var mesuringColiders = new BoxCollider[quantificationDegree, quantificationDegree];
        for(var i = 0; i < quantificationDegree; i++) {
            for(var j = 0; j < quantificationDegree; j++) {
                var temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                temp.name = ("PerceptionGrid[" + i + "][" + j + "]");
                temp.transform.parent = bitLayerObject.transform;
                temp.transform.position = new Vector3(
                    colliderPosition.x + (i * colliderSize.x / quantificationDegree) + XOfset,
                    colliderPosition.y + YOfset,
                    colliderPosition.z + (j * colliderSize.z / quantificationDegree) - colliderSize.z / 2 + ZOfset
                    );
                temp.transform.localScale = new Vector3(colliderSize.x / quantificationDegree, colliderSize.y, colliderSize.z / quantificationDegree);
                mesuringColiders[i, j] = temp.GetComponent<BoxCollider>();
                mesuringColiders[i, j].isTrigger = true;
                perceptionVolumes[i, j] = temp.AddComponent<DetectionVolume>();
                perceptionVolumes[i, j].detectionOn = detectionOn;
                perceptionVolumes[i, j].detectionOff = detectionOff;
            }
        }
    }

    public List<float> GetDetectionData() {
        var ret = new List<float>();
        for(var i = 0; i < quantificationDegree; i++) {
            for(var j = 0; j < quantificationDegree; j++) {
                ret.Add(NeuralData[i, j].x);
                ret.Add(NeuralData[i, j].y);
                ret.Add(NeuralData[i, j].z);
                ret.Add(NeuralData[i, j].w);
            }
        }
        return ret;
    }
}
