using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Text;
    private bool won;
    private float time;
    private float timeLimit;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        timeLimit = 5;
        int result = PlayerPrefs.GetInt("Won");
        if (result == 0)
        {
            Text.text = "You Were Defeated!";
        }
        else
        {
            Text.text = "You Have Won!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > timeLimit)
        {
            SceneManager.LoadScene("MainMenu");
            timeLimit = float.MaxValue;
        }
    }
}
