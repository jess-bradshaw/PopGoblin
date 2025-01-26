using UnityEngine;
// inspired by https://www.youtube.com/watch?v=S1WJ9H-KvKo tutorial 
public class PlayerController : MonoBehaviour
{
    public float movementSpeed; 

    Vector2 moveInput; 
    [SerializeField] bool jumpInput; 
   public float jumpForce; 

    //Raycasting for ground checking
    public Transform groundChecker; 
    public LayerMask ground; 
    public float rayLength; 
    [SerializeField] bool grounded; 
    [SerializeField] bool backTurned;

    public bool flipped; 
    public float flipSpeed; //helps with which direction to flip 

    Quaternion flipLeft = Quaternion.Euler(0, -180, 0);
    Quaternion flipRight = Quaternion.Euler(0, 0, 0);

    Rigidbody playerRB; 
    Animator playerAnimator;

    //audio sounds
    public AudioClip jumpSound;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>(); 
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput.x = Input.GetAxis("Horizontal"); 
        moveInput.y = Input.GetAxis("Vertical"); 

        playerAnimator.SetFloat("MoveSpeed", playerRB.linearVelocity.magnitude); //pushes speed of character to animator 

        //check to see if we have flipped 
        if (!flipped && moveInput.x <0)
        {
            flipped = true; 
        }
        else if (flipped && moveInput.x >0)
        {
            flipped = false; 
        }

        if (flipped)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, flipLeft, flipSpeed * Time.deltaTime); 
        }
        else if (!flipped)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, flipRight, flipSpeed * Time.deltaTime);
        }
        // check to seee if we are moving deeper (backwards) in the scene
        if(!backTurned && moveInput.y >0)
        {
            backTurned = true; 
        }
        else if (backTurned && moveInput.y < 0)
        {
            backTurned = false; 
        }

        playerAnimator.SetBool("BackTurned", backTurned); 

        if(Input.GetKeyDown(KeyCode.Space) && grounded) jumpInput = true; 
        playerAnimator.SetBool("Grounded", grounded); 
    }

    private void FixedUpdate()
    {
        playerRB.linearVelocity = new Vector3(moveInput.x * movementSpeed, playerRB.linearVelocity.y, moveInput.y * movementSpeed); 

        RaycastHit hit; 
        if (Physics.Raycast(groundChecker.position, Vector3.down, out hit, rayLength, ground)) //when you are on the groun set grounded to true. 
        {   
            grounded = true; 
        }
        else 
        {
            grounded = false; 
        }

        Debug.DrawRay(groundChecker.position, Vector2.down, Color.red); 

        if(jumpInput) 
        {
            Jump(); 
        }
    }
    
    void Jump()
    {
        playerRB.linearVelocity = new Vector3(0f, jumpForce, 0f);
        playerAnimator.SetTrigger("Jump");
        jumpInput = false;
        audioSource.PlayOneShot(jumpSound);
    }
}
