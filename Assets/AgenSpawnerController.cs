using Assets.NeuralNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AgenSpawnerController : MonoBehaviour {
    private const string NEURAL_PATH_PREFAB = "Neurals/NET{0}.xml";
    private const string MAIN_NETWORK = "Neurals/NETMain.xml";
    private const int AGENT_LIFE_TIME = 10;
    private const int POPULATION_SIZE = 100;
    private const int GENERATION_SIZE = 200;
    private const int TIME_SPEED = 5;
    public GameObject agentPrefab;
    public GameObject pointerPrefab;
    public GameObject pointerLayer;
    private NeuralNetwork modelInUse;

    public GameObject destination;

    private GameObject currentAgent;
    private float currentAgentSpawningTime;
    private List<Tuple<float, string>> population;
    private int generations = 0;
    private bool IsBusy = false;
    // Start is called before the first frame update
    void Start() {
        population = new List<Tuple<float, string>>();
        if(File.Exists(MAIN_NETWORK)) {
            modelInUse = NeuralNetwork.Deserialize(MAIN_NETWORK);
        } else {
            modelInUse = new NeuralNetwork(403, 3);
            modelInUse.AddNextLayer(new NeuralLayer(400, modelInUse.NeuronIdAssigner, "H(400)"));
            //modelInUse.AddNextLayer(new NeuralLayer(200, modelInUse.NeuronIdAssigner, "H(200)"));
            modelInUse.AddNextLayer(new NeuralLayer(100, modelInUse.NeuronIdAssigner, "H(100)"));
            //modelInUse.AddNextLayer(new NeuralLayer(50, modelInUse.NeuronIdAssigner, "H(50)"));
            modelInUse.AddNextLayer(new NeuralLayer(20, modelInUse.NeuronIdAssigner, "H(20)"));
            modelInUse.AddNextLayer(new NeuralLayer(10, modelInUse.NeuronIdAssigner, "H(10)"));
            modelInUse.Build();
        }
        Time.timeScale = TIME_SPEED;
    }

    // Update is called once per frame
    void Update() {
        if(IsBusy) return;
        if(generations < GENERATION_SIZE) {
            if(population.Count < POPULATION_SIZE) {
                //Spawning new agent
                if(currentAgent == null) {
                    Debug.Log("Spawning new agent");
                    currentAgent = Instantiate(agentPrefab);
                    currentAgent.transform.position = transform.position;
                    currentAgent.GetComponent<ForkLiftAgentController>().askedNetwork =
                        NeuralNetwork.Mutate(modelInUse, 0.05 + ((100 - generations) / 100) * 0.5);
                    currentAgent.GetComponent<ForkLiftAgentController>().startPosition = destination;
                    currentAgentSpawningTime = Time.fixedTime;
                    //Debug.Log(string.Format("SpawnedAgent {0} has weght sum of {1}", population.Count, currentAgent.GetComponent<ForkLiftAgentController>().askedNetwork.NetworkWeightSum()));
                }
                //Ending agent
                if(currentAgentSpawningTime + AGENT_LIFE_TIME < Time.fixedTime) {
                    var distance = Vector3.Distance(destination.transform.position, currentAgent.transform.position);
                    //Debug.Log(distance);
                    var path = string.Format(NEURAL_PATH_PREFAB, population.Count);
                    currentAgent.GetComponent<ForkLiftAgentController>().askedNetwork.Serialize(path);
                    population.Add(new Tuple<float, string>(distance, path));
                    //Instantiet pointer
                    var pointer = Instantiate(pointerPrefab);
                    pointer.transform.parent = pointerLayer.transform;
                    pointer.transform.Translate(new Vector3(currentAgent.transform.position.x, pointer.transform.position.y, currentAgent.transform.position.z));
                    Destroy(currentAgent);
                    currentAgent = null;
                }
            } else {
                Time.timeScale = 0;
                IsBusy = true;
                var minValue = population.Min(x => x.Item1);
                var winner1 = population.Where(x => x.Item1 == minValue).First();
                var winner2 = population.OrderBy(x => x.Item1).Skip(1).FirstOrDefault();
                Debug.Log(string.Format("Best performace {0} {1}", winner1.Item1, winner1.Item2));
                Debug.Log(string.Format("Best performace {0} {1}", winner2.Item1, winner2.Item2));
                var des1 = NeuralNetwork.Deserialize(winner1.Item2);
                var des2 = NeuralNetwork.Deserialize(winner2.Item2);
                var newNetwork = NeuralNetwork.CrossBread(des1, des2);
                //RemoveOldData();
                newNetwork.Serialize(MAIN_NETWORK);
                modelInUse = newNetwork;
                population = new List<Tuple<float, string>>();
                generations++;
                IsBusy = false;
                Time.timeScale = TIME_SPEED;
            }
        } else {
            Destroy(this);
        }

    }

    private static void RemoveOldData() {
        var di = new DirectoryInfo("Neurals");

        foreach(var file in di.GetFiles()) {
            file.Delete();
        }
    }
}
