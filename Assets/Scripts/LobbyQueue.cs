using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LobbyQueue
{
    private static Queue<NPCController> waitingNPCs = new Queue<NPCController>();

    public static void AddToQueue(NPCController npc)
    {
        waitingNPCs.Enqueue(npc);
        npc.StartWaitingForBed();
    }

    public static void NotifyNextNPC()
    {
        if (waitingNPCs.Count > 0)
        {
            NPCController nextNPC = waitingNPCs.Dequeue();
            nextNPC.CheckForAvailableBed();
        }
    }
}