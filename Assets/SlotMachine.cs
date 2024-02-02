using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

//? so if you get nothing i give you the last shit gun
//? if you get 2 i give you medium gun's type
//? if you get 3 i give you advanced gun's type


[System.Serializable]
public class GameObjectArray
{
    public GameObject[] GameObjects;
    public GameObjectArray(GameObject[] _GameObjects)
    {
        this.GameObjects = _GameObjects;
    }
}

public class SlotMachine : NetworkBehaviour
{

    public TextMeshProUGUI[] SlotsText;
    public GameObjectArray[] Rewards;
    public int[] SlotNumbers;
    public NetworkIdentity networkIdentity;
    // Start is called before the first frame update
    void Start()
    {
        SlotNumbers = new int[3];
        CmdAssignAuthority(networkIdentity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void CmdAssignAuthority(NetworkIdentity targetIdentity)
    {
        targetIdentity.AssignClientAuthority(connectionToClient);
    }

    [Command]
    public void SpinSlots() {
        serverSpinSlots();
    }

    [Server]
    public void serverSpinSlots() {
        ClearReward();
        RollSlotNumbers();
        SlotsText[0].text = "" + SlotNumbers[0];
        SlotsText[1].text = "" + SlotNumbers[1];
        SlotsText[2].text = "" + SlotNumbers[2];
        GetReward();
    }

    void RollSlotNumbers() {
        SlotNumbers[0] = Random.Range(1, 7);
        SlotNumbers[1] = Random.Range(1, 7);
        SlotNumbers[2] = Random.Range(1, 7);
    }

//sign var
    [ClientRpc]
    void GetReward() {
        int score = 1;
        int type = 0;
        foreach (int slotNumber in SlotNumbers) {
            int temp_score = 0;
            foreach (int _slotNumber in SlotNumbers) {
                if (slotNumber == _slotNumber) {
                    temp_score++;
                }
            }
            if (temp_score >= score) {
                type = slotNumber;
                score = temp_score;
            }
            temp_score = 0;
        }

        Debug.LogError("" + score + ", " + type);
        Rewards[score - 1].GameObjects[type - 1].SetActive(true);
    }

    [ClientRpc]
    void ClearReward() {
        foreach (GameObjectArray objArray in Rewards) {
            foreach (GameObject obj in objArray.GameObjects) {
                obj.SetActive(false);
            }
        }
    }
}
