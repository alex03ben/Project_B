using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

public class NetworkLogicManager : GameLogicBehavior
{
    public override void PlayerScored(RpcArgs args)
    {

        string playerName = args.GetNext<string>();
        print("vla le player: " + playerName);
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Instance.InstantiatePlayer(position: new Vector3(14, 2, -19));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
