using UnityEngine;
using System.Collections;

public class BACKUP_CarController : MonoBehaviour
{
    public WheelCollider leftFrontWheel;
    public WheelCollider rightFrontWheel;
    public WheelCollider leftBackWheel;
    public WheelCollider rightBackWheel;

    public Transform leftFrontVisuals;
    public Transform rightFrontVisuals;
    public Transform leftBackVisuals;
    public Transform rightBackVisuals;

    public float maxTorque = 50.0F;
    public float maxBrakeTorque = 100.0F;
    public float steerAngle = 30;
    public float maxSpeed = 120;
    public float downforce = 100f;
    public float slipLimit = 0.3F;
    //private float maxRpm = 0;
    public float currentMotorTorque = 0;

    //private GameObject leftSteering;
    //private GameObject rightSteering;

    private Rigidbody myRigidbody;
    private float velocityMagnitude = 0;
    //public float speed;
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>(); //Unity 5

        //Geschwindigkeit = Umfang * Drehungen pro Stunde / 1000 (für KM)
        //float currentSpeed = (2 * Mathf.PI * leftFrontWheel.radius) * leftFrontWheel.rpm * 60 /1000;
        /*
		//ergibt umgestellt eine maximale Anzahl an Umdrehungen pro Minute:
		//maxRpm = maxSpeed * 1000 / (2* Mathf.PI * leftFrontWheel.radius * 60);
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

    void FixedUpdate()
    {


        float thrustTorque = 0;
        thrustTorque = currentMotorTorque * Input.GetAxis("Vertical");

        //float rpm = leftFrontWheel.rpm;
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
        leftFrontWheel.motorTorque = thrustTorque / 2;
        rightFrontWheel.motorTorque = thrustTorque / 2;

        if (Input.GetKey(KeyCode.Space))
        {
            leftFrontWheel.brakeTorque = maxBrakeTorque;
            rightFrontWheel.brakeTorque = maxBrakeTorque;
        }
        else
        {
            leftFrontWheel.brakeTorque = 0;
            rightFrontWheel.brakeTorque = 0;
        }

        leftFrontWheel.steerAngle = steerAngle * Input.GetAxis("Horizontal");
        rightFrontWheel.steerAngle = steerAngle * Input.GetAxis("Horizontal");

        AddDownForce(); //Stabilisierung

        TractionControl();
    }

    private void TractionControl()
    {

        WheelHit wheelHit;

        leftFrontWheel.GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);

        rightFrontWheel.GetGroundHit(out wheelHit);
        AdjustTorque(wheelHit.forwardSlip);
    }

    private void AdjustTorque(float forwardSlip)
    {
        if (forwardSlip >= slipLimit && currentMotorTorque >= 0)
        {
            currentMotorTorque -= 10;
        }
        else
        {
            currentMotorTorque += 10;
            if (currentMotorTorque > maxTorque)
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
        RefreshVisuals(leftFrontWheel, leftFrontVisuals);
        RefreshVisuals(rightFrontWheel, rightFrontVisuals);
        RefreshVisuals(leftBackWheel, leftBackVisuals);
        RefreshVisuals(rightBackWheel, rightBackVisuals);

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
