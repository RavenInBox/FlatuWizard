using UnityEngine;

public class InibidorController : MonoBehaviour
{
    [SerializeField] ManagerSonido managerSonido;
    [SerializeField] Animator animator;
    [SerializeField] MinionController[] minions = new MinionController[0];

    const string IDLE = "IdleInibidor";
    const string SPAWN = "Spawn";

    [SerializeField] int count = 0;

    public void CallToAction()
    {
        animator.Play(SPAWN);
    }

    private void Spawn() // animator llama esta funcion
    {
        if (count > 4)
        {
            animator.Play(IDLE);
            count = 0;
            return;
        }

        MinionController minion = minions[count];

        minion.gameObject.SetActive(true);
        minion.StartActions();
        count++;
    }

    private void SpwanSound() // animator tambien llama esta funcion
    {
        managerSonido.Play(SPAWN);
    }
}
