using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{    
    public Transform player;    //Creating a reference to keep track of players position
    private Vector3 offset = new Vector3(0, 0.73f, -0.759f); //The position offset for the jetpack
    private Vector3 objectRot = new Vector3(15.09f, 0, 0); //The rotation offset
    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        transform.position = player.position + player.rotation * offset;

        Quaternion angle = Quaternion.Euler(objectRot);
        transform.rotation = player.rotation * angle; 

    }
}
