using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    enum GameState
    {
        playing,
        paused,
    }

    private GameState gstate;
    // Start is called before the first frame update
    void Start()
    {
        gstate = GameState.playing;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
