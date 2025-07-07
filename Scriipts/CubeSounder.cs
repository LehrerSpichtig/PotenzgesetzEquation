using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeSounder : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip driveClip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Win()
    {
        while (AudioSource.isPlaying) AudioSource.Stop();
        //AudioSource.clip = winClip;
        //AudioSource.Play();
        AudioSource.PlayOneShot(winClip);
    }

    public void Lose()
    {
        //AudioSource.Stop();
        //AudioSource.clip = loseClip;
        AudioSource.PlayOneShot(loseClip);
    }

    public void Drive()
    {
        if (driveClip != null)
        {
            AudioSource.clip = driveClip;
            if (!AudioSource.isPlaying)
                AudioSource.Play();

        }

    }
}
