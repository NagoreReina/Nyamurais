using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public List <GameObject> characters;
    public List<GameObject> playerSpawns;
    public int whatCharacter, whatSpawn;
    public int numOfPlayers;
    public TextMeshProUGUI txtPing, txtPlayers;
    public GameObject pnlYourCharacter;
    public TextMeshProUGUI txtYourCharacter;
    public SpriteRenderer srYourCharacter;
    public Sprite sDef, sAtt, sSan;
    private float yourCharTimmer;
    public bool infoPanelIsOn;
    private GameObject[] players;
    public AudioClip musicaEnfado;
    public Sprite chibiDef, chibiAtt, chibiSan;
    public SpriteRenderer chibiSprite;
    void Start()
    {
        infoPanelIsOn = true;
        pnlYourCharacter.SetActive(true);
        print("Comienza el juego");
        numOfPlayers = PhotonNetwork.countOfPlayers; 
        whatCharacter = Random.Range(0, 3);
        whatSpawn = Random.Range(0, playerSpawns.Count);
        string playerToSpawn = characters[whatCharacter].name;
        Vector3 inWhatSpawn = playerSpawns[whatSpawn].transform.position;
        print(inWhatSpawn);
        PhotonNetwork.Instantiate(playerToSpawn, inWhatSpawn, Quaternion.identity, 0);
        print("jugador instanciado");
        if (whatCharacter == 0)
        {
            srYourCharacter.sprite = sDef;
            txtYourCharacter.text = "You are a defense Nyamurai!";
            chibiSprite.sprite = chibiDef;
        }
        else if (whatCharacter == 1)
        {
            srYourCharacter.sprite = sAtt;
            txtYourCharacter.text = "You are a attack Nyamurai!";
            chibiSprite.sprite = chibiAtt;
        }
        else if (whatCharacter == 2)
        {
            srYourCharacter.sprite = sSan;
            txtYourCharacter.text = "You are a healing Nyamurai!";
            chibiSprite.sprite = chibiSan;
        }
    }

    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length >= PhotonNetwork.playerList.Length)
        {
            yourCharTimmer += Time.deltaTime;
            if (yourCharTimmer > 3)
            {
                infoPanelIsOn = false;
                pnlYourCharacter.SetActive(false);
            }
        }
        txtPing.text = "Ping: " + PhotonNetwork.GetPing();
        txtPlayers.text = "Players: " + PhotonNetwork.playerList.Length;
        if (players.Length <= 0)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("DeadScene");
        }
        
    }
    public void ChangeMusic()
    {
        gameObject.GetComponent<AudioSource>().clip = musicaEnfado;
        gameObject.GetComponent<AudioSource>().Play();
    }
}
