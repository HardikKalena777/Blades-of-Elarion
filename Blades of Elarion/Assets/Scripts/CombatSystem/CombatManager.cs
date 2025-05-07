using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CombatManager : MonoBehaviour
{
    public State currentState;

    public List<AttackSO> lightCombo;
    public List<AttackSO> heavyCombo;
    public List<AttackSO> meleeCombo;
    public TMP_Text comboText;

    [SerializeField] int combatLayer;
    [SerializeField] float comboDelay = 0.5f;
    [SerializeField] float attackDelay = 0.2f;

    public UnityEvent onAttackStart;
    public UnityEvent onAttackEnd;

    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleComboUI(comboText);
        ExitAttack();
    }

    public void MeleeAttack()
    {
        HandleCombatState(State.MeleeAttack);
        Attack(meleeCombo);
    }

    public void LightAttack()
    {
        HandleCombatState(State.LightAttack);
        Attack(lightCombo);
    }

    public void HeavyAttack()
    {
        HandleCombatState(State.HeavyAttack);
        Attack(heavyCombo);
    }

    void Attack(List<AttackSO> combo)
    {
        onAttackStart?.Invoke();
        if (Time.time - lastComboEnd > comboDelay && comboCounter <= combo.Count)
        {
            CancelInvoke(nameof(EndCombo));

            if (Time.time - lastClickedTime >= attackDelay)
            {
                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                animator.Play("Attack", combatLayer, 0.07f);
                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    void HandleCombatState(State state)
    {
        currentState = state;
    }

    void ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(combatLayer).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(combatLayer).IsTag("Attack"))
        {
            Invoke(nameof(EndCombo), 1f);
            onAttackEnd?.Invoke();
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        HandleCombatState(State.Combat);
    }

    void HandleComboUI(TMP_Text comboText)
    {
        if (comboText == null)
            return;
        if (comboCounter > 0)
        {
            comboText.text = comboCounter.ToString() + "X";
        }
        else
        {
            comboText.text = "";
        }

    }
}
public enum State
{
    Combat,
    MeleeAttack,
    LightAttack,
    HeavyAttack,
}

