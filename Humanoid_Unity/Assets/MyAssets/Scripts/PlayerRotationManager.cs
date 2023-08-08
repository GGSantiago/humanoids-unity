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
    public void OnLook(InputAction.CallbackContext context)
    {

        Vector2 rotationInput = context.ReadValue<Vector2>();
        // Rotate around Pivot
        Vector3 rotation = new Vector3(rotationInput.y, rotationInput.x, 0.0f);
        _lookAtReference.transform.RotateAround(_pivotReference.transform.position, rotation, sensibilityMultiplier);      

        // Remove lookat's z rotation
        Vector3 lookatRotation = new Vector3(_lookAtReference.transform.eulerAngles.x, _lookAtReference.transform.eulerAngles.y, 0.0f);
        _lookAtReference.transform.eulerAngles = lookatRotation;

        // Fix player's rotation
        Vector3 playerRotation = new Vector3(_lookAtReference.transform.position.x, _playerReference.transform.position.y, _lookAtReference.transform.position.z);
        _playerReference.transform.LookAt(playerRotation);
    }

    private void FixedUpdate() 
    {

        Vector3 playerPosition = _lookAtReference.transform.position;
        Vector3 endPosition = playerPosition + _lookAtReference.transform.forward.normalized;
        Debug.DrawLine(playerPosition, endPosition, Color.white);
    
    }


}
