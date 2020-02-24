using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class MonPetitCube : BasicCubeBehavior
{


    // Update is called once per frame
    void Update()
    {

        if(networkObject == null)
        {
            return;
        }


        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;

            return;
        }

        transform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized * 5f * Time.deltaTime;

        networkObject.position = transform.position;
        networkObject.rotation = transform.rotation;

    }
}
