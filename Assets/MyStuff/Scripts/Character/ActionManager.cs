using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public void AttackingOtherPlayer(GameObject player, CharacterClass classType)
    {
        switch (classType)
        {
            case CharacterClass.Warrior:
                if (player.TryGetComponent<WarriorSkills>(out WarriorSkills warrior))
                {
                    warrior.OnNormalAttack();
                }
                    break;
            case CharacterClass.Archer:
                if (player.TryGetComponent<ArcherSkills>(out ArcherSkills archer))
                {
                    archer.OnNormalAttack();
                }
                    break;
            case CharacterClass.Mage:
                break;
            default:
                break;
        }
    }

    public void ResetSkillBehaviours(GameObject player, CharacterClass classType)
    {
        switch (classType)
        {
            case CharacterClass.Warrior:
                if(player.TryGetComponent<WarriorSkills>(out WarriorSkills warrior))
                 {
                    warrior.OnDeactivateGuard();
                }
                break;
            case CharacterClass.Archer:
                if(player.TryGetComponent<ArcherSkills>(out ArcherSkills archer))
                {
                    archer.OnDeactiveHide();
                }
                break;
            case CharacterClass.Mage:
                break;
            default:
                break;
        }
    }

    public void InvokeSkill(GameObject player, CharacterClass classType)
    {
        switch (classType)
        {
            case CharacterClass.Warrior:
                if (player.TryGetComponent<WarriorSkills>(out WarriorSkills warrior))
                {
                    player.GetComponent<WarriorSkills>().OnGuard();
                }
                break;
            case CharacterClass.Archer:
                break;
            case CharacterClass.Mage:
                break;
            default:
                break;
        }
    }

}
