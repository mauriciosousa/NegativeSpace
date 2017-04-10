using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using System.Collections.Generic;
using System.Net;

public class SurfaceMessage
{
    public static string createRequestMessage(int port)
    {
        return "SurfaceMessage" + MessageSeparators.L0 + Network.player.ipAddress + MessageSeparators.L1 + port;
    }
}
