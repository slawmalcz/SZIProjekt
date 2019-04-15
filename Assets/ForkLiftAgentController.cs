using Assets.NeuralNetwork;
using System.Collections.Generic;
using UnityEngine;

public class ForkLiftAgentController : MonoBehaviour {
    public Percepton perception;
    public GameObject startPosition;
    public NeuralNetwork askedNetwork;

    public float MAX_SPEAD = 10;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        var networkData = new List<float> {
            startPosition.transform.position.x,
            startPosition.transform.position.y,
            startPosition.transform.position.z
            };
        networkData.AddRange(perception.GetDetectionData());
        var answer = askedNetwork.AskNetwork(networkData);
        //TODO:: Not shure of this
        var destination = new Vector3((float)answer[0], /*(float)answer[1]*/ 0, (float)answer[2]);
        var pointToTravel = destination * Time.deltaTime * MAX_SPEAD;
        var calculatedDestination = transform.position + pointToTravel;
        var lookAtPoint = calculatedDestination;
        lookAtPoint.y = transform.position.y;
        transform.LookAt(lookAtPoint);
        transform.position = calculatedDestination;
    }
}
