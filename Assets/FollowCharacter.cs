using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform player;

    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        transform.position = new Vector3(player.position.x, player.position.y + 0.73f, player.position.z - 0.759f);
        
      
    }
}
