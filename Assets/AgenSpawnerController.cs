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

    public int AgentLifeTime = 5;
    public int PopulationSize = 10;
    public int GenerationSize = 200;
    public int TimeAcceleration = 1;
    public int[] networkLayout;

    public double mutationBasicChance = 0.05;
    public double mutationDegradingCance = 0.1;
    private double MutationChance => ((GenerationSize - currentGeneration) / GenerationSize) * mutationDegradingCance + mutationBasicChance;

    public double mutationBasicForce = 2;
    public double mutationDegradingFoerce = 1;
    private double MutationForce => mutationBasicForce + ((GenerationSize - currentGeneration) / GenerationSize) * mutationDegradingFoerce;

    public GameObject agentPrefab;
    public GameObject pointerPrefab;
    public GameObject pointerLayer;
    public GameObject destination;

    private NeuralNetwork modelInUse;
    private GameObject currentAgent;
    private float currentAgentSpawningTime;

    private List<Tuple<float, string>> population;
    private int currentGeneration = 0;

    private bool IsBusy = false;

    private double SecondBestResoult => (population.Count < 2) ? double.PositiveInfinity : population.OrderBy(x => x.Item1).ToArray()[1].Item1;

    void Start() {
        //Loading default network layout if needed
        if(networkLayout == null) networkLayout = new int[] { 406, 1600, 100, 3 };
        population = new List<Tuple<float, string>>();
        Time.timeScale = TimeAcceleration;
        if(File.Exists(MAIN_NETWORK)) {
            //Loading exisiting network
            modelInUse = NeuralNetwork.Deserialize(MAIN_NETWORK);
        } else {
            //Creating new network by given specification
            modelInUse = new NeuralNetwork(networkLayout.First(), networkLayout.Last());
            for(var i = 1; i < networkLayout.Length - 1; i++)
                modelInUse.AddNextLayer(new NeuralLayer(networkLayout[i], modelInUse.NeuronIdAssigner));
            modelInUse.Build();
        }
    }

    void Update() {
        if(IsBusy) return;
        if(currentGeneration < GenerationSize) {
            if(population.Count < PopulationSize) {
                //Spawning new agent
                if(currentAgent == null) {
                    currentAgent = Instantiate(agentPrefab);
                    currentAgent.transform.position = transform.position;
                    currentAgent.GetComponent<ForkLiftAgentController>().askedNetwork = NeuralNetwork.Mutate(modelInUse.Clone(), MutationChance, (float)MutationForce);
                    currentAgent.GetComponent<ForkLiftAgentController>().targetPlace = destination;
                    currentAgentSpawningTime = Time.fixedTime;
                }
                //Ending agent
                if(currentAgentSpawningTime + AgentLifeTime < Time.fixedTime) {
                    //Calculating succes rate
                    var succesRate = Vector3.Distance(destination.transform.position, currentAgent.transform.position)
                        + currentAgent.GetComponent<ForkLiftAgentController>().collisions;
                    var path = string.Format(NEURAL_PATH_PREFAB, population.Count);
                    if(succesRate < SecondBestResoult) {
                        //Seriallization take time and i dont need to save networks with bad resoults
                        currentAgent.GetComponent<ForkLiftAgentController>().askedNetwork.Serialize(path);
                    }
                    population.Add(new Tuple<float, string>(succesRate, path));
                    //Instantiet pointer
                    var pointer = Instantiate(pointerPrefab);
                    pointer.transform.parent = pointerLayer.transform;
                    pointer.transform.Translate(new Vector3(currentAgent.transform.position.x, pointer.transform.position.y, currentAgent.transform.position.z));
                    //Cleaning after agent
                    Destroy(currentAgent);
                    currentAgent = null;
                }
            } else {
                IsBusy = true;
                population = population.OrderBy(x => x.Item1).ToList();
                var winner1 = population.First();
                var winner2 = population.Skip(1).First();
                var des1 = NeuralNetwork.Deserialize(winner1.Item2);
                var des2 = NeuralNetwork.Deserialize(winner2.Item2);
                var newNetwork = NeuralNetwork.CrossBread(des1, des2);
                newNetwork.Serialize(MAIN_NETWORK);
                modelInUse = newNetwork;
                //Cleaning all pointers
                foreach(Transform child in pointerLayer.transform)
                    GameObject.Destroy(child.gameObject);
                population = new List<Tuple<float, string>>();
                currentGeneration++;
                IsBusy = false;
            }
        } else {
            Destroy(this);
        }
    }
}
