using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public static LoadLevel instance;

    public Animator crossFadeAnim;
    public float transitionTime = 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // load by string
    public IEnumerator LoadLevelString(string levelName)
    {
        crossFadeAnim.SetTrigger("trigger");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelName);
    }

    // load by index
    public IEnumerator LoadLevelIndex(int levelIndex)
    {
        crossFadeAnim.SetTrigger("trigger");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
