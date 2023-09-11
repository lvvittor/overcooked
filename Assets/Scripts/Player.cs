using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private const float MOVE_SPEED = 7f;
    [SerializeField] private const float ROTATE_SPEED = 10f;
    [SerializeField] private const float PLAYER_RADIUS = .7f;
    [SerializeField] private const float PLAYER_HEIGHT = 2f;
    [SerializeField] private const float INTERACT_DISTANCE = 2f;
    [SerializeField] private GameInput gameInput;

    [SerializeField] private LayerMask counterLayerMask;

    private bool isWalking;
    private Vector3 lastInteractDir;

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        if(Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, INTERACT_DISTANCE, counterLayerMask)) {
            if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)){
                clearCounter.Interact();
            }
        }

    }

    private void HandleMovement() { 
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);


        float moveDistance = MOVE_SPEED * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PLAYER_HEIGHT, PLAYER_RADIUS, moveDir, moveDistance);

        if (!canMove) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PLAYER_HEIGHT, PLAYER_RADIUS, moveDirX, moveDistance);

            if (canMove) {
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PLAYER_HEIGHT, PLAYER_RADIUS, moveDirZ, moveDistance);

                if (canMove){
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove){
            transform.position += moveDir * moveDistance;
        } 
        
        isWalking = moveDir != Vector3.zero;

        float rotateDistance = ROTATE_SPEED * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateDistance);
    }

    public bool IsWalking() {
        return isWalking;
    }
}
