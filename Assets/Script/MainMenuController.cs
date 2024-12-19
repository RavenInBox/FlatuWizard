using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] MenuData menuData;

    public void PCStart()
    {
        menuData.IsAndroid = false;
        SceneManager.LoadScene("Dev");
    }

    public void ANDStart()
    {
        menuData.IsAndroid = true;
        SceneManager.LoadScene("Dev");
    }
}
