using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CthulhuController : MonoBehaviour
{
    //public float life;
    //private Slider sLife;
    private GameController controller;
    private EnemyHealth healthController;
    private bool ultimateAttackPhase;
    public GameObject tentacleAttack, rayAttack, waveAttack, eyeAttack, laserAttack;
    public List<Transform> SpawnUp;
    public List<Transform> SpawnDown;
    public List<Transform> SpawnLeft;
    public List<Transform> SpawnRight;
    public List<Transform> FinishTentaclePoints;
    public List<Transform> WaveAttackSpawn;
    public List<Transform> FinishWavePoints;
    public float maxTimeNextAttack, maxTimeNextAttackSecondFase;
    public float nextAttackTimmer, isProtectedTimmer;
    public bool isProtected;
    public bool makingAttackAnim, doTheAttack;
    public Animator anim;
    private bool initiateGame;
    private int whatAttack;
    private string currentAnimation;
    public PhotonView cPhotonView;
    private int selectedSpawn;
    int[] tentacleSpawns;
    List<int> choosenSpawnsLasersUp = new List<int>();
    List<int> choosenSpawnsLasersDown = new List<int>();
    List<int> choosenSpawnsLasersLeft = new List<int>();
    List<int> choosenSpawnsLasersRight = new List<int>();
    int selectedSpawnLaser = 0;
    int randomSpawn = 0;
    int finishSpawnDir = 0;
    Transform[] laserInitialSpawns;
    List<Transform> laserInitialSpawnsList = new List<Transform>();
    Transform[] laserFinalSpawns;
    List<Transform> laserFinalSpawnsList = new List<Transform>();

    void Start()
    {
        try
        {
            ultimateAttackPhase = false;
            //sLife = FindObjectOfType<Slider>();
            //if (sLife == null)
            //    return;
            controller = FindObjectOfType<GameController>();
            if (controller == null)
                return;
            healthController = FindObjectOfType<EnemyHealth>();
            if (healthController == null)
                return;
            print(controller.numOfPlayers);
            //sLife.maxValue = controller.numOfPlayers * 500; //La vida del enemigo depende del numero de jugadores
            //sLife.value = sLife.maxValue;
            //life = sLife.value;
            isProtected = false;
            isProtectedTimmer = 0;
            anim.SetBool("playGame", true);
            initiateGame = false;
            makingAttackAnim = false;
            doTheAttack = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("START: " + ex);
            throw ex;
        }

    }
    void Update()
    {
        if (!controller.infoPanelIsOn)
        {
            try
            {
                print("-------------------COMIENZA EL JUEGO-------------------------------");
                anim.speed = 1;
                if (anim.GetBool("playGame") || anim.GetBool("secondPhase"))
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                    {
                        initiateGame = true;
                        anim.SetBool("playGame", false);
                        anim.SetBool("secondPhase", false);
                        anim.SetBool("iddle", true);
                        isProtected = false;
                        isProtectedTimmer = 0;
                    }
                    else
                    {
                        if (true)
                        {
                            nextAttackTimmer = 0;
                            isProtected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Comienzo o segunda fase: " + ex);
                throw ex;
            }

            if (initiateGame)
            {
                print("-------------------TERMINÓ LA ANIMACIÓN DE SALIR DEL AGUA-------------------------------");
                //Sistema de vida
                try
                {
                    AnimatorClipInfo[] animinfo = anim.GetCurrentAnimatorClipInfo(0);
                    currentAnimation = animinfo[0].clip.name;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Animator clip info: " + ex);
                    throw;
                }
                try
                {
                    if (healthController.life <= 0 && !ultimateAttackPhase)
                    {
                        healthController.life = healthController.sLife.maxValue;
                        ultimateAttackPhase = true;
                        controller.ChangeMusic();
                        anim.SetBool("secondPhase", true);
                        initiateGame = false;
                    }
                    else if (healthController.life <= 0 && ultimateAttackPhase)
                    {
                        anim.SetBool("dead", true);
                        anim.speed = 1;
                        print("GAME OVER");
                        nextAttackTimmer = 0;
                        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && currentAnimation == "CuthuluDead")
                        {
                            PhotonNetwork.LeaveRoom();
                            PhotonNetwork.LoadLevel("VictoryScene");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Controles de vida: " + ex);
                    throw;
                }


                #region Ataques
                nextAttackTimmer += Time.deltaTime;
                float endAttackTimmer = 0;

                if (ultimateAttackPhase)
                    endAttackTimmer = maxTimeNextAttackSecondFase;
                else
                    endAttackTimmer = maxTimeNextAttack;

                try
                {
                    if (anim.GetBool("iddle") && ultimateAttackPhase)
                    {
                        anim.speed = 2;
                    }
                    else
                    {
                        anim.speed = 1;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Animation speed: " + ex);
                    throw;
                }

                if (makingAttackAnim)
                {
                    try
                    {
                        print("-------------------SE ACTIVA LA ANIMACIÓN DE ATAQUE-------------------------------");
                        anim.SetBool("otherAttack", true);
                        anim.SetBool("iddle", false);
                        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && currentAnimation != "IddleAnim")
                        {
                            print("-------------------SE TERMINA LA ANIMACIÓN DE ATAQUE-------------------------------");
                            anim.SetBool("otherAttack", false);
                            anim.SetBool("iddle", true);
                            makingAttackAnim = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Animación de ataque activada: " + ex);
                        throw;
                    }

                }
                if (nextAttackTimmer >= endAttackTimmer)
                {
                    print("-------------------TIEMPO DE UN NUEVO ATAQUE-------------------------------");
                    if (doTheAttack == true)
                    {
                        try
                        {
                            if (PhotonNetwork.isMasterClient)
                            {
                                whatAttack = Random.Range(0, 6);
                                if (whatAttack == 4) //---------------------EVITAR ATAQUE LASER PARA NO DAR FALLO
                                {
                                    whatAttack = 0;
                                }
                                cPhotonView.RPC("SelectedAttack", PhotonTargets.AllBuffered, whatAttack);
                            }
                            if (whatAttack != 5)
                            {
                                print("-------------------NO ES ATAQUE DE PROTECCIÓN-------------------------------");
                                makingAttackAnim = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("MASTER elige ataque: " + ex);
                            throw;
                        }

                    }
                }
                if (!makingAttackAnim && !doTheAttack)
                {
                    print("-------------------SE HACE EL ATAQUE-------------------------------");
                    switch (whatAttack)
                    {
                        #region tentacleAttack
                        case 0:
                            try
                            {
                                print("-------------------TENTACULOS-------------------------------");
                                List<int> choosenSpawns = new List<int>();
                                int[] arraySpawns = new int[] { };
                                if (PhotonNetwork.isMasterClient)
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        selectedSpawn = Random.Range(0, SpawnDown.Count);
                                        if (!choosenSpawns.Contains(selectedSpawn))
                                        {
                                            choosenSpawns.Add(selectedSpawn);
                                        }
                                        else
                                        {
                                            i--;
                                        }
                                    }
                                    arraySpawns = choosenSpawns.ToArray();
                                }
                                cPhotonView.RPC("TentacleAttack", PhotonTargets.AllBuffered, arraySpawns);
                                foreach (var spawn in tentacleSpawns)
                                {
                                    GameObject tentacle = PhotonNetwork.Instantiate(tentacleAttack.name, SpawnDown[spawn].transform.position, Quaternion.identity, 0);
                                    tentacle.GetComponent<CthulhuTentacleAttack>().spawn = SpawnDown[spawn];
                                    tentacle.GetComponent<CthulhuTentacleAttack>().finishSpawn = FinishTentaclePoints[spawn];
                                }
                                Array.Clear(tentacleSpawns, 0, tentacleSpawns.Length);
                                doTheAttack = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("Tentaculos: " + ex);
                                throw;
                            }
                            break;

                        #endregion

                        #region RayAttack
                        case 1:
                            try
                            {
                                print("-------------------RAYOS-------------------------------");
                                int selectSide = Random.Range(0, 2);
                                if (selectSide == 0)
                                {
                                    for (int i = 0; i < 8; i++)
                                    {
                                        GameObject newRayAttack = Instantiate(rayAttack, SpawnUp[i]);
                                        Destroy(newRayAttack, 2);
                                    }
                                }
                                else if (selectSide == 1)
                                {
                                    for (int i = SpawnUp.Count - 1; i > 8; i--)
                                    {
                                        GameObject newRayAttack = Instantiate(rayAttack, SpawnUp[i]);
                                        Destroy(newRayAttack, 2);
                                    }
                                }
                                doTheAttack = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("RAYOS: " + ex);
                                throw;
                            }
                                break;
                            
                        #endregion

                        #region WaveAttack

                        case 2:
                            try
                            {
                                print("-------------------OLAS-------------------------------");
                                for (int i = 0; i < 2; i++)
                                {
                                    GameObject wave = Instantiate(waveAttack, WaveAttackSpawn[i]);
                                    wave.GetComponent<CthulhuWaveAttack>().spawn = WaveAttackSpawn[i];
                                    wave.GetComponent<CthulhuWaveAttack>().finishSpawn = FinishWavePoints[i];
                                    if (i == 1)
                                    {
                                        wave.transform.localScale = new Vector2(-wave.transform.localScale.x, wave.transform.localScale.y);
                                    }
                                }
                                doTheAttack = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("Wave: " + ex);
                                throw;
                            }
                            
                            break;
                        #endregion

                        #region EyeAttack
                        case 3:
                            try
                            {
                                print("-------------------OJOS-------------------------------");
                                List<int> choosenSpawnsEye = new List<int>();
                                for (int i = 0; i < 8; i++)
                                {
                                    int randomDirection = Random.Range(0, 2);
                                    if (randomDirection == 0)
                                    {
                                        int selectedSpawnEye = Random.Range(0, SpawnLeft.Count);
                                        if (!choosenSpawnsEye.Contains(selectedSpawnEye))
                                        {
                                            choosenSpawnsEye.Add(selectedSpawnEye);
                                            GameObject EyeMonster = Instantiate(eyeAttack, SpawnLeft[selectedSpawnEye]);
                                        }
                                    }
                                    else if (randomDirection == 1)
                                    {
                                        int selectedSpawnEye = Random.Range(0, SpawnRight.Count);
                                        if (!choosenSpawnsEye.Contains(selectedSpawnEye))
                                        {
                                            choosenSpawnsEye.Add(selectedSpawnEye);
                                            GameObject EyeMonster = Instantiate(eyeAttack, SpawnRight[selectedSpawnEye]);
                                            EyeMonster.gameObject.transform.Find("ojoCthulhu").GetComponent<SpriteRenderer>().flipX = true;
                                            EyeMonster.GetComponent<CthulhuEyeAttack>().isRight = true;
                                        }
                                    }
                                }
                                doTheAttack = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("OJOS: " + ex);
                                throw;
                            }
                            
                            break;
                        #endregion

                        #region LaserAttack
                        case 4: //Laser Attack
                            try
                            {
                                print("-------------------LASER-------------------------------");
                                for (int i = 0; i < 20; i++)
                                {
                                    int selectedSpawn;
                                    int randomSpawn = Random.Range(0, 4);
                                    int finishSpawnDir = Random.Range(0, 4);
                                    Transform transfSpawn = null;
                                    Transform transfSpawnFinish = null;
                                    if (randomSpawn == 0)
                                    {
                                        selectedSpawn = Random.Range(0, SpawnUp.Count);
                                        if (!choosenSpawnsLasersUp.Contains(selectedSpawn))
                                        {
                                            choosenSpawnsLasersUp.Add(selectedSpawn);
                                            transfSpawn = SpawnUp[selectedSpawn];
                                        }
                                        else
                                        {
                                            i--;
                                        }
                                    }
                                    else if (randomSpawn == 1)
                                    {
                                        selectedSpawn = Random.Range(0, SpawnDown.Count);
                                        if (!choosenSpawnsLasersDown.Contains(selectedSpawn))
                                        {
                                            choosenSpawnsLasersDown.Add(selectedSpawn);
                                            transfSpawn = SpawnDown[selectedSpawn];
                                        }
                                        else
                                        {
                                            i--;
                                        }
                                    }
                                    else if (randomSpawn == 2)
                                    {
                                        selectedSpawn = Random.Range(0, SpawnLeft.Count);
                                        if (!choosenSpawnsLasersLeft.Contains(selectedSpawn))
                                        {
                                            choosenSpawnsLasersLeft.Add(selectedSpawn);
                                            transfSpawn = SpawnLeft[selectedSpawn];
                                        }
                                        else
                                        {
                                            i--;
                                        }
                                    }
                                    else if (randomSpawn == 3)
                                    {
                                        selectedSpawn = Random.Range(0, SpawnRight.Count);
                                        if (!choosenSpawnsLasersRight.Contains(selectedSpawn))
                                        {
                                            choosenSpawnsLasersRight.Add(selectedSpawn);
                                            transfSpawn = SpawnRight[selectedSpawn];
                                        }
                                        else
                                        {
                                            i--;
                                        }
                                    }
                                    if (finishSpawnDir == 0)
                                    {
                                        transfSpawnFinish = SpawnUp[finishSpawnDir];
                                    }
                                    else if (finishSpawnDir == 1)
                                    {
                                        transfSpawnFinish = SpawnDown[finishSpawnDir];
                                    }
                                    else if (finishSpawnDir == 2)
                                    {
                                        transfSpawnFinish = SpawnLeft[finishSpawnDir];
                                    }
                                    else if (finishSpawnDir == 3)
                                    {
                                        transfSpawnFinish = SpawnRight[finishSpawnDir];
                                    }

                                    if (transfSpawn)
                                    {
                                        GameObject laser = Instantiate(laserAttack, transfSpawn);
                                        laser.GetComponent<CthulhuLaserAttack>().spawn = transfSpawn;
                                        laser.GetComponent<CthulhuLaserAttack>().finishSpawn = transfSpawnFinish;
                                    }
                                }
                                //VERSION ONLINE FALLIDA
                                #region VERSION ONLINE FALLIDA
                                //if (PhotonNetwork.isMasterClient)
                                //{
                                //    for (int i = 0; i < 20; i++)
                                //    {
                                //        randomSpawn = Random.Range(0, 4);
                                //        finishSpawnDir = Random.Range(0, 4);

                                //        Transform transfSpawn = null;
                                //        Transform transfSpawnFinish = null;
                                //        if (randomSpawn == 0)
                                //        {
                                //            selectedSpawnLaser = Random.Range(0, SpawnUp.Count);
                                //            if (!choosenSpawnsLasersUp.Contains(selectedSpawnLaser))
                                //            {
                                //                choosenSpawnsLasersUp.Add(selectedSpawnLaser);
                                //                transfSpawn = SpawnUp[selectedSpawnLaser];
                                //            }
                                //            else
                                //            {
                                //                i--;
                                //            }
                                //        }
                                //        else if (randomSpawn == 1)
                                //        {
                                //            selectedSpawnLaser = Random.Range(0, SpawnDown.Count);
                                //            if (!choosenSpawnsLasersDown.Contains(selectedSpawnLaser))
                                //            {
                                //                choosenSpawnsLasersDown.Add(selectedSpawnLaser);
                                //                transfSpawn = SpawnDown[selectedSpawnLaser];
                                //            }
                                //            else
                                //            {
                                //                i--;
                                //            }
                                //        }
                                //        else if (randomSpawn == 2)
                                //        {
                                //            selectedSpawnLaser = Random.Range(0, SpawnLeft.Count);
                                //            if (!choosenSpawnsLasersLeft.Contains(selectedSpawnLaser))
                                //            {
                                //                choosenSpawnsLasersLeft.Add(selectedSpawnLaser);
                                //                transfSpawn = SpawnLeft[selectedSpawnLaser];
                                //            }
                                //            else
                                //            {
                                //                i--;
                                //            }
                                //        }
                                //        else if (randomSpawn == 3)
                                //        {
                                //            selectedSpawnLaser = Random.Range(0, SpawnRight.Count);
                                //            if (!choosenSpawnsLasersRight.Contains(selectedSpawnLaser))
                                //            {
                                //                choosenSpawnsLasersRight.Add(selectedSpawnLaser);
                                //                transfSpawn = SpawnRight[selectedSpawnLaser];
                                //            }
                                //            else
                                //            {
                                //                i--;
                                //            }
                                //        }
                                //        if (transfSpawn != null)
                                //        {
                                //            print("----SPAWN SELECCIONADO" + transfSpawn);
                                //            laserInitialSpawnsList.Add(transfSpawn);
                                //        }
                                //        if (finishSpawnDir == 0)
                                //        {
                                //            transfSpawnFinish = SpawnUp[finishSpawnDir];
                                //        }
                                //        else if (finishSpawnDir == 1)
                                //        {
                                //            transfSpawnFinish = SpawnDown[finishSpawnDir];
                                //        }
                                //        else if (finishSpawnDir == 2)
                                //        {
                                //            transfSpawnFinish = SpawnLeft[finishSpawnDir];
                                //        }
                                //        else if (finishSpawnDir == 3)
                                //        {
                                //            transfSpawnFinish = SpawnRight[finishSpawnDir];
                                //        }
                                //        if (transfSpawnFinish != null)
                                //        {
                                //            laserFinalSpawnsList.Add(transfSpawnFinish);
                                //        }
                                //    }
                                //    laserInitialSpawns = laserInitialSpawnsList.ToArray();
                                //    laserFinalSpawns = laserFinalSpawnsList.ToArray();
                                //    cPhotonView.RPC("SetLaserSpawns", PhotonTargets.AllBuffered, laserInitialSpawns, laserFinalSpawns);
                                //}
                                //if (laserInitialSpawns.Length > 0)
                                //{
                                //    for (int i = 0; i < laserInitialSpawns.Length; i++)
                                //    {
                                //        GameObject laser = PhotonNetwork.Instantiate(laserAttack.name, laserInitialSpawns[i].position, Quaternion.identity, 0);
                                //        laser.GetComponent<CthulhuLaserAttack>().spawn = laserInitialSpawns[i];
                                //        laser.GetComponent<CthulhuLaserAttack>().finishSpawn = laserFinalSpawns[i];
                                //    }
                                //}
                                //laserInitialSpawnsList.Clear();
                                //laserFinalSpawnsList.Clear();
                                //choosenSpawnsLasersUp.Clear();
                                //choosenSpawnsLasersDown.Clear();
                                //choosenSpawnsLasersLeft.Clear();
                                //choosenSpawnsLasersRight.Clear();
                                //Array.Clear(laserInitialSpawns, 0, laserInitialSpawns.Length);
                                //Array.Clear(laserFinalSpawns, 0, laserFinalSpawns.Length);
                                #endregion
                                doTheAttack = true;
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("Laser: " + ex);
                                throw;
                            }
                            
                            break;
                        #endregion

                        #region Protection
                        case 5: //Protection
                            try
                            {
                                print("-------------------PROTECCION-------------------------------");
                                isProtected = true;
                                anim.SetBool("protectAttack", true);
                                anim.SetBool("iddle", false);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError("Proteccion: " + ex);
                                throw;
                            }                          
                            break;
                            #endregion
                    }
                }
                #endregion
            }
            if (isProtected)
            {
                try
                {
                    print("-------------------SE ESTÁ PROTEGIENDO-------------------------------");
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && currentAnimation != "IddleAnim")
                    {
                        print("-------------------SE HA TERMINADO LA ANIMACIÓN DE PROTECCIÓN-------------------------------");
                        isProtected = false;
                        anim.SetBool("protectAttack", false);
                        anim.SetBool("iddle", true);
                        doTheAttack = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("SE HACE EL ATAQUE DE PROTECION: " + ex);
                    throw;
                }
                
            }
        }
        else
        {
            anim.speed = 0;
        }
    }
    [PunRPC]
    private void SelectedAttack(int attack)
    {
        whatAttack = attack;
        print("-------------------SE SELECCIONA EL ATAQUE-------------------------------");
        nextAttackTimmer = 0;
        doTheAttack = false;
    }
    [PunRPC]
    private void TentacleAttack(int[] allSpawns)
    {
        print("TentacleSpawns: " + allSpawns.Length);
        //print(allSpawns[0] + "," + allSpawns[1] + "," + allSpawns[2] + "," + allSpawns[3] + "," + allSpawns[4]);
        tentacleSpawns = allSpawns;
        //print(tentacleSpawns[0] + "," + tentacleSpawns[1] + "," + tentacleSpawns[2] + "," + tentacleSpawns[3] + "," + tentacleSpawns[4]);
    }
    [PunRPC]
    private void SetLaserSpawns(Transform[] first, Transform[] final)
    {
        laserInitialSpawns = first;
        laserFinalSpawns = final;
    }
    public void CallReduceHealth(float amount)
    {
        cPhotonView.RPC("ReduceHealth", PhotonTargets.AllBuffered, amount);
    }

    [PunRPC]
    public void ReduceHealth(float amount)
    {
        healthController.ModifyHealth(amount);
    }
}
