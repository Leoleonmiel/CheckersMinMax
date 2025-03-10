using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] private Image fadeImagePrefab;  
    private Canvas currentCanvas;
    #endregion

    #region UnityMessages
    void Start()
    {
        // Get the current canvas in the scene
        currentCanvas = FindAnyObjectByType<Canvas>();

        if (currentCanvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
        }
    }
    #endregion

    #region PublicMethods
    public void SetMenuState()
    {
        GameStateManager.Instance.currentState = GameStateManager.State.Menu;
        StartCoroutine(LoadSceneWithFade("MainMenu"));
    }

    public void SetPlayerVsPlayer()
    {
        GameStateManager.Instance.currentState = GameStateManager.State.PlayerVsPlayer;
        StartCoroutine(LoadSceneWithFade("GameScene"));
    }

    public void SetPlayerVsAI()
    {
        GameStateManager.Instance.currentState = GameStateManager.State.PlayerVsAI;
        StartCoroutine(LoadSceneWithFade("GameScene"));
    }
    #endregion

    #region PrivateMethods
    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        Image fadeImage = Instantiate(fadeImagePrefab, currentCanvas.transform);
        yield return Fade(fadeImage, 1f, Ease.InOutSine); 

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;  
            }

            yield return null;
        }

        yield return Fade(fadeImage, 0f, Ease.InOutSine);
    }

    private IEnumerator Fade(Image fadeImage, float targetAlpha, Ease easeType)
    {
        if (fadeImage != null)
        {
            fadeImage.DOFade(targetAlpha, 1f).SetEase(easeType);  
            yield return new WaitForSeconds(1f); 
        }
    }
    #endregion
}
