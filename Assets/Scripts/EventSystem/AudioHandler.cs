using System.Collections;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    AudioSource loopSource, oneShotSource;
    Coroutine loopStopRoutine;

    private void Start()
    {
        loopSource = gameObject.AddComponent<AudioSource>();
        oneShotSource = gameObject.AddComponent<AudioSource>();
        loopSource.loop = true;
    }

    public IEnumerator PlayClips(Sound[] sounds)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.loop)
                StartLoop(sound);
            else
            {
                oneShotSource.PlayOneShot(sound.audioClip, sound.volume);
                if (sound.waitForEnd)
                {
                    yield return new WaitForSeconds(sound.audioClip.length);
                }
            }

            yield return null;
        }

        //loopStopRoutine = null;
    }

    void StartLoop(Sound sound)
    {
        loopSource.Stop();

        loopSource.clip = sound.audioClip;
        loopSource.volume = sound.volume;
        loopSource.Play();

        if (sound.stopAtEnd)
            EventManager.SetSoundStop();
        else
            EventManager.SetSoundStop(sound.stopAtStep);
    }

    public void StopLoop()
    {
        loopSource.Stop();
    }
}
