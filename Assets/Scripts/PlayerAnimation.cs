using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : Photon.MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer visualCharacter;
    public PlayerController controller;

    public PhotonView myPhotonView;

    //private Vector3 defPos = new Vector3(0.038f, 0.0556f, 1);
    //private Vector3 attPos = new Vector3(0.0589f, 0.0405f, 1);
    //private Vector3 sanPos = new Vector3(0.038f, 0.0556f, 1);
    //private Vector3 defScale = new Vector3(0.355892f, 0.1781093f, 0.3558928f);
    //private Vector3 attScale = new Vector3(0.5032316f, 0.2518461f, 1.413998f);
    //private Vector3 sanScale = new Vector3(0.4681058f, 0.2342671f, 0.4681058f);

    private void Awake()
    {
        anim = GetComponent<Animator>();

    }
    void Start()
    {
        switch (controller.whatCharacter)
        {
            case 0:
                //myPhotonView.RPC("CharacterSelected", PhotonTargets.AllBuffered, "def");
                break;
            case 1:
                //myPhotonView.RPC("CharacterSelected", PhotonTargets.AllBuffered, "att");
                break;
            case 2:
                //myPhotonView.RPC("CharacterSelected", PhotonTargets.AllBuffered, "san");
                break;
        }
    }
    [PunRPC]
    private void CharacterSelected(string skin)
    {
        try
        {
            if (skin == "def")
            {
                //visualCharacter.sprite = sDef;
                //anim.SetBool("isDef", true);
                //anim.SetBool("isAtt", false);
                //anim.SetBool("isSan", false);
                //visualCharacter.transform.localPosition = defPos;
                //visualCharacter.transform.localScale = defScale;
            }
            else if (skin == "att")
            {
                //visualCharacter.sprite = sAtt;
                //anim.SetBool("isDef", false);
                //anim.SetBool("isAtt", true);
                //anim.SetBool("isSan", false);
                //visualCharacter.transform.localPosition = attPos;
                //visualCharacter.transform.localScale = attScale;
            }
            else if (skin == "san")
            {
                //visualCharacter.sprite = sSan;
                //anim.SetBool("isDef", false);
                //anim.SetBool("isAtt", false);
                //anim.SetBool("isSan", true);
                //visualCharacter.transform.localPosition = sanPos;
                //visualCharacter.transform.localScale = sanScale;
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError("AttCharacterSelected " + ex);
            throw ex;
        }

    }
}
