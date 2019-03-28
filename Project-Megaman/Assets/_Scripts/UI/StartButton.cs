using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField] private float flashRate;
    [SerializeField] private float flashDuration;
    [SerializeField] private string targetScene;
    private Text targetText;
    private Button button;

    private Coroutine cr_ChangeScene;
    // Start is called before the first frame update
    void Start()
    {
        targetText = GetComponent<Text>();
        button = GetComponent<Button>();
    }

    public void OnDisable()
    {
        CoroutineManager.HaltCoroutine(ref cr_ChangeScene, this);
    }

    public void BeginLoadSceneSequence()
    {
        CoroutineManager.BeginCoroutine(ChangeScene(), ref cr_ChangeScene, this);
    }

    private IEnumerator ChangeScene()
    {
        button.enabled = false;

        Color startColor = targetText.color;
        Color flashColor = startColor;
        flashColor.a = 0;
        targetText.color = flashColor;
        float nextFlashThreshold = flashRate;
        bool isStartColor = false;
        float t = 0;

        while(t < flashDuration)
        {
            if(t > nextFlashThreshold)
            {
                nextFlashThreshold += flashRate;
                if (isStartColor)
                {
                    targetText.color = flashColor;
                }
                else
                {
                    targetText.color = startColor;
                }
                isStartColor = !isStartColor;
            }
            t += Time.unscaledDeltaTime;

            yield return null;
        }

        targetText.color = startColor;
        SceneManager.LoadScene(targetScene);
    }
}
