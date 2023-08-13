using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMomeventManager : MonoBehaviour
{
    [SerializeField] private float movementMultiplier = 50.0f;
    [SerializeField] private float runningMultiplier  = 1.7f;
    
    [SerializeField] private float jumpMultiplier     = 1800.0f;
    [SerializeField] private float runningJumpMultiplier  = 1.2f;

    [SerializeField] private float hangingJumpMultiplier  = 0.8f;

    [SerializeField] private float fallMultiplier     = -50.0f;

    [SerializeField] private bool isRunning;
    private bool isGrounded;
    private bool isHanging;
    Vector3 moveAmount;
    Vector3 jumpAmount;
    Rigidbody playerRb;
    [SerializeField] private Transform _lookAtTransform;
    [SerializeField] private Transform _playerTransform;

    /// <summary>
    /// Method <c>Awake</c> initializes variables relative to the <c>PlayerMovementManager</c>.
    /// </summary>
    public void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        isRunning = false;
        isGrounded = true;
        isHanging = false;
    }

    /// <summary>
    /// Method <c>OnMove</c> is called whenever movement input is recieved. It calculates the movement,
    /// vector which later will be applied as a force in method <c>FixedUpdate</c>.
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
            Vector2 inputMovement = context.ReadValue<Vector2>();
            moveAmount = new Vector3(inputMovement.x, 0.0f, inputMovement.y) * movementMultiplier;
        
    }

    /// <summary>
    /// Method <c>OnJump</c> is called whenever jump action is recieved. It calculates the jump vector
    /// which later will be aaplied as a force in method <c>FixedUpdate</c>. It must check if the 
    /// conditions to jump are met, i.e, <c>isGrounded == true</c> or </c>isHanging == true<c>. This 
    /// method also takes into account <c>isRunning</c> and applies a multiplier, <c>runningJumpMultiplier</c>,
    /// to jump higher.
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        if (isGrounded)
        {
            jumpAmount = Vector3.up * jumpMultiplier;
            if(isRunning)
            {
                jumpAmount *= runningJumpMultiplier;
            }
        } 
        else
        {
            if (isHanging)
            {
                jumpAmount = Vector3.up * jumpMultiplier * hangingJumpMultiplier;
            }
        }

    }

    /// <summary>
    /// Method <c>OnRun</c> is called whenever run input is recieved and applies a multiplier, <c>runningMultiplier</c>,
    /// to <c>moveAmount</c> and updates the <c>isRunning</c> flag. 
    /// </summary>
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            moveAmount *= runningMultiplier;
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }


    /// <summary>
    /// Method <c>FixedUpdate</c> is called 60 times per second and applies the corresponding forces to move 
    /// the Player's GameObject
    /// </summary>
    public void FixedUpdate()
    {
        // Make motion relative to vector forward 
        Vector3 forward = gameObject.transform.forward.normalized;


        playerRb.AddRelativeForce(moveAmount, ForceMode.Acceleration);

        if (jumpAmount.sqrMagnitude > 0.0f)
        {   
            playerRb.AddRelativeForce(jumpAmount, ForceMode.Impulse);
            jumpAmount = Vector3.zero;
        }
        isGrounded = IsGrounded();

        if (!isGrounded)
        {
            playerRb.AddRelativeForce(Vector3.up * fallMultiplier, ForceMode.Acceleration);

        }

        playerRb.angularVelocity = Vector3.zero;

    }
    
    /// <summary>
    /// Method <c>IsGrounded</c> checks whether the player's GameObject is grounded. A Linecast 
    /// is used to check the collision against any type of object.
    /// TODO: use layers to only collide against floor/terrain or obstacles
    /// </summary>     
    bool IsGrounded()
    {
        Vector3 playerPosition = gameObject.transform.position;
        Vector3 endPosition = new Vector3(playerPosition.x, playerPosition.y - 1.2f, playerPosition.z);
        bool grounded = false;
        if (Physics.Linecast(playerPosition, endPosition))
        {
            grounded = true;
            Debug.DrawLine(playerPosition, endPosition, Color.white);
        }
        else
        {
            Debug.DrawLine(playerPosition, endPosition, Color.red);

        }
        return grounded;
    }


    /// <summary>
    /// Method <c>OnTriggerEnter</c> is called when the gameobject collides against anything. Currently
    /// it only checks if the collision is against a GameObject tagged with "Cornise".
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cornise")
        {
            Debug.Log("Hanging from Cornise");
            //isHanging = true;
        }
    }
    
    /// <summary>
    /// Method <c>OnTriggerEnter</c> is called when the gameobject stop colliding against anything. Currently
    /// it only checks if the end of the collision is against a GameObject tagged with "Cornise".
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Cornise")
        {
            //isHanging = false;
        }
    }
}
