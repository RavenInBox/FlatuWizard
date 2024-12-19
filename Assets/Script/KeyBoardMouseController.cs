using System.Runtime.InteropServices;
using UnityEngine;

public class KeyBoardMouseController : MonoBehaviour
{
    public KeyCode CameraLock;
    public KeyCode CameraTempLock;
    public KeyCode Menu;
    public KeyCode Q;
    public KeyCode W;
    public KeyCode E;
    public KeyCode R;
    public KeyCode ONE;
    public KeyCode D;
    public KeyCode F;

    public bool bQ;
    public bool bW;
    public bool bE;
    public bool bR;
    public bool bOne;
    public bool bD;
    public bool bF;
    bool openMenu;

    [SerializeField] MenuData menuData;
    [SerializeField] GameObject Topr;
    [SerializeField] GameObject Topl;
    [SerializeField] GameObject MenuUI;

    private void Start()
    {
        if (menuData.IsAndroid)
        {
            Topr.SetActive(true);
            Topl.SetActive(true);
        }
        else
        {
            Topr.SetActive(false);
            Topl.SetActive(false);
        }
    }

    public void UpdateInputs()
    {
        if (Input.GetKeyDown(Menu))
        {
            if(openMenu)
            {
                openMenu = false;
                MenuUI.SetActive(false);
            }
            else
            {
                openMenu = true;
                MenuUI.SetActive(true);
            }
        }

        if (menuData.IsAndroid) return;

        bQ = Input.GetKeyDown(Q);
        bW = Input.GetKeyDown(W);
        bE = Input.GetKeyDown(E);
        bR = Input.GetKeyDown(R);
        bOne = Input.GetKeyDown(ONE);
        bD = Input.GetKeyDown(D);
        bF = Input.GetKeyDown(F);
    }

    public void EnterQ(bool state)
    {
        bQ = state;
    }

    public void EnterW(bool state)
    {
        bW = state;
    }

    public void EnterE(bool state)
    {
        bE = state;
    }

    public void EnterR(bool state)
    {
        bR = state;
    }

    public void EnterF(bool state)
    {
        bF = state;
    }

    public void EnterD(bool state)
    {
        bD = state;
    }

    public void EnterOne(bool state)
    {
        bOne = state;
    }
}
