using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishOfLevel : MonoBehaviour
{
    private AudioSource finishSound;
    private bool levelCompleted = false;
    private void Start()
    {
        finishSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted) 
        { 
            finishSound.Play();
            levelCompleted = true;
            Invoke("CompleteOfTheLevel", 2f);
        }
    }


    private void CompleteOfTheLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
