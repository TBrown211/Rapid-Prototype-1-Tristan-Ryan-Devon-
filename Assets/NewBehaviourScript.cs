using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject playerObject;
    

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y);
        transform.position = position;
    }
}
