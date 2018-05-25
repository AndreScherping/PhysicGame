using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {
	public WheelCollider leftFrontWheel;
	public WheelCollider rightFrontWheel;
	public WheelCollider leftBackWheel;
	public WheelCollider rightBackWheel;
	
	public Transform leftFrontVisuals;
	public Transform rightFrontVisuals;
	public Transform leftBackVisuals;
	public Transform rightBackVisuals;
    [SerializeField]
	private float maxTorque = 750.0F;
    [SerializeField]
    private float maxBrakeTorque = 900.0F;
    [SerializeField]
    private float steerAngle = 30;
    [SerializeField]
    private float maxSpeed = 250;
    [SerializeField]
    private float downforce = 100f;
    [SerializeField]
    private float slipLimit = 0.1F;
    //[SerializeField]
    //private float maxRpm = 0;
    [SerializeField]
    public float currentMotorTorque = 0;
	
	//private GameObject leftSteering;
	//private GameObject rightSteering;
	
	private Rigidbody myRigidbody;
	private float velocityMagnitude = 0;

    //[SerializeField]
    //public float speed;
    //[SerializeField]
    //public float currentSpeed;

	void Awake ()
	{	
		myRigidbody = GetComponent<Rigidbody>(); //Unity 5
        
		//Geschwindigkeit = Umfang * Drehungen pro Stunde / 1000 (für KM)
		//currentSpeed = (2 * Mathf.PI * leftBackWheel.radius) * leftFrontWheel.rpm * 60 /1000;
		/*
		//ergibt umgestellt eine maximale Anzahl an Umdrehungen pro Minute:
		maxRpm = maxSpeed * 1000 / (2* Mathf.PI * leftBackWheel.radius * 60);
		//Neues GameObject erzeugen und dem Namen "Left Steering" geben
		leftSteering = new GameObject("Left Steering");
		//Dem GameObject ein Elternobjekt zuweisen, und zwar das Hauptobjekt
		leftSteering.transform.parent = transform;
		//Position des Rad-Objektes zuweisen
		leftSteering.transform.position = leftFrontVisuals.position; 
		//Drehung des Rad-Objektes zuweisen
		leftSteering.transform.rotation = leftFrontVisuals.rotation;
		//Dem Rad-Objekt das neue Objekt als Elternobjekt zuweisen
		leftFrontVisuals.parent = leftSteering.transform;
		
		rightSteering = new GameObject("Right Steering");
		rightSteering.transform.parent = transform;
		rightSteering.transform.position = rightFrontVisuals.position; 
		rightSteering.transform.rotation = rightFrontVisuals.rotation;
		rightFrontVisuals.parent = rightSteering.transform;
		*/
	}
	
	void FixedUpdate () {
				

		float thrustTorque = 0;
		thrustTorque = currentMotorTorque * Input.GetAxis("Vertical");

		//float rpm = leftBackWheel.rpm;
		//Unity 5 -B
		velocityMagnitude = myRigidbody.velocity.magnitude; 
		float speed = velocityMagnitude;
		speed *= 3.6f;
		if (speed >= maxSpeed)
			thrustTorque = 0;

        /*
		if ((Mathf.Abs (rpm) >= maxRpm))
		{
			if (Mathf.Sign (rpm * currentMotorTorque)==1)
				currentMotorTorque = 0;
		}
		*/

        //Unity 5 -E
        //currentMotorTorque wird auf die 2 Antriebsraeder aufgeteilt
        leftBackWheel.motorTorque =  thrustTorque / 2;
        rightBackWheel.motorTorque = thrustTorque / 2;	

		if (Input.GetKey(KeyCode.Space))
		{
            leftBackWheel.brakeTorque = maxBrakeTorque;
            rightBackWheel.brakeTorque = maxBrakeTorque;
		}
		else
		{
            leftBackWheel.brakeTorque = 0;
            rightBackWheel.brakeTorque = 0;
		}
		
		leftFrontWheel.steerAngle = steerAngle * Input.GetAxis("Horizontal");
		rightFrontWheel.steerAngle = steerAngle *  Input.GetAxis("Horizontal");	

		AddDownForce(); //Stabilisierung

        TractionControl();
    }

    private void TractionControl()
    {

        WheelHit wheelHit;

        leftBackWheel.GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);

        rightBackWheel.GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);
    }

    private void AdjustTorque(float forwardSlip)
	{
		if (forwardSlip >= slipLimit && currentMotorTorque >= 0)
		{
			currentMotorTorque -= 1;
		}
		else
		{
			currentMotorTorque += 10;
			if (currentMotorTorque >  maxTorque)
			{
				currentMotorTorque = maxTorque;
			}
		}
	}

	// this is used to add more grip in relation to speed
	private void AddDownForce()
	{
		myRigidbody.AddForce(-transform.up * downforce * velocityMagnitude);
	}

	void Update()
	{
		//Unity 5 -B
		RefreshVisuals(leftFrontWheel,leftFrontVisuals);		
		RefreshVisuals(rightFrontWheel,rightFrontVisuals);
		RefreshVisuals(leftBackWheel,leftBackVisuals);
		RefreshVisuals(rightBackWheel,rightBackVisuals);

		//SteerVisuals(leftFrontWheel,leftSteering.transform);
		//SteerVisuals(rightFrontWheel,rightSteering.transform);
		//Unity 5 -E
	}

	//Unity 5 -B
	void RefreshVisuals(WheelCollider wc, Transform visualWheel)
	{			
		//visualWheel.Rotate(wc.rpm / 60 * 360 * Time.deltaTime,0,0);	
		Quaternion quat;
		Vector3 position;
		wc.GetWorldPose(out position, out quat);
		visualWheel.transform.position = position;
		visualWheel.transform.rotation = quat;
	}
	/*
	void SteerVisuals(WheelCollider wc, Transform steeringObject)
	{
		steeringObject.localEulerAngles  = new Vector3(0,wc.steerAngle,0);		
	}
	*/
	//Unity 5 -E
}
