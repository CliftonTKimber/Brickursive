using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    // Start is called before the first frame update

    public List<AudioClip> audioClips;

    private AudioSource source;
    private float lowPitchRange = .9F;
    private float highPitchRange = 1.1F;
    public float volumeStarter = 10F;

    private Transform mainCamera;


    void Awake () {

        mainCamera = GameObject.FindWithTag("MainCamera").transform;
        source = GetComponent<AudioSource>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PlayBrickSnap(Vector3 playPosition)
    {
        transform.position = playPosition;

        source.pitch = Random.Range (lowPitchRange,highPitchRange);

        source.PlayOneShot(audioClips[0], 1f);
        
    }

    public void PlayBrickPop(Vector3 playPosition)
    {
        transform.position = playPosition;

        source.pitch = Random.Range (lowPitchRange,highPitchRange);

        source.PlayOneShot(audioClips[1], CalculateVolume(playPosition));
        
    }

    public void PlayVacuumBrick(Vector3 playPosition)
    {
        transform.position = playPosition;

        source.pitch = Random.Range (lowPitchRange,highPitchRange);

        source.PlayOneShot(audioClips[2], CalculateVolume(playPosition));

    }

    public void PlayMakeBrick(Vector3 playPosition)
    {
        transform.position = playPosition;

        source.pitch = Random.Range (lowPitchRange,highPitchRange);

        source.PlayOneShot(audioClips[3], CalculateVolume(playPosition));

    }

    private float CalculateVolume(Vector3 playPosition)
    {
        

        float distance = Vector3.Distance(mainCamera.position, playPosition);

        float volume = volumeStarter / distance;


        if(volume > 1f)
        {
            volume = 1f;
        }

        return volume;
    }
}
