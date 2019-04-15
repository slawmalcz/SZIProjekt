using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Vector4[,] neuralData {
        get {
            var temp = new Vector4[QUANTIFICATION_DEGREE, QUANTIFICATION_DEGREE];
            for(var i = 0; i < QUANTIFICATION_DEGREE; i++)
                for(var j = 0; j < QUANTIFICATION_DEGREE; j++) {
                    var relativePosition = transform.InverseTransformPoint(perceptionVolumes[i, j].perceptionFustum.position);
                    temp[i, j] = new Vector4(
                        relativePosition.x,
                        relativePosition.y,
                        relativePosition.z,
                        perceptionVolumes[i, j].FillingPercentage
                        );
                }
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
        GenerateDetectionGrid();
        //Debug.Log(GetDetectionData());
    }

    // Update is called once per frame
    void Update() {
        if(Time.frameCount % 20 == 0) {
            //Debug.Log(GetDetectionData());
        }

    }

    private void GenerateDetectionGrid() {
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

    private string GetDetectionDataString() {
        var ret = "";
        var linePattern = "";
        for(int i = 0; i < QUANTIFICATION_DEGREE + 1; i++) {
            linePattern += "{" + i + ",20}|";
        }
        linePattern = linePattern.Remove(linePattern.Length - 1);
        linePattern += " \n";

        var parameters = new List<string>();
        parameters.Add("\\");
        parameters.AddRange(Enumerable.Range(0, QUANTIFICATION_DEGREE).ToArray().Select(x => x.ToString()));
        ret = string.Format(linePattern, parameters.ToArray());

        for(int i = 0; i < QUANTIFICATION_DEGREE; i++) {
            parameters = new List<string>();
            parameters.Add(i.ToString());
            for(int j = 0; j < QUANTIFICATION_DEGREE; j++) {
                parameters.Add(neuralData[i, j].ToString());
            }
            ret += string.Format(linePattern, parameters.ToArray());
        }
        return ret;
    }

    public List<float> GetDetectionData() {
        var ret = new List<float>();
        for(var i = 0; i < QUANTIFICATION_DEGREE; i++) {
            for(var j = 0; j < QUANTIFICATION_DEGREE; j++) {
                ret.Add(neuralData[i, j].x);
                ret.Add(neuralData[i, j].y);
                ret.Add(neuralData[i, j].z);
                ret.Add(neuralData[i, j].w);
            }
        }
        return ret;
    }
}
