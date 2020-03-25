using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public enum GameAgentAction { Move, Wait, Potion, MeleeAttack, Taunt, RangedAttack, RangedAttackMultiShot, MagicAttackSingleTarget, MagicAttackAOE, Heal, Neutral };

public abstract class GameAgent : DungeonObject, Damageable, Renderable
{
    public float speed;
	public string nickname;
    public GameAgentStats stats;
    //public GameAgentAction currentAction;
	
    public GameAgentState currentState;
	public AIComponent AI;
	public int team;
	public int move_budget;

	public CharacterAnimator animator;
    public Inventory inventory = new Inventory();
	public bool animating = false;
	protected CharacterClassDefiner classDefiner;
	public Attack currentAttack;
	
    public abstract IEnumerator smooth_movement(Path path);
	
	public abstract void attack(Damageable target);
	public abstract void playAttackAnimation();
	public abstract void playHitAnimation();
	public abstract void playAttackNoise(string type);
	public abstract void playHitNoise(string type);
	public abstract bool animationFinished();

    public abstract void GetHealed(int amount);
	
	public abstract void init_agent(Pos position, GameAgentStats stats, string name = null);
	
	// for enemies, this will make them go through their AI motions
	// for players, this will trigger the boolean value that allows them to take their turn
	public abstract void take_turn();
	
	public abstract bool turn_over();

    // commands from the action menu
    public abstract void move();
    public abstract void act();
    public abstract void wait();
    public abstract void potion();
	public abstract void take_damage(int amount);
	private int actionNo;
	public bool SetCurrentAction(int action)
	{
		Attack[] attacks = stats.playerCharacterClass.GetAvailableActs();
		if (action >= attacks.Length) return false;
		else currentAttack = attacks[action];
		actionNo = action;
		return true;
	}
	public int GetCurrentAction()
	{
		return actionNo;
	}

    public void UseItemOnSelf(int slot)
    {
        Item item = inventory.GetItemFromSlot(slot);
        InventoryManager.UseItem(item, this);
        inventory.DecrementItemAtSlot(slot);
    }
	
	protected bool renderingEnabled = true;
	public void DisableRendering()
	{
		GetComponent<HealthBarController>().Disable();
		classDefiner.DisableRendering();
		renderingEnabled = false;
	}
	
	public void EnableRendering()
	{
		GetComponent<HealthBarController>().Enable();
		classDefiner.EnableRendering();
		renderingEnabled = true;
	}
}
