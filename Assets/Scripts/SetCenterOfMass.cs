using UnityEngine;
using System.Collections;

public class SetCenterOfMass : MonoBehaviour {
	
	public Transform centerOfMass;
	// Use this for initialization
	void Start () {	
		//rigidbody.centerOfMass.y = 0;	
		GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition; //Hier legen wir den Schwerpunkt des Autos/Rigidbodys auf die Position des Objektes centerOfMass.	
	}
	
}
