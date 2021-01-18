using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement_Lobby : MonoBehaviourPun, IPunObservable 
{
    //components
    Rigidbody myRB;
    Animator myAnim;
    Transform myAvatar;
    //player movement
    [SerializeField] InputAction WASD;
    Vector2 movementInput;
    [SerializeField] float movementSpeed;

    //sync direction player
    private float direction = 1;

    //networking
    private PhotonView myPv;
    private Camera myCamera;
    private AudioListener myAudio;
    
    private bool canMove = true;

    public void MovePlayer()
    {
        canMove = true;
    }
    
    public void StopPlayer()
    {
        canMove = false;
    }

    private void OnEnable()
    {
        WASD.Enable();
    }

    private void OnDisable()
    {
        WASD.Disable();
    }

    private void Start()
    {
        myPv = GetComponent<PhotonView>();
        myCamera = transform.GetChild(1).GetComponent<Camera>();
        myAudio = transform.GetChild(1).GetComponent<AudioListener>();
        
        myRB = GetComponent<Rigidbody>();
        myAvatar = transform.GetChild(0);
        myAnim = GetComponent<Animator>();

        if (!myPv.IsMine)
        {
            myCamera.gameObject.SetActive(false);
            myAudio.gameObject.SetActive(false);
            return;
        }
    }

    private void Update()
    {
        myAvatar.localScale = new Vector2(direction, 1);
        if (!myPv.IsMine) return;
        movementInput = WASD.ReadValue<Vector2>();
        myAnim.SetFloat("Speed", movementInput.magnitude);
        if (movementInput.x != 0)
        {
            direction = Mathf.Sign(movementInput.x);
        }
    }

    private void FixedUpdate()
    {
        if (!myPv.IsMine) return;
        myRB.velocity = movementInput * movementSpeed;
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
}