using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AU_PlayerController : MonoBehaviourPun, IPunObservable 
{
    [SerializeField] bool hasControl;
    public static AU_PlayerController localPlayer;
    
    //components
    Rigidbody myRB;
    Animator myAnim;
    Transform myAvatar;
    //player movement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;

    private float direction = 1;
    //player color
    //static Color myColor;
    [SerializeField] Color myColor;
    SpriteRenderer myAvatarSprite;
    //role
    [SerializeField] bool isImposter = false;
    [SerializeField] InputAction KILL;
    float killInput;
    //AU_PlayerController target;
    List<AU_PlayerController> targets;
    [SerializeField] Collider myCollider;

    bool isDead;

    [SerializeField] GameObject bodyPrefab;

    public static List<Transform> allBodies;

    List<Transform> bodiesFound;

    [SerializeField] InputAction REPORT;
    [SerializeField] LayerMask ignoreForBody;
    
    //networking
    private PhotonView myPv;
    private Camera myCamera;
    private AudioListener myAudio;
    [SerializeField] private GameObject lightMask;
    
    //vent system
    VentsSystem ventsSystem;
    private bool canMove = true;
    
    //play music when walking
    private bool isMoving = false;
    private PlayMusicController playMusicController;
    
    //hide btn kill and vent from other players
    private Button btnKill;
    private Button btnVent; 
    
    //cooldown kill
    [SerializeField] float cooldown = 20;
    private bool canTouchKill = true;
    private Text btnCooldownTxt;

    //hide button report and minimap when using chat
    private Button btnReport;
    private Button btnMinimap;

    //inactive player when using chat
    [HideInInspector] public UIControl _uiControl;
    
    //show and hide eye's imposter
    public SpriteRenderer eyeSprite;
    
    //random player color body 
    public PlayerInfo _playerInfo;
    
    //hide btn sabotage from other players
    private Button btnSabotage;
    
    //interaction object
    [SerializeField] private InputAction MOUSE;
    private Vector2 mousePositionInput;
    [SerializeField] private InputAction INTERACTION;
    [SerializeField] private LayerMask interactLayer;
    
    //hide button use from imposter
    private Button btnUse;
    
    //hide name imposter from vent
    [SerializeField] private Text hideNameFromVent;

    //cooldown kill
    IEnumerator ResetKill()
    {
        canTouchKill = false;
        float TimeLeft = cooldown;
        while (TimeLeft != 0)
        {
            yield return new WaitForSeconds(1);
            TimeLeft--;
            btnCooldownTxt.text = TimeLeft.ToString();
        }
        canTouchKill = true;
        btnCooldownTxt.text = "";
    }

    private void Awake()
    {
        KILL.performed += KillTarget;
        REPORT.performed += ReportBody;
        INTERACTION.performed += Interact;
    }

    private void OnEnable()
    {
        WASD.Enable();
        KILL.Enable();
        REPORT.Enable();
        MOUSE.Enable();
        INTERACTION.Enable();
    }

    private void OnDisable()
    {
        WASD.Disable();
        KILL.Disable();
        REPORT.Disable();
        MOUSE.Disable();
        INTERACTION.Disable();
    }

    private void Start()
    {
        myPv = GetComponent<PhotonView>();
        if (myPv.IsMine) localPlayer = this;
        myCamera = transform.GetChild(1).GetComponent<Camera>();
        myAudio = transform.GetChild(1).GetComponent<AudioListener>();
        
        targets = new List<AU_PlayerController>();
        myRB = GetComponent<Rigidbody>();
        myAvatar = transform.GetChild(0);
        myAnim = GetComponent<Animator>();
        myAvatarSprite = myAvatar.GetComponent<SpriteRenderer>();
        
        if (!myPv.IsMine)
        {
            myCamera.gameObject.SetActive(false);
            myAudio.gameObject.SetActive(false);
            lightMask.SetActive(false);
            return;
        }
        /*change color local player body color, no use with multiplayer random color body
        if (myColor == Color.clear)
            myColor = Color.white;
        myAvatarSprite.color = myColor;
        */
        if (allBodies == null)
        {
            allBodies = new List<Transform>();
        }

        bodiesFound = new List<Transform>();
        //music walking
        playMusicController = GetComponent<PlayMusicController>();
        //hide btn kill and vent from other players
        btnKill = GameObject.Find("kill_button").GetComponent<Button>();
        btnVent = GameObject.Find("VentUIButton").GetComponent<Button>(); 
        //cooldown text
        btnCooldownTxt = GameObject.Find("CooldownText").GetComponent<Text>();
        btnCooldownTxt.text = "";
        //hide button report and minimap when using chat
        btnReport = GameObject.Find("report_button").GetComponent<Button>();
        btnMinimap = GameObject.Find("minimapBtn").GetComponent<Button>();
        //hide btn sabotage from other players
        btnSabotage = GameObject.Find("SabotageBtn").GetComponent<Button>();
        //hide button use from impsoter
        btnUse = GameObject.Find("UseBtn").GetComponent<Button>();
        btnUse.interactable = false;
    }

    //vent system
    public void EnterVent(VentsSystem ventsSystem)
    {
        this.ventsSystem = ventsSystem;
        myAnim.SetTrigger("Vent");
        playMusicController.StopWalking();
        playMusicController.PlayVent();
    }
    
    public void VentEntered()
    {
        myPv.RPC("DisablePlayer", RpcTarget.All);
        playMusicController.StopWalking();
        ventsSystem.PlayerInVent();
    }
    
    public bool IsInVent()
    {
        if (!myPv.IsMine) return false; //sync vent of specific imposter
        if (isImposter == false) return false; //disable vent interaction to other player
        return myRB.detectCollisions;
    }

    public void VentExited()
    {
        myPv.RPC("EnablePlayer", RpcTarget.All);
        playMusicController.PlayVent();
    }
    
    public void MovePlayer()
    {
        canMove = true;
    }
    
    public void StopPlayer()
    {
        canMove = false;
    }
    
    [PunRPC]
    void DisablePlayer()
    {
        //hide name imposter from vent
        hideNameFromVent.enabled = false;
        //hide body and eye of imposter from vent
        _playerInfo.playerBodyColor.enabled = false;
        _playerInfo.playerEyeColor.enabled = false;
        /*hide eye and body color of imposter from vent
        Color cc = eyeSprite.color;
        cc.a = 0;
        eyeSprite.color = cc;
        Color c = myAvatarSprite.color;
        c.a = 0;
        myAvatarSprite.color = c;
        */
        myRB.detectCollisions = false;
        canMove = false;
    }
    
    [PunRPC]
    void EnablePlayer()
    {
        //show name imposter in vent
        hideNameFromVent.enabled = true;
        //show body and eye of imposter in vent
        _playerInfo.playerBodyColor.enabled = true;
        _playerInfo.playerEyeColor.enabled = true;
        /*show eye and body color of imposter in vent
        Color cc = eyeSprite.color;
        cc.a = 1;
        eyeSprite.color = cc;
        Color c = myAvatarSprite.color;
        c.a = 1;
        myAvatarSprite.color = c;
        */
        myRB.detectCollisions = true;
        canMove = true;
    }
    //vent system

    private void Update()
    {
        myAvatar.localScale = new Vector2(direction, 1);
        if (!myPv.IsMine) return;
        //use when control facing by mouse
        //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //if(mousePos.x < transform.position.x && facingRight) flip();
        //else if(mousePos.x > transform.position.x && !facingRight) flip();
        movementInput = WASD.ReadValue<Vector2>();
        myAnim.SetFloat("Speed", movementInput.magnitude);
        if (movementInput.x != 0)
        {
            //myAvatar.localScale = new Vector2(Mathf.Sign(movementInput.x), 1);
            direction = Mathf.Sign(movementInput.x);
        }

        if (allBodies.Count > 0)
        {
            BodySearch();
        }
        //play music when walking
        if (movementInput.x != 0 || movementInput.y != 0) isMoving = true;
        else isMoving = false;
        if(isMoving == true) playMusicController.PlayWalkingSound();
        else playMusicController.StopWalking();
        if(canMove == false) playMusicController.StopWalking();
        //hide btn kill and vent from other player
        btnKill.gameObject.SetActive(isImposter); 
        btnVent.gameObject.SetActive(isImposter);
        //disable btn kill of raycast block
        if (canTouchKill)
        {
            btnKill.interactable = true;
            KILL.Enable();
        }
        else
        {
            btnKill.interactable = false;
            KILL.Disable();
        }
        //inactive player when using chat
        if (_uiControl.isChatWindowActive())
        {
            OnDisable();
            btnKill.interactable = false;
            btnVent.interactable = false;
            btnReport.interactable = false;
            btnMinimap.interactable = false;
            btnSabotage.interactable = false;
        }
        else
        {
            OnEnable();
            btnKill.interactable = true;
            btnReport.interactable = true;
            btnMinimap.interactable = true;
            btnSabotage.interactable = true;
            if(!canTouchKill) KILL.Disable(); //prevent kill from raycast block when chat is inactive
        }
        //disable report if player was killed by imposter
        btnReport.gameObject.SetActive(!isDead);
        //hide btn sabotage from other players
        btnSabotage.gameObject.SetActive(isImposter);
        //interact object
        mousePositionInput = MOUSE.ReadValue<Vector2>();
        //hide btn use from imposter
        btnUse.gameObject.SetActive(!isImposter);
    }

    private void FixedUpdate()
    {
        if (!myPv.IsMine) return;
        if(canMove) myRB.velocity = movementInput * movementSpeed;
        else myRB.velocity = Vector3.zero; //prevent imposter from walking and moving to vent 
    }

    public void SetColor(Color newColor)
    {
        myColor = newColor;
        if (myAvatarSprite != null)
        {
            myAvatarSprite.color = myColor;
        }
    }

    public void SetRole(bool newRole)
    {
        isImposter = newRole;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (isImposter == true)
            {
                if (tempTarget.isImposter == true) return;
                else
                {
                    targets.Add(tempTarget);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            AU_PlayerController tempTarget = other.GetComponent<AU_PlayerController>();
            if (targets.Contains(tempTarget))
            {
                targets.Remove(tempTarget);
            }
        }
    }
    
    void KillTarget(InputAction.CallbackContext context)
    {
        if (!myPv.IsMine) return;
        if (!isImposter) return;
        if (context.phase == InputActionPhase.Performed)
        {
            if (targets.Count == 0) return;
            else
            {
                if (targets[targets.Count - 1].isDead) return;
                transform.position = targets[targets.Count - 1].transform.position;
                targets[targets.Count - 1].myPv.RPC("Die", RpcTarget.All);
                targets.RemoveAt(targets.Count - 1);
                playMusicController.PlayKillSound(); //music kill
                StartCoroutine(ResetKill()); //cooldown kill
            }
        }
        /* old idea need fix
        if (isImposter == true)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Physics.IgnoreLayerCollision(11, 11, false);
                //float dist = Vector3.Distance(transform.position, bodyPrefab.transform.position);
                //if (dist < 2f && isImposter)
                //{
                    //Physics.IgnoreLayerCollision(11, 11, false);
                //} 
                if (targets.Count == 0) return;
                else
                {
                    if (targets[targets.Count - 1].isDead) return;
                    transform.position = targets[targets.Count - 1].transform.position;
                    targets[targets.Count - 1].Die();
                    targets.RemoveAt(targets.Count - 1);
                }
            }
        }
        if(isImposter == false && isDead == true) myPv.RPC("Die", RpcTarget.All);
        */
    }

    [PunRPC]
    public void Die()
    {
        //if (!myPv.IsMine) return; 
        AU_Body tempBody = PhotonNetwork.Instantiate("AU_Body", transform.position, transform.rotation).GetComponent<AU_Body>();
        //AU_Body tempBody = Instantiate(bodyPrefab, transform.position, transform.rotation).GetComponent<AU_Body>();
        //tempBody.SetColor(myAvatarSprite.color);
        tempBody.SetColor(_playerInfo.playerBodyColor.color); //set color follow body color
        isDead = true;
        myAnim.SetBool("isDead", isDead);
        gameObject.layer = 9;
        myCollider.enabled = false;
    }

    void BodySearch()
    {
        foreach (Transform body in allBodies)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, body.position - transform.position);
            Debug.DrawRay(transform.position, body.position - transform.position, Color.cyan);
            if (Physics.Raycast(ray, out hit, 1000f, ~ignoreForBody))
            {
                if (hit.transform == body)
                {
                    Debug.Log(hit.transform.name);
                    Debug.Log(bodiesFound.Count);
                    if (bodiesFound.Contains(body.transform)) return;
                    bodiesFound.Add(body.transform);
                }
                else
                {
                    bodiesFound.Remove(body.transform);
                }
            }
        }
    }

    private void ReportBody(InputAction.CallbackContext obj)
    {
        if (bodiesFound == null) return;
        if (bodiesFound.Count == 0) return;
        Transform tempBody = bodiesFound[bodiesFound.Count - 1];
        allBodies.Remove(tempBody);
        bodiesFound.Remove(tempBody);
        tempBody.GetComponent<AU_Body>().Report();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
        }
        else
        {
            direction = (float) stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void SetImposter()
    {
        isImposter = true;
        gameObject.layer = 11; //change layer to disable highlight of imposter
    }

    void Interact(InputAction.CallbackContext context)
    {
        if (isImposter) return; //prevent imposter from playing mini game
        if (context.phase == InputActionPhase.Performed)
        {
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay(mousePositionInput);
            if (Physics.Raycast(ray, out hit, interactLayer))
            {
                if (hit.transform.tag == "Interactable")
                {
                    if (!hit.transform.GetChild(0).gameObject.activeInHierarchy) return;
                    AU_Interactable temp = hit.transform.GetComponent<AU_Interactable>();
                    temp.PlayMiniGame();
                }
            }
        }
    }
}