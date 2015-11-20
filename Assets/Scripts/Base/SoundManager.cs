using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance { get; set; }
    public bool _muted;
    private static bool _isMuted;
    public static bool isMuted {
        get { return _isMuted; }
        set {
            _isMuted = value;
            
        }
    }
    public Sound[] _sounds;
    private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
    private Transform soundParent;

    private void Awake() {
        instance = this;
        isMuted = _muted;

        FillSounds();
        soundParent = new GameObject("Sounds").transform;
        soundParent.parent = transform;
    }

    public static Sound Create(string name) {
        return Create(GetSound(name));
    }

    public static Sound Create(Sound sound) {
        GameObject go = new GameObject("Sound Source (" + sound.name + ")");
        go.transform.parent = instance.soundParent;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = sound.clip;
        sound.source = source;
        go.AddComponent<SoundTracker>().Init(sound);
        return sound;
    }

    public static Sound Play(string name) {
        return Play(SoundManager.GetSound(name));
    }

    public static Sound Play(Sound sound) {
        if(sound == null) {
            Debug.LogError("Sound can not be null!");
            return null;
        }
        sound.destroyOnFinish = true;
        Create(sound).Play();
        return sound;
    }

    public static Sound Pause(Sound sound) {
        return sound.Pause();
    }

    public static Sound GetSound(string name) {
        Sound sound;
        instance.sounds.TryGetValue(name.ToLower(), out sound);
        if(sound != null) return new Sound(sound.name, sound.clip);
        return null;
    }

    private void FillSounds() {
        for(int i = 0; i < _sounds.Length; i++)
            sounds.Add(_sounds[i].name.ToLower(), _sounds[i]);
    }
}

[System.Serializable]
public class Sound {
    public AudioClip clip;
    public string name;
    public delegate void SoundDelegate(Sound sound);
    public SoundDelegate onSoundEnded;

    public bool destroyOnFinish { get; set; }
    public AudioSource source { get; set; }
    public bool isPlaying { get; set; }
    public bool isPaused { get; set; }
    public float length { get { return clip.length; } }

    public Sound(string name, AudioClip clip) {
        this.name = name;
        this.clip = clip;
    }

    public Sound Play() {
        if(isPaused) {
            source.Play();
            isPaused = false;
            isPlaying = true;
            return this;
        } else if(source != null) {
            source.mute = SoundManager.isMuted;
            source.Play();
            isPlaying = true;
            return this;
        }
        SoundManager.Play(this);
        isPlaying = true;
        return this;
    }

    public Sound Pause() {
        if(source != null) {
            source.Pause();
            isPaused = true;
            isPlaying = false;
        } else NotInit();
        return this;
    }

    public void Stop(bool destroy = false) {
        if(source != null) {
            source.Stop();
            if(destroy) PixelDog.Destroy(source.gameObject);
        } else NotInit();
    }

    public Sound SetPitch(float pitch) {
        if(source != null) {
            if(pitch > 0) source.pitch = pitch;
        } else NotInit();
        return this;
    }

    public Sound SetVolume(float volume) {
        if(source != null) {
            if(volume > 0) source.volume = volume;
        } else NotInit();
        return this;
    }

    public Sound SetLoop(bool loop) {
        if(source != null) {
            source.loop = loop;
        } else NotInit();
        return this;
    }

    private void NotInit() {
        Debug.LogWarning("This sound has not been properly initialized");
    }
}

public class SoundTracker : MonoBehaviour {
    private Sound sound;

    public void Init(Sound sound) {
        this.sound = sound;
    }

    private void Update() {
        if(sound.source.time >= sound.length) SoundEnded();
    }

    private void SoundEnded() {
        if(sound.onSoundEnded != null) sound.onSoundEnded(sound);
        if(sound.destroyOnFinish) Destroy(gameObject);
        enabled = false;
    }
}
