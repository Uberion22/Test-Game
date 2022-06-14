using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;

    [SerializeField] private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = audioClips.FirstOrDefault();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
