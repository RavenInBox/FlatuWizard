using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private float volume = 1.0f;
    [SerializeField] TextMeshProUGUI Vol;
    [SerializeField] UpdateMyUpdate myUpdate;
    [SerializeField] MenuData menuData;

    public void SaveSelectAndroid()
    {
        menuData.IsAndroid = true;
    }

    public void SaveSelectPd()
    {
        menuData.IsAndroid = false;
    }

    public void OpenConfigMenu()
    {
        Time.timeScale = 0f;
        myUpdate.PauseAll = true;
    }

    public void CloseConfigMenu()
    {
        Time.timeScale = 1f;
        myUpdate.PauseAll = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void IncreaseVolume()
    {
        volume += 0.1f;
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        UpdateVolume();
    }

    public void DecreaseVolume()
    {
        volume -= 0.1f;
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        UpdateVolume();
    }

    private void UpdateVolume()
    {
        AudioListener.volume = volume;
        Vol.text = Mathf.Round(volume * 100).ToString();
    }
}
