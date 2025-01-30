using System;
using System.Collections.Generic;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour, IDamageable {
    [SerializeField, FoldoutGroup("Settings")]
    private int BreakAmount;

    [SerializeField, FoldoutGroup("Settings")]
    private float MoveSpeed;

    [SerializeField, FoldoutGroup("Settings")]
    private GameObject DestroyEffect;

    [SerializeField, FoldoutGroup("Settings")]
    private List<GameObject> SplitObjects = new();

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private Vector2 MoveDirection;

    private static Vector2 WorldOrigin = Vector2.zero; // Todo: Determine if this is the best place for this?
    private const float MaxDistance = 30f;
    private const float SafeTime = 0.1f;
    private float TimeUntilUnsafe;

    private void OnEnable() {
        SetDirection(new Vector2(Random.Range(-1f, 1f),-1f));
        TimeUntilUnsafe = Time.time + SafeTime;
        GameController.OnGameEnd += Kill;
    }

    private void OnDisable() {
        GameController.OnGameEnd -= Kill;
    }

    private void FixedUpdate() {
        transform.position += (Vector3)MoveDirection * (Time.fixedDeltaTime * MoveSpeed * GameController.GameTimeScale);
        if (Vector3.Distance(transform.position, WorldOrigin) > MaxDistance) {
            Kill();
        }
    }

    public void SetDirection(Vector2 direction) {
        MoveDirection = direction;
    }

    public void TakeHit() {
        if (Time.time < TimeUntilUnsafe) return;
        for (int i = 0; i < BreakAmount; i++) {
            Split();
        }
        Kill();
    }

    public void Kill() {
        Pooler.SpawnAt(DestroyEffect, transform.position);
        this.gameObject.SetActive(false);
    }

    private void Split() {
        Pooler.SpawnAt(SplitObjects[Random.Range(0, SplitObjects.Count)], transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out IDamageable t)) {
            t.TakeHit();
            TakeHit();
        }
    }
}