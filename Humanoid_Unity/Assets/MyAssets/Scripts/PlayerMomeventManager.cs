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

    public void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        isRunning = false;
        isGrounded = true;
        isHanging = false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputMovement = context.ReadValue<Vector2>();
        moveAmount = new Vector3(inputMovement.x, 0.0f, inputMovement.y) * movementMultiplier;
    }

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

        if (!isGrounded && !isHanging)
        {
            playerRb.AddRelativeForce(Vector3.up * fallMultiplier, ForceMode.Acceleration);

        }

        playerRb.angularVelocity = Vector3.zero;

    }
    
    bool IsGrounded()
    {
        Vector3 playerPosition = gameObject.transform.position;
        Vector3 endPosition = new Vector3(playerPosition.x, playerPosition.y - 1.1f, playerPosition.z);
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


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cornise")
        {
            Debug.Log("Hanging from Cornise");
            isHanging = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Cornise")
        {
            isHanging = false;
        }
    }
}
