using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    public TMP_Text gold_text;
    public TMP_Text time_text;
    public TMP_Text enemies_text;
    // Start is called before the first frame update
    void Start()
    {
        gold_text.text = EndGameStats.GoldCollected.ToString();
        time_text.text = EndGameStats.TimePlayed.ToString();
        enemies_text.text = EndGameStats.EnemiesKilled.ToString();
    }
    public void StartNewGame()
    {
        SceneManager.LoadScene(1);
    }
    public void LeaveToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
