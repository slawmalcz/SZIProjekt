using Assets.NeuralNetwork;
using System.Collections.Generic;
using UnityEngine;

public class ForkLiftAgentController : MonoBehaviour {
    public Percepton perception;
    public GameObject startPosition;
    private NeuralNetwork askedNetwork;

    public float MAX_SPEAD = 1;

    // Use this for initialization
    void Start() {
        askedNetwork = new NeuralNetwork(403, 3);
        //askedNetwork.AddNextLayer(new NeuralLayer(400, askedNetwork.NeuronIdAssigner, "H(400)"));
        //askedNetwork.AddNextLayer(new NeuralLayer(200, askedNetwork.NeuronIdAssigner, "H(200)"));
        //askedNetwork.AddNextLayer(new NeuralLayer(100, askedNetwork.NeuronIdAssigner, "H(100)"));
        //askedNetwork.AddNextLayer(new NeuralLayer(50, askedNetwork.NeuronIdAssigner, "H(50)"));
        //askedNetwork.AddNextLayer(new NeuralLayer(20, askedNetwork.NeuronIdAssigner, "H(20)"));
        //askedNetwork.AddNextLayer(new NeuralLayer(10, askedNetwork.NeuronIdAssigner, "H(10)"));
        askedNetwork.Build(true);
    }

    // Update is called once per frame
    void Update() {
        if(Time.frameCount % 10 == 0) {
            var networkData = new List<float> {
            startPosition.transform.position.x,
            startPosition.transform.position.y,
            startPosition.transform.position.z
            };
            networkData.AddRange(perception.GetDetectionData());
            var answer = askedNetwork.AskNetwork(networkData);
            var destination = new Vector3((float)answer[0], (float)answer[1], (float)answer[2]);
            var pointToTravel = (destination - transform.position) * Time.deltaTime * MAX_SPEAD;
            transform.position = transform.position + pointToTravel;
        }
        if((transform.position - startPosition.transform.position).magnitude < 2 || Time.fixedTime > 20) {
            Time.timeScale = 0;
        }

    }
}
