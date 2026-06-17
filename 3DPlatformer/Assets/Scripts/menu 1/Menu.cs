using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public void LoadBeginner() => LoadScene("BeginnerLevel");
    public void LoadAdvanced() => LoadScene("AdvancedLevel");
    public void LoadExpert() => LoadScene("ExpertLevel");

    private void LoadScene(string sceneName)
    {
        DOTween.KillAll();
        SceneManager.LoadScene(sceneName);
    }
}