using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotationManager : MonoBehaviour
{
    [SerializeField] float sensibilityMultiplier = 0.2f;
    [SerializeField] private GameObject _lookAtReference;
    [SerializeField] private GameObject _pivotReference;
    [SerializeField] private GameObject _playerReference;

    private void Awake() 
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Method <c>OnLook</c> handles the mouse/joystick input and rotates player accordingly.
    public void OnLook(InputAction.CallbackContext context)
    {
        #region
        // Read Input from the Input System
        Vector2 rotationInput = context.ReadValue<Vector2>();
        #endregion

        #region 
        // Apply rotation
        Vector3 horizontalRotation = new Vector3(0.0f, rotationInput.x, 0.0f);
        gameObject.transform.Rotate(horizontalRotation * sensibilityMultiplier);

        // The vertical rotation is applied exclusively to the pivot reference, and it has to be clamped
        Vector3 verticalRotation = new Vector3(rotationInput.y * sensibilityMultiplier, 0.0f, 0.0f);
        float currentVerticalRotation = _pivotReference.transform.rotation.eulerAngles.x;
        float finalRotation = currentVerticalRotation + verticalRotation.x;
        Debug.Log(finalRotation);
        if (finalRotation > 285.0f || finalRotation < 80.0f)
        {
            _pivotReference.transform.Rotate(verticalRotation);     
        }
        #endregion
    }

    private void FixedUpdate() 
    {

        Vector3 playerPosition = _lookAtReference.transform.position;
        Vector3 endPosition = playerPosition + _lookAtReference.transform.forward.normalized;
        Debug.DrawLine(playerPosition, endPosition, Color.white);
    
    }


}
