using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace ParentHouse.Audio {
    public class AudioManager : MonoBehaviour {
        public static Action<AudioClip, float> OnPlayClip;
        public static Action<string, float> OnSetGroupVolume;
        [SerializeField] private AudioSource audioSrc;
        [SerializeField] private AudioMixer audioMixer;

        private void OnEnable() {
            OnPlayClip += HandlePlayClip;
            OnSetGroupVolume += HandleSetGroupVolume;
        }

        private void OnDisable() {
            OnPlayClip += HandlePlayClip;
            OnSetGroupVolume += HandleSetGroupVolume;
        }

        private void HandlePlayClip(AudioClip clip, float pitch) {
            if (clip == null) return;
            audioSrc.PlayOneShot(clip);
        }

        private void HandleSetGroupVolume(string channel, float volume) {
            Debug.Log(channel);
            audioMixer.SetFloat(channel, volume);
        }
    }
}