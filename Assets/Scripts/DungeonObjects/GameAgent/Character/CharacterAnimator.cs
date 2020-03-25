using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    #region Variables
    // Referenced conponents.
    GameAgent character;
    Animator animator;
    bool isDead = false;

    // Animation variation variables. (Do not change)
    int animationNumber;
    const int maxAttackAnimations = 3;
    const int maxHitAnimations = 5;
    const int maxBlockedAnimations = 2;

    // Animation duartion variables.
    const float actionDuration = 0.4f;
    const float particleDuration = 1.0f;

    // Particle System objects.
    public ParticleSystem magicAura;
    public ParticleSystem magicSparks;
    public ParticleSystem focusSpark;
    public ParticleSystem slashCharge;
    public ParticleSystem healAura;
    public ParticleSystem blood;
    public ParticleSystem hit;
    public ParticleSystem sparks;
    public ParticleSystem dust;
    public ParticleSystem ghosts;
    #endregion

    public void init()
    {
        // Get required components.
        character = GetComponent<GameAgent>();
        animator = GetComponent<Animator>();

        // Size up particle effects.
        magicAura.transform.localScale = new Vector3(3f, 3f, 3f);
        magicSparks.transform.localScale = new Vector3(2f, 2f, 2f);
        focusSpark.transform.localPosition = new Vector3(0f, 1.3f, 0.6f);
        healAura.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        blood.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        sparks.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        ghosts.transform.localScale = new Vector3(3f, 3f, 3f);
    }

    /*void Update()
    {

    }*/

    // StartMovementAnimation(), StopMovementAnimation(), PlayRotateAnimation()
    #region Character Movement
    public IEnumerator StartMovementAnimation()
    {
        animator.SetBool("Moving", true);
        animator.SetFloat("Velocity Z", 1);
        yield return null;
    }

    public IEnumerator StopMovementAnimation()
    {
        animator.SetBool("Moving", false);
        animator.SetFloat("Velocity Z", 0);
        yield return null;
    }

    public IEnumerator PlayRotateAnimation()
    {
        animator.SetTrigger("TurnLeftTrigger");
        yield return null;
    }
    #endregion

    // PlayAttackAnimation(), PlayUseItemAnimation()
    #region Character Action
    public IEnumerator PlayAttackAnimation()
    {
        animationNumber = UnityEngine.Random.Range(1, maxAttackAnimations + 1);

        if (animator.GetInteger("Weapon") == CharacterClassOptions.Staff)
        {
            animator.SetTrigger("CastAttack" + (animationNumber).ToString() + "Trigger");
            SpawnParticleSystemAtCharacter(magicAura);
            SpawnParticleSystemAtCharacter(magicSparks);
            yield return new WaitForSeconds(actionDuration);
            animator.SetTrigger("CastEndTrigger");
   
        }
        else if (animator.GetInteger("Weapon") == CharacterClassOptions.Bow)
        {
            animator.SetTrigger("Attack" + (animationNumber).ToString() + "Trigger");
            SpawnParticleSystemAtCharacter(focusSpark);
            yield return null;
        }
        else
        {
            animator.SetTrigger("Attack" + (animationNumber).ToString() + "Trigger");
            SpawnParticleSystemAtCharacter(slashCharge);
            yield return null;
        }
    }

    public IEnumerator PlayTauntAnimation() {
        animator.SetTrigger("ActivateTrigger");
        SpawnParticleSystemAtCharacter(magicAura);
        SpawnParticleSystemAtCharacter(magicSparks);
        yield return new WaitForSeconds(actionDuration);
        animator.SetTrigger("CastEndTrigger");
    }

    public bool AnimatorIsPlaying() {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public IEnumerator PlayUseItemAnimation()
    {
        animator.SetTrigger("ActivateTrigger");
        SpawnParticleSystemAtCharacter(healAura);
        yield return null;
    }
    #endregion

    // PlayHitAnimation(), PlayBlockAnimation(), PlayKilledAnimation()
    #region Character Reaction
    public IEnumerator PlayHitAnimation()
    {
        animationNumber = UnityEngine.Random.Range(1, maxHitAnimations + 1);
        animator.SetTrigger("GetHit" + (animationNumber).ToString() + "Trigger");
        SpawnParticleSystemAtCharacter(blood);
        SpawnParticleSystemAtCharacter(hit);
        yield return null;
    }

    public IEnumerator PlayBlockAnimation()
    {
        animator.SetBool("Blocking", true);
        animator.SetTrigger("BlockTrigger");
        animationNumber = UnityEngine.Random.Range(1, maxBlockedAnimations + 1);
        animator.SetTrigger("BlockGetHit" + (animationNumber).ToString() + "Trigger");
        SpawnParticleSystemAtCharacter(sparks);
        SpawnParticleSystemAtCharacter(dust);
        yield return new WaitForSeconds(actionDuration);
        animator.SetBool("Blocking", false);
    }

    public IEnumerator PlayKilledAimation()
    {
        animator.SetTrigger("Death1Trigger");
        SpawnParticleSystemAtCharacter(ghosts);
        yield return null;
        isDead = true;
    }
	
	public void PlayHealedAnimation()
	{
		SpawnParticleSystemAtCharacter(healAura);
	}
    #endregion

    // SpawnParticleSystemAtCharacter(ParticleSystem particle)
    #region Character Particle Effects
    void SpawnParticleSystemAtCharacter(ParticleSystem particle)
    {
        var clone = Instantiate(particle, character.transform);
        Destroy(clone.gameObject, particleDuration);
    }
    #endregion

}
