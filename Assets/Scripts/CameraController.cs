using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //variables
    public Transform playerTransform;
    public int depth = -20;

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            //set camera to player location
            transform.position = playerTransform.position + new Vector3(0, 0, depth);
        }
    }

    public void setTarget(Transform target)
    {
        //get current plater location
        playerTransform = target;
    }
}
