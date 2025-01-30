using System;
using System.Collections.Generic;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner : MonoBehaviour {
    [SerializeField, FoldoutGroup("Settings")]
    private Vector2 HorizontalSpawnRange;

    [SerializeField, FoldoutGroup("Settings")]
    private Vector2 VerticalSpawnRange;

    [SerializeField, FoldoutGroup("Settings")]
    private List<GameObject> AsteroidPrefabs = new();

    [SerializeField, FoldoutGroup("Settings")]
    private float SpawnRate = 1f;

    [SerializeField, FoldoutGroup("Settings")]
    private float SpawnRateRamp = 0.05f;
    
    [SerializeField, FoldoutGroup("Settings")]
    private Vector2 SpawnRateLimit;

    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private bool IsActive;
    
    [SerializeField, FoldoutGroup("Status"), ReadOnly]
    private float NextSpawnTime;

    private void OnEnable() {
        GameController.OnGameStart += HandleGameStart;
        GameController.OnGameEnd += HandleGameEnd;
    }


    private void OnDisable() {
        GameController.OnGameStart -= HandleGameStart;
        GameController.OnGameEnd -= HandleGameEnd;
    }
    
    private void HandleGameStart() {
        IsActive = true;
        SpawnRate = SpawnRateLimit.y;
    }

    private void HandleGameEnd() {
        IsActive = false;
    }
    
    private void Update() {
        if (!IsActive) return;
        if (Time.time >= NextSpawnTime && GameController.GameTimeScale > 0) {
            Spawn();
        }

        SpawnRate -= Time.deltaTime * SpawnRateRamp;
        SpawnRate = Mathf.Clamp(SpawnRate, SpawnRateLimit.x, SpawnRateLimit.y);
    }

    private void Spawn() {
        NextSpawnTime = Time.time + SpawnRate;
        var asteroidObject = Pooler.SpawnAt(AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Count)],
            new Vector3(Random.Range(HorizontalSpawnRange.x, HorizontalSpawnRange.y),
                Random.Range(VerticalSpawnRange.x, VerticalSpawnRange.y), 0));
        if (asteroidObject.TryGetComponent(out Asteroid t)) {
            t.SetDirection(new Vector2(Random.Range(-.3f, .3f), -1f));
        }
    }
}