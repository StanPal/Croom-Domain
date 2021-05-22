using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
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
                player.GetComponent<WarriorSkills>().OnGuard();
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
