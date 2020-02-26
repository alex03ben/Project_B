using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;

public class PlayerNetworkManager : PlayerBehavior
{
    public override void UpdataName(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();


        if (!networkObject.IsOwner)
        {

            transform.GetChild(0).gameObject.SetActive(false);

            //GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerEngineController>().enabled = false;

            Destroy(GetComponent<CharacterController>());

        }

    }

    void Update()
    {

        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.position;
            return;
        }

        networkObject.position = transform.position;
    }
}
