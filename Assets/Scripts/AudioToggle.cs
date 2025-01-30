using System;
using ParentHouse.Audio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum Group {
    Sfx,
    Bgm
}

public class AudioToggle : MonoBehaviour {
    [SerializeField] [BoxGroup("Dependencies")]
    private GameObject ToggleImg;

    [SerializeField] [BoxGroup("Settings")]
    private string AudioGroupTarget;

    [SerializeField] [BoxGroup("Status")] [ReadOnly]
    private float AudioVolume;

    private void Start() {
        AudioVolume = PlayerPrefs.GetFloat(AudioGroupTarget, -100);
        SetVolume();
        SetToggleImg();
    }

    [Button]
    public void Toggle() {
        AudioVolume = AudioVolume >= 0 ? -100 : 0;
        PlayerPrefs.SetFloat(AudioGroupTarget,AudioVolume);
        SetVolume();
        SetToggleImg();
    }

    private void SetVolume() {
        AudioManager.OnSetGroupVolume?.Invoke(AudioGroupTarget,AudioVolume);
    }

    private void SetToggleImg() {
        print($"Volume: {AudioVolume}, Check: {AudioVolume >= 0}");
        ToggleImg.SetActive(AudioVolume >= 0);
    }
}
