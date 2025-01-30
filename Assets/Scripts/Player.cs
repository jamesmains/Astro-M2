using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable {
    [SerializeField, FoldoutGroup("Settings")]
    private float EntrySpeed = 2f;

    [SerializeField, FoldoutGroup("Settings")]
    private Vector3 EntryPosition;
    
    [SerializeField, FoldoutGroup("Settings")]
    private Vector3 StartPosition;
    
    [SerializeField, FoldoutGroup("Settings")]
    private float HorizontalBounds;

    [SerializeField, FoldoutGroup("Settings")]
    private float ControlChangeSpeed;

    [SerializeField, FoldoutGroup("Settings")]
    private float MoveSpeed;

    [SerializeField, FoldoutGroup("Dependencies")]
    private Animator PlayerAnim;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private float Input;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private float CachedInput;

    private InputSystem_Actions PlayerInput;

    private void OnEnable() {
        if (PlayerInput == null) {
            PlayerInput = new InputSystem_Actions();
        }

        StartCoroutine(EntryRoutine());
        IEnumerator EntryRoutine() {
            transform.position = EntryPosition;
            while (Vector3.Distance(transform.position, StartPosition) > 0.05f) {
                transform.position = Vector3.Lerp(transform.position, StartPosition, EntrySpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            RegisterCallbacks();
            GameController.OnGameStart?.Invoke();
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDisable() {
        PlayerInput.Disable();
        PlayerInput.Player.Move.performed -= SetInput;
        PlayerInput.Player.Move.canceled -= SetInput;
    }

    private void RegisterCallbacks() {
        PlayerInput.Enable();
        PlayerInput.Player.Move.performed += SetInput;
        PlayerInput.Player.Move.canceled += SetInput;
    }

    public void SetInput(InputAction.CallbackContext callbackContext) {
        CachedInput = callbackContext.ReadValue<float>();
    }

    private void Update() {
        Input = Mathf.Lerp(Input, CachedInput, (ControlChangeSpeed * Time.deltaTime) * GameController.GameTimeScale);
    }

    private void FixedUpdate() {
        var pos = transform.position;
        pos.x += Input;
        pos.x = Mathf.Clamp(pos.x, -HorizontalBounds, HorizontalBounds);
        transform.position = Vector3.Lerp(transform.position, pos, (Time.fixedDeltaTime * MoveSpeed)* GameController.GameTimeScale);
    }

    public void TakeHit() {
        GameController.GameTimeScale = 0;
        PlayerAnim.SetTrigger("Death");
    }

    public void FinalizePlayerDeath() {
        GameController.GameTimeScale = 1;
        GameController.OnGameEnd?.Invoke();
    }
}