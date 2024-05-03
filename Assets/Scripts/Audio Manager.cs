using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [SerializeField] private float MusicVolume = 1;
    [SerializeField] private float sfxVolume = 1;

    [SerializeField] private float MusicWeight = 0.2f;
    [SerializeField] private float sfxWeight = 0.3f;

    [SerializeField] private AudioClip MusicIntro;
    [SerializeField] private AudioClip[] MusicLoop;

    [SerializeField] private AudioClip[] SFX;

    private Queue<AudioClip> MusicQueue = new();
    private Queue<AudioClip> sfxQueue = new();

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("Audio Manager");
                    _instance = singletonObject.AddComponent<AudioManager>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this.gameObject);
            StartCoroutine(PlayMusicLoop());
        }
    }

    private void Start()
    {
        Creature.VoiceEvent += PlaysfxTrack;
    }

    private void AdjustVolumes()
    {
        float distribution = 1;

        _sfxSource.volume = distribution*sfxVolume;
        if(_sfxSource.isPlaying) distribution -= sfxWeight;

        _musicSource.volume = distribution*MusicVolume;
        
    }

    public void PlaysfxTrack(AudioClip clip)
    {
        sfxQueue.Enqueue(clip);
        TryPlayNext(sfxQueue, _sfxSource);
        Debug.Log(clip.name);
    }

    public void PlaySFX()
    {
        AudioSource.PlayClipAtPoint(SFX[Random.Range(0,SFX.Length)],transform.position);
    }

    private void TryPlayNext(Queue<AudioClip> Reference,AudioSource Source)
    {
        if (Reference.Count == 0) return;
        if (Reference.Peek() != null)
        {
            Source.clip = Reference.Dequeue();
            StartCoroutine(WaitTillEnd(Source.clip.length, Reference, Source));
            Source.Play();
        }

        AdjustVolumes();

    }

    private IEnumerator WaitTillEnd(float duration, Queue<AudioClip> Reference, AudioSource Source)
    {
        yield return new WaitForSeconds(duration);
        TryPlayNext(Reference, Source);
    }

    private IEnumerator PlayMusicLoop()
    {
        MusicQueue.Enqueue(MusicIntro);


        while (MusicQueue.Count > 0)
        {
            _musicSource.clip = MusicQueue.Dequeue();
            _musicSource.Play();
            AdjustVolumes();
            yield return new WaitForSeconds(_musicSource.clip.length);
            MusicQueue.Enqueue(MusicLoop[Random.Range(0,MusicLoop.Length)]);
            
        }

    }


}

