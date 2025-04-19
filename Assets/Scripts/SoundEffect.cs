// SoundEffectsManager.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton manager for playing sound effects in the terminal game.
/// Assign your clips in the inspector and call Play() by effect name.
/// </summary>
public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip typingClip;
    public AudioClip successClip;
    public AudioClip errorClip;
    public AudioClip backClip;
    public AudioClip clearClip;

    private AudioSource audioSource;
    private Dictionary<Effect, AudioClip> clipMap;

    public enum Effect
    {
        Typing,
        Success,
        Error,
        Back,
        Clear
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        clipMap = new Dictionary<Effect, AudioClip>
        {
            { Effect.Typing, typingClip },
            { Effect.Success, successClip },
            { Effect.Error, errorClip },
            { Effect.Back, backClip },
            { Effect.Clear, clearClip }
        };
    }

    /// <summary>
    /// Play a one‑shot sound effect by enum.
    /// </summary>
    public void Play(Effect effect)
    {
        if (clipMap.TryGetValue(effect, out AudioClip clip) && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"No clip assigned for effect {effect}");
        }
    }
}
