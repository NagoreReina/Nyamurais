using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Photon.MonoBehaviour
{
    //Estadísitcas del player
    private int life;
    public float speed;
    public float jumpForce, jumpHeight;
    private GameObject imgLife1, imgLife2, imgLife3;

    //que personaje te ha tocado
    public int whatCharacter;
    public GameController controller;

    //para restarle vida al enemigo
    public GameObject enemy;

    public float horizontalInput;
    private float halfPlayerSizeX;

    public GameObject checkIfFloor, especialAttack, defautChar;
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer visualCharacter;

    public Sprite sDef, sAtt, sSan;
    public Transform spritePositionScale;

    //De los ataques
    private bool canAttack, attackEsp, executeAttack, canReciveDamage, damageRecently, makingAttack;
    public GameObject imgBonus, txtUnsername;
    private float especialAttackTimmer = 0f, makeSphereBiggerTimmer = 0f, bonusTimmer, scaleSphereX, scaleSphereY, inmunityTimmer = 0f, blinkTimmer = 0f;
    public Vector2 sphereBasicSize;
    private int attackPower;

    //multijugador
    public PhotonView photonViewP;
    public Text txtUsername;

    private Vector3 defPos = new Vector3(0.038f, 0.0556f, 1);
    private Vector3 attPos = new Vector3(0.0589f, 0.0405f, 1);
    private Vector3 sanPos = new Vector3(0.038f, 0.0556f, 1);
    private Vector3 defScale = new Vector3(0.355892f, 0.1781093f, 0.3558928f);
    private Vector3 attScale = new Vector3(0.5032316f, 0.2518461f, 1.413998f);
    private Vector3 sanScale = new Vector3(0.4681058f, 0.2342671f, 0.4681058f);

    private void Awake()
    {
        if (photonViewP.isMine)
        {
            txtUnsername.GetComponent<TextMeshPro>().text = PhotonNetwork.playerName;
        }
        else
        {
            txtUnsername.GetComponent<TextMeshPro>().text = photonViewP.owner.NickName;
            txtUnsername.GetComponent<TextMeshPro>().color = Color.cyan;
        }
    }

    void Start()
    {
        try
        {
            checkIfFloor = transform.GetChild(0).gameObject;
            rb = GetComponent<Rigidbody2D>();
            controller = FindObjectOfType<GameController>();
            enemy = FindObjectOfType<CthulhuController>().gameObject;
            whatCharacter = controller.whatCharacter;
           
            imgBonus = GameObject.Find("ImgEspecialAttack");
            imgBonus.SetActive(true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            throw;
        }

        halfPlayerSizeX = GetComponent<MeshRenderer>().bounds.size.x / 2;
        canAttack = true;
        attackEsp = true;
        executeAttack = false;
        sphereBasicSize = especialAttack.transform.localScale;
        attackPower = 1;
        canReciveDamage = true;
        life = 3;
        imgLife1 = GameObject.Find("imgLife1");
        imgLife2 = GameObject.Find("imgLife2");
        imgLife3 = GameObject.Find("imgLife3");
        damageRecently = false;
    }

    void Update()
    {
        if (!controller.infoPanelIsOn)
        {
            if (photonViewP.isMine)
            {
                CheckInput();
            }

            //limites cámara
            clampPlayersMovement();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

    }
    private void CheckInput()
    {
        //Movimiento
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);

        #region Animación y Activar ataque normal
        //Cambios de la animación
        if (anim.GetBool("attack"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            {
                anim.SetBool("attack", false);
                makingAttack = false;
            }
            else
            {
                makingAttack = true;
            }
        }
        if (horizontalInput != 0)
        {
            if (!makingAttack && checkIfFloor.GetComponent<CheckIfGround>().isGrounded)
            {
                anim.SetBool("run", true);
            }
            canAttack = false;
        }
        else
        {
            if (!makingAttack)
            {
                anim.SetBool("run", false);
            }
            canAttack = true;
        }
        if (horizontalInput > 0)
        {
            photonViewP.RPC("FlipTrue", PhotonTargets.AllBuffered);
        }
        else
        {
            photonViewP.RPC("FlipFalse", PhotonTargets.AllBuffered);
        }
        #endregion

        //salto
        if (Input.GetButtonDown("Jump") && checkIfFloor.GetComponent<CheckIfGround>().isGrounded)
        {
            Jump();
        }
        if (checkIfFloor.GetComponent<CheckIfGround>().isGrounded)
        {
            anim.SetBool("jump", false);
        }
        else
        {
            if (!makingAttack)
            {
                anim.SetBool("run", false);
                anim.SetBool("jump", true);
            }
        }

        //ATAQUES
        if (Input.GetButtonDown("Fire1") && canAttack && !enemy.GetComponent<CthulhuController>().isProtected)
        {
            //enemyController.life -= attackPower;
            enemy.GetComponent<CthulhuController>().CallReduceHealth(attackPower);
            anim.SetBool("run", false);
            anim.SetBool("jump", false);
            anim.SetBool("attack", true);
            anim.SetBool("especial", false);
        }
        if (Input.GetButtonDown("Fire2") && canAttack && attackEsp)
        {
            imgBonus.SetActive(false);
            photonViewP.RPC("ActiveEspecialAttack", PhotonTargets.AllBuffered);
            executeAttack = true;
            anim.SetBool("run", false);
            anim.SetBool("jump", false);
            anim.SetBool("attack", false);
            anim.SetBool("especial", true);
            attackEsp = false;
        }
        //Ejecutar el ataque especial
        if (executeAttack)
        {
            anim.SetBool("especial", true);
            makeSphereBiggerTimmer += Time.deltaTime;
            scaleSphereX += 0.01f;
            scaleSphereY += 0.005f;
            especialAttack.transform.localScale = new Vector2(scaleSphereX, scaleSphereY);
            if (makeSphereBiggerTimmer >= 5)
            {
                scaleSphereX = 0;
                scaleSphereY = 0;
                especialAttack.transform.localScale = sphereBasicSize;
                photonViewP.RPC("DesactiveEspecialAttack", PhotonTargets.AllBuffered);
                makeSphereBiggerTimmer = 0;
                executeAttack = false;
                anim.SetBool("especial", false);
            }
        }
        //bonus de ataque activado
        if (attackPower == 2)
        {
            bonusTimmer += Time.deltaTime;
            if (bonusTimmer > 3)
            {
                bonusTimmer = 0;
                attackPower = 1;
            }
        }
        //Sistema de vidas
        if (life == 3 && (!imgLife2.activeSelf || !imgLife1.activeSelf))
        {
            imgLife1.SetActive(true);
            imgLife2.SetActive(true);
            imgLife3.SetActive(true);
        }
        else if (life == 2)
        {
            imgLife1.SetActive(false);
        }
        else if (life == 1)
        {
            imgLife2.SetActive(false);
        }
        else if (life <= 0)
        {
            imgLife3.SetActive(false);
            print("El jugador ha sido destruido");

            photonViewP.RPC("DestroyObject", PhotonTargets.AllBuffered);
        }
        //inmunidad por haber recibido daño recientemente
        if (damageRecently)
        {
            inmunityTimmer += Time.deltaTime;
            blinkTimmer += Time.deltaTime;
            if (blinkTimmer < 0.2f)
            {
                photonViewP.RPC("BlipOneOnDamage", PhotonTargets.AllBuffered);
            }
            else if (blinkTimmer < 0.4f)
            {
                photonViewP.RPC("BlipTwoOnDamage", PhotonTargets.AllBuffered);
            }
            else
            {
                blinkTimmer = 0;
            }


            if (inmunityTimmer > 3)
            {
                inmunityTimmer = 0;
                damageRecently = false;
                photonViewP.RPC("SetNormalColor", PhotonTargets.AllBuffered);
            }
        }

        if (attackEsp)
            imgBonus.SetActive(true);
        else
        {
            especialAttackTimmer += Time.deltaTime;
            if (especialAttackTimmer > 30)
            {
                especialAttackTimmer = 0;
                attackEsp = true;
            }
        }
    }
    //limites cámara
    void clampPlayersMovement()
    {
        Vector3 position = transform.position;

        float distance = transform.position.z - Camera.main.transform.position.z;

        float leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance)).x + halfPlayerSizeX;
        float rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance)).x - halfPlayerSizeX;


        position.x = Mathf.Clamp(position.x, leftBorder, rightBorder);

        transform.position = position;
    }

    public void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
    [PunRPC]
    private void BlipOneOnDamage()
    {
        try
        {
            visualCharacter.color = new Color(visualCharacter.color.r, visualCharacter.color.g, visualCharacter.color.b, 0.2f);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("BlipOneOnDamage " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void BlipTwoOnDamage()
    {
        try
        {
            visualCharacter.color = new Color(visualCharacter.color.r, visualCharacter.color.g, visualCharacter.color.b, 1f);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("BlipTwoOnDamage " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void SetNormalColor()
    {
        try
        {
            visualCharacter.color = Color.white;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SetNormalColor " + ex);
            throw ex;
        }

    }

    [PunRPC]
    private void FlipTrue()
    {
        try
        {
            visualCharacter.flipX = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("FlipTrue " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void FlipFalse()
    {
        try
        {
            visualCharacter.flipX = false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("FlipFalse " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void ActiveEspecialAttack()
    {
        try
        {
            especialAttack.SetActive(true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("ActiveEspecialAttack " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void DesactiveEspecialAttack()
    {
        try
        {
            especialAttack.SetActive(false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("DesactiveEspecialAttack " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void AttCharacterSelected()
    {
        try
        {
            especialAttack.GetComponent<SpriteRenderer>().color = Color.red;
            especialAttack.tag = "BonusAttack";
            visualCharacter.sprite = sAtt;
            anim.SetBool("isDef", false);
            anim.SetBool("isAtt", true);
            anim.SetBool("isSan", false);
            visualCharacter.transform.localPosition = attPos;
            visualCharacter.transform.localScale = attScale;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("AttCharacterSelected " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void DefCharacterSelected()
    {
        try
        {
            especialAttack.GetComponent<SpriteRenderer>().color = Color.cyan;
            especialAttack.tag = "BonusDeffense";
            visualCharacter.sprite = sDef;
            anim.SetBool("isDef", true);
            anim.SetBool("isAtt", false);
            anim.SetBool("isSan", false);
            visualCharacter.transform.localPosition = defPos;
            visualCharacter.transform.localScale = defScale;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("DefCharacterSelected " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void SanCharacterSelected()
    {
        try
        {
            especialAttack.GetComponent<SpriteRenderer>().color = Color.green;
            especialAttack.tag = "BonusLife";
            visualCharacter.sprite = sSan;
            anim.SetBool("isDef", false);
            anim.SetBool("isAtt", false);
            anim.SetBool("isSan", true);
            visualCharacter.transform.localPosition = sanPos;
            visualCharacter.transform.localScale = sanScale;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SanCharacterSelected " + ex);
            throw ex;
        }

    }
    [PunRPC]
    private void DestroyObject()
    {
        try
        {
            Destroy(gameObject);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("DestroyObject " + ex);
            throw ex;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BonusAttack")
        {
            attackPower = 2;
        }
        if (collision.tag == "BonusLife")
        {
            life = 3;
        }
        if (collision.tag == "BonusDeffense")
        {
            canReciveDamage = false;
        }
        if (collision.tag == "EnemyAttack" && canReciveDamage && !damageRecently)
        {
            life--;
            damageRecently = true;

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "BonusDeffense")
        {
            canReciveDamage = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "BonusDeffense")
        {
            canReciveDamage = true;
        }
    }
}
