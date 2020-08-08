using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class GlobalAudio : MonoBehaviour
{
    private int chasingMonster = 0;

    public StudioEventEmitter BackgroundMusic;

    public StudioEventEmitter MonsterChaseMusic;

    public StudioEventEmitter SeenByMonster;

    /// <summary>
    /// Triggers the chasing sounds
    /// </summary>
    public void CueMonsterChase()
    {
        BackgroundMusic.Stop();

        chasingMonster++;
        if (!MonsterChaseMusic.IsPlaying()) 
            MonsterChaseMusic.Play();
        if (!SeenByMonster.IsPlaying())
            SeenByMonster.Play();
    }

    public void MonsterStopsChasing()
    {
        chasingMonster--;
        if (chasingMonster == 0)
        {
            BackgroundMusic.Play();
            MonsterChaseMusic.Stop();
        }
    }
}
