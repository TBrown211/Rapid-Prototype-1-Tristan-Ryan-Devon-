using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;    
    private Vector3 offset = new Vector3(0, 0.73f, -0.759f);

    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        transform.position = player.position + player.rotation * offset;

        transform.rotation = player.rotation;     
    }
}
