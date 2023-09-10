using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private GameInput gameInput;

    private bool isWalking;

    private void Update() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = CalculateValidMoveDirection(inputVector);

        MovePlayer(moveDir);
    }

    private void MovePlayer(Vector3 moveDir){
        float moveDistance = moveSpeed * Time.deltaTime;
        transform.position += moveDir * moveDistance;
        isWalking = moveDir != Vector3.zero;
        
        UpdateRotation(moveDir);
    }

    private void UpdateRotation(Vector3 moveDir){
        float rotateDistance = rotateSpeed * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotateDistance);
    }

    private bool CanMoveInDirection(Vector3 moveDir, float moveDistance){
        return !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);
    }

    private Vector3 CalculateValidMoveDirection(Vector2 inputVector){
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;

        if (!CanMoveInDirection(moveDir, moveDistance)){
            // Cannot move towards moveDir
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;

            if (CanMoveInDirection(moveDirX, moveDistance)){
                // Can move only on the X
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
                if (CanMoveInDirection(moveDirZ, moveDistance)){
                    // Can move only on the Z
                    moveDir = moveDirZ;
                }
            }
        }

        return moveDir;
    }

    public bool IsWalking() {
        return isWalking;
    }
}
