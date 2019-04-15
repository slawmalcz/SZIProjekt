using Assets.NeuralNetwork;
using System.Collections.Generic;
using UnityEngine;

public class ForkLiftAgentController : MonoBehaviour {
    public Percepton perception;
    public GameObject targetPlace;
    public NeuralNetwork askedNetwork;
    public int collisions = 0;

    public float MAX_SPEAD = 1;

    // Update is called once per frame
    void Update() {
        var networkData = new List<float> {
            targetPlace.transform.position.x,
            targetPlace.transform.position.y,
            targetPlace.transform.position.z,
            transform.position.x,
            transform.position.y,
            transform.position.z,
            };
        networkData.AddRange(perception.GetDetectionData());
        var answer = askedNetwork.AskNetwork(networkData);
        //Conversion form vector 0..1 to -1..1
        var destination = new Vector3((float)answer[0] * 2 - 1, (float)answer[1] * 2 - 1, (float)answer[2] * 2 - 1);
        var pointToTravel = destination * Time.deltaTime * MAX_SPEAD;
        var calculatedDestination = transform.position + pointToTravel;
        var lookAtPoint = calculatedDestination;
        lookAtPoint.y = transform.position.y;
        transform.LookAt(lookAtPoint);
        transform.position = calculatedDestination;
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Ground")) return;
        collisions++;
    }
}
