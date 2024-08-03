using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalRoomManager : MonoBehaviour
{
    List<Collider> playersInRoom = new List<Collider>();
    public int numOfPlayersInRoom = 0;

    public static FinalRoomManager instance;
    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playersInRoom.Add(other);
            numOfPlayersInRoom = 1;
            other.GetComponent<PlayerState>().isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playersInRoom.Clear();
            numOfPlayersInRoom = 0;
            other.GetComponent<PlayerState>().isSafe = false;
        }
    }

}
