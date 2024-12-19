using UnityEngine;

public class SkillsReference : MonoBehaviour
{
    [SerializeField] SkillLogic QSK;
    [SerializeField] SkillLogic WSK;
    [SerializeField] SkillLogic ESK;
    [SerializeField] SkillLogic RSK;

    public SkillLogic ActiveThisSkill(int s)
    {
        switch(s)
        {
            case 0:
                return QSK;
            case 1:
                return WSK;
            case 2:
                return ESK;
            case 3:
                return RSK;
        }

        return default;
    }
}
