using UnityEngine;
public class InputController : MonoBehaviour
{
    private MainInput inputActions;
    
    private void Awake()
    {
        inputActions = new MainInput();
    }
    
    private void OnEnable()
    {
        inputActions.Controls.Enable();
        inputActions.Controls.MoveRight.started += OnMoveRight;
        inputActions.Controls.MoveLeft.started += OnMoveLeft;
        inputActions.Controls.Jump.started += OnJump;
    }
    
    private void OnDisable()
    {
        inputActions.Controls.MoveRight.started -= OnMoveRight;
        inputActions.Controls.MoveLeft.started -= OnMoveLeft;
        inputActions.Controls.Jump.started -= OnJump;
        inputActions.Controls.Disable();
    }
    
    private void OnMoveRight(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log("Move Right input - Swapping to right lane");
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.MoveRight();
        }
    }
    
    private void OnMoveLeft(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log("Move Left input - Swapping to left lane");
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.MoveLeft();
        }
    }
    
    private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log("Jump input");
        PlayerController.Instance?.Jump();
    }
}
