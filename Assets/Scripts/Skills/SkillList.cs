using System;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "SkillList", menuName = "Skill List", order = 0)]
public class SkillList : ScriptableObject {
    public SkillRecord[] skillRecords = { };

    public SkillRecord? GetSkillRecord(string skillName) {
        foreach (var skillRecord in skillRecords)
            if (skillRecord.skill.skillName == skillName)
                return skillRecord;
        return null;
    }

    public SkillRecord EnableSkill(string skillName) {
        var skillRecord = Array.Find(skillRecords, skillRecord => skillRecord.skill.skillName == skillName);
        skillRecord.enabled = true;
        return skillRecord;
    }

    public SkillRecord DisableSkill(string skillName) {
        var skillRecord = Array.Find(skillRecords, skillRecord => skillRecord.skill.skillName == skillName);
        skillRecord.enabled = false;
        return skillRecord;
    }

    public SkillRecord ToggleSkill(string skillName) {
        var skillRecord = Array.Find(skillRecords, skillRecord => skillRecord.skill.skillName == skillName);
        skillRecord.enabled = !skillRecord.enabled;
        return skillRecord;
    }

    public Skill[] GetEnabledSkills() {
        return skillRecords.Where(skillRecord => skillRecord.enabled).Select(skillRecord => skillRecord.skill)
            .ToArray();
    }

    public Skill[] GetDisabledSkills() {
        return skillRecords.Where(skillRecord => !skillRecord.enabled).Select(skillRecord => skillRecord.skill)
            .ToArray();
    }
}