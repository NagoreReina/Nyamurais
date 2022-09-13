using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Photon.MonoBehaviour
{
    public Slider sLife;
    public float MaxLife;
    public float life;
    public GameController controller;
    public CthulhuController cthulhu;
    public PhotonView cPhotonView;

    //[PunRPC] 
    //public void ReduceHealth(float amount)
    //{
    //    ModifyHealth(amount);
    //}
    public void ModifyHealth(float amount)
    {
        life -= amount;
        if (PhotonNetwork.isMasterClient)
        {
            cPhotonView.RPC("SendLifeToOthers", PhotonTargets.AllBuffered, life);
        }
    }
    [PunRPC]
    public void SendLifeToOthers(float SendLifeToOthers)
    {
        life = SendLifeToOthers;
        sLife.value = SendLifeToOthers;
    }
    private void Start()
    {
        sLife = FindObjectOfType<Slider>();
        if (sLife == null)
            return;
        controller = FindObjectOfType<GameController>();
        if (controller == null)
            return;
        cPhotonView = gameObject.GetComponent<PhotonView>();
        if (cPhotonView == null)
            return;
        if (PhotonNetwork.isMasterClient)
        {
            sLife.maxValue = controller.numOfPlayers * 500; //La vida del enemigo depende del numero de jugadores
            MaxLife = sLife.maxValue;
            cPhotonView.RPC("SetMaxLife", PhotonTargets.AllBuffered, MaxLife);
        }     
    }
    private void Update()
    {
        sLife.value = life;
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(life);
        }
        else
        {
            life = (float)stream.ReceiveNext();
        }
    }
    [PunRPC]
    private void SetMaxLife (float amount)
    {
        MaxLife = amount;
        sLife.maxValue = MaxLife;
        sLife.value = MaxLife;
        life = sLife.value;
    }
}
