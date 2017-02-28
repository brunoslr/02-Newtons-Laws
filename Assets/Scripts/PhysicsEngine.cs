using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PhysicsEngine : MonoBehaviour {
	public float mass;				// [kg]
	public Vector3 velocityVector;  // [m / s] or [m s^-1]

    [Tooltip ("Sum of Forces Acting Upon the Engine")]
	public Vector3 netForceVector;  // N [kg m s^-2]
	
	private List<Vector3> forceVectorList = new List<Vector3>();

    private PhysicsEngine[] physicsEngineArray;

    private const float bigG = 6.673e-11f; // [m^3 s^-2 kg^-1]

    // Use this for initialization
    void Start () {
		SetupThrustTrails();
        physicsEngineArray = GameObject.FindObjectsOfType<PhysicsEngine>();
    }
	
	void FixedUpdate () {
        CalculateGravity();
        RenderTrails();
        UpdateVelocity();
        UpdatePosition ();
    }

    private void UpdatePosition()
    {
        transform.position += velocityVector * Time.deltaTime;
    }

    private void UpdateVelocity()
    {
        netForceVector = forceVectorList.Aggregate(Vector3.zero, (acc, cur) => (acc + cur));
        forceVectorList = new List<Vector3>();
        Vector3 accelerationVector = netForceVector / mass;
        velocityVector =  accelerationVector*Time.deltaTime;
    }


    public void AddForce(Vector3 forceVector)
    {
        forceVectorList.Add(forceVector);
        Debug.Log("Adding force " + forceVector + " to " + gameObject.name);
    }

    void CalculateGravity()
    {
        foreach (var physicsEngineA in physicsEngineArray)
        {
            foreach (var physicsEngineB in physicsEngineArray)
            {
                if (physicsEngineA != physicsEngineB && physicsEngineA != this)
                {
                    Vector3 offset = physicsEngineA.transform.position - physicsEngineB.transform.position;
                    float rSquared = Mathf.Pow(offset.magnitude, 2f);
                    float gravityMagnitude = bigG * physicsEngineA.mass * physicsEngineB.mass / rSquared;
                    Vector3 gravityFeltVector = gravityMagnitude * offset.normalized;
                    physicsEngineA.AddForce(-gravityFeltVector);
                }

            }
        }
    }


    /// <summary>
    /// Code for drawing thrust trails
    /// </summary>
    public bool showTrails = true;
	
	private LineRenderer lineRenderer;
	private int numberOfForces;
	
	// Use this for initialization
	void SetupThrustTrails () {
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.startWidth = 0.2F;
        lineRenderer.endWidth = 0.2F;
        lineRenderer.useWorldSpace = false;
        /*
        		
    */
    }

    // Update is called once per frame
    void RenderTrails () {
		if (showTrails) {
			lineRenderer.enabled = true;
			numberOfForces = forceVectorList.Count;
			lineRenderer.numPositions = (numberOfForces * 2);
			int i = 0;
			foreach (Vector3 forceVector in forceVectorList) {
				lineRenderer.SetPosition(i, Vector3.zero);
				lineRenderer.SetPosition(i+1, -forceVector);
				i = i + 2;
			}
		} else {
			lineRenderer.enabled = false;
		}
	}

}
