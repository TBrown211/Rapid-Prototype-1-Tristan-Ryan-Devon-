using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackFlight : MonoBehaviour
{
    private float flightForce = 50f; //the amount of force that the jetpack will propel to    
    public float flightTime = 10f; //how much time that the player will fly for
    private float fuelConsumption = 1f;
    public bool flying; //a boolean to keep track when player is flying
    public bool grounded; //a boolean to keep track of when player is on ground

    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = true;

        if(Input.GetKey(KeyCode.Space) && flightTime > 0f)
        {
            grounded = false;
        }
        else
        {
            grounded = true;
            flying = false;
        }

        if(flying == true && flightTime > 0f)
        {
            rb.AddForce(Vector3.up * flightForce, ForceMode.Force);
            flightTime -= Time.deltaTime;
        }
        
    }
}
