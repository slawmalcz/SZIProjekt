using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForkLiftAgentController : MonoBehaviour {
    public Percepton perception;
    public GameObject startPosition;

    public float MAX_SPEAD = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if((transform.position - startPosition.transform.position).magnitude > 2) {
            var pointToTravel = (startPosition.transform.position - transform.position) * Time.deltaTime * MAX_SPEAD;
            transform.position = transform.position + pointToTravel;
        }
		
	}
}
