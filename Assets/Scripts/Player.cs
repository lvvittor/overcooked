using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {

    public static Player Instance { get; private set; }

    public event EventHandler OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private float throwForce = 10f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask kitchenObjectsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private KitchenObject floorKitchenObject;

    private void Awake() {
        if (Instance != null){
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }

    private void Start(){
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        gameInput.OnThrowAction += GameInput_OnThrowAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e){
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null){
            selectedCounter.Interact(this);
            return;
        }
        if (floorKitchenObject != null) {
            floorKitchenObject.SetKitchenObjectParent(this);
            Rigidbody kitchenObjectRb = floorKitchenObject.GetComponent<Rigidbody>();
            if (kitchenObjectRb != null) {
                kitchenObjectRb.isKinematic = true; // Make it kinematic when picked up.
            }
            floorKitchenObject.transform.localRotation = Quaternion.identity;
            floorKitchenObject = null;
            return;
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, System.EventArgs e) {
        if(!KitchenGameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    private void GameInput_OnThrowAction(object sender, System.EventArgs e) {
        if (HasKitchenObject()) {
            ThrowKitchenObject();
        }
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public void HandleInteractions() {
        CheckForObjectsOnFloor();
        CheckForCounter();
    }

    private void CheckForObjectsOnFloor() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float sphereCastRadius = 0.5f;
        Vector3 startPos = transform.position + Vector3.up * 0.1f;  // slightly above ground

        if (Physics.SphereCast(startPos, sphereCastRadius, lastInteractDir, out RaycastHit hitInfo, 2f, kitchenObjectsLayerMask)) {
            KitchenObject floorObject = hitInfo.transform.GetComponent<KitchenObject>();
            if (floorObject != null) {
                floorKitchenObject = floorObject;
            } else {
                floorKitchenObject = null;
            }
        } else {
            floorKitchenObject = null;
        }
    }

    private void CheckForCounter() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    public void HandleMovement() { 
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance, countersLayerMask);

        if (!canMove) {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance, countersLayerMask);

            if (canMove) {
                // Can move only on the X
                moveDir = moveDirX;
            } else {
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance, countersLayerMask);

                if (canMove){
                    // Can move only on the Z
                    moveDir = moveDirZ;
                } else {
                     // Cannot move in any direction
                }
            }
        }

        if (canMove){
            EventQueueManager.Instance.AddCommand(new CmdMovement(transform, moveDir, moveSpeed));
            transform.position += moveDir * moveDistance;
        } 
        
        isWalking = moveDir != Vector3.zero;

        EventQueueManager.Instance.AddCommand(new CmdSlerp(transform, moveDir, rotateSpeed));
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject){
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject(){
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject(){
        return kitchenObject != null;
    }

    private void ThrowKitchenObject() {
        Rigidbody kitchenObjectRb = kitchenObject.GetComponent<Rigidbody>();
        if (kitchenObjectRb != null) {
            kitchenObjectRb.isKinematic = false; // Ensure physics affects the object
            Vector3 throwDirection = transform.forward;
            kitchenObjectRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
        }
        kitchenObject.SetKitchenObjectParent(null);
    }

}
