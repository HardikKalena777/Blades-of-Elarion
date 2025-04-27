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
        if (comboCounter > 0)
        {
            comboText.text = comboCounter + "X";
        }
        else
        {
            comboText.text = "";
        }
        ExitAttack();
    }

    public void LightAttack()
    {
        HandleCombatState(State.Attack);
        Attack(lightCombo);
    }

    public void HeavyAttack()
    {
        HandleCombatState(State.Attack);
        Attack(heavyCombo);
    }

    void Attack(List<AttackSO> combo)
    {
        if (currentState == State.Base)
            return;
            onAttackStart?.Invoke();
            if (Time.time - lastComboEnd > comboDelay && comboCounter <= combo.Count)
            {
                CancelInvoke(nameof(EndCombo));

                if(Time.time - lastClickedTime >= attackDelay)
                {
                    animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                    animator.Play("Attack", combatLayer, 0f);
                    comboCounter++;
                    lastClickedTime = Time.time;

                    if(comboCounter >= combo.Count)
                    {
                        comboCounter = 0;
                    }
                }
            }
    }

    void HandleCombatState(State state)
    {
        if (currentState == State.Base)
            return;
        currentState = state;
    }

    void ExitAttack()
    {
        if(animator.GetCurrentAnimatorStateInfo(combatLayer).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(combatLayer).IsTag("Attack"))
        {
            Invoke(nameof(EndCombo), 1f);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
        HandleCombatState(State.Combat);
        onAttackEnd?.Invoke();
    }

}

public enum State
{
    Base,
    Combat,
    Attack,
}
