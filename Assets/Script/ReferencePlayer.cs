using UnityEngine;

public class ReferencePlayer : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] MasterGame game;

    void StartSkillAnimation()
    {
        controller.SkillAnimation();
    }

    void BasicAttak()
    {
        controller.LaunchBasic();
    }

    void DeadEnd()
    {
        game.EndGame();
    }
}
