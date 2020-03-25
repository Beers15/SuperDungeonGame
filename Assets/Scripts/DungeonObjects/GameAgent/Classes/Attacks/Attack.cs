using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack
{
	public int range;
	protected int AOE;
	protected float damageModifier;
    public int MPcost;
	
	public bool attacking;
	public abstract IEnumerator Execute(GameAgent attacker, Damageable target);
	public abstract string toString();
	
	public Attack(int range, int AOE, float damageModifier, int MPcost) {
		this.range = range;
		this.AOE = AOE;
		this.damageModifier = damageModifier;
        this.MPcost = MPcost;
	}
	public Attack(){}

    public static Dictionary<string, Attack> Get = new Dictionary<string, Attack>() {
        ["Melee"] = new MeleeAttack(),
        ["WeakMelee"] = new WeakMeleeAttack(),
        ["StrongMelee"] = new StrongMeleeAttack(),
        ["Shortbow"] = new ShortbowAttack(),
        ["Longbow"] = new LongbowAttack(),
        ["Fire"] = new FireSpell(),
        ["Fire Storm"] = new FireStormSpell(),
        ["Berserk"] = new Berserk(),
        ["Multishot"] = new Multishot(),
        ["Lightning"] = new LightningSpell()
	};
}

public class MeleeAttack : Attack
{
	public MeleeAttack() : base(2, 1, 2, 0) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		while (attacking) yield return null;
		attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Melee");
		
		Debug.Log("Waiting for animation to finish");
		while (attacker.animating) yield return null;
		
		try {
			target.playHitAnimation();
			target.playHitNoise("Melee");
			target.take_damage((int) (attacker.stats.DealDamage() * damageModifier));
		}
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Slash"; }
}

public class WeakMeleeAttack : Attack
{
    public WeakMeleeAttack() : base(2, 1, 1, 0) { }
    public override IEnumerator Execute(GameAgent attacker, Damageable target)
    {
		while (attacking) yield return null;
        attacking = true;

        attacker.transform.LookAt((target as DungeonObject).transform);
        attacker.playAttackAnimation();
        attacker.playAttackNoise("Melee");

        Debug.Log("Waiting for animation to finish");
        while (attacker.animating) yield return null;

        try
        {
            target.playHitAnimation();
            target.playHitNoise("Melee");
            target.take_damage((int)(attacker.stats.DealDamage() * damageModifier));
        }
        catch (Exception e)
        {
            // swallow the error
        }

        attacking = false;
    }
    public override string toString() { return "Swing"; }
}

public class StrongMeleeAttack : Attack
{
    public StrongMeleeAttack() : base(2, 1, 4.25f, 2) { }
    public override IEnumerator Execute(GameAgent attacker, Damageable target)
    {
		while (attacking) yield return null;
        if (attacker.stats.currentMagicPoints < 2)
        {
            yield break;
        }
        attacker.stats.currentMagicPoints = Mathf.Max((attacker.stats.currentMagicPoints - 2), 0);
        attacking = true;

        attacker.transform.LookAt((target as DungeonObject).transform);
        attacker.playAttackAnimation();
        attacker.playAttackNoise("Melee");

        Debug.Log("Waiting for animation to finish");
        while (attacker.animating) yield return null;

        try
        {
            target.playHitAnimation();
            target.playHitNoise("Melee");
            target.take_damage((int)(attacker.stats.DealDamage() * damageModifier));
        }
        catch (Exception e)
        {
            // swallow the error
        }

        attacking = false;
    }
    public override string toString() { return "Power Slash"; }
}

public class ShortbowAttack : Attack
{
	public ShortbowAttack() : base(7, 1, 1.3f, 0) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		while (attacking) yield return null;
		attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Bow");
		
		while (attacker.animating) yield return null;
		
		Projectile arrow = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "arrow");
		
		while (!(arrow == null)) yield return null;
		
		try {
		target.playHitAnimation();
		target.playHitNoise("Bow");
		target.take_damage((int) (attacker.stats.DealDamage() * damageModifier));
		}
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Shoot (short)"; }
}

public class LongbowAttack : Attack
{
	public LongbowAttack() : base(11, 1, 0.9f, 0) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		//while (attacking) yield return null;
		attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Bow");
		
		while (attacker.animating) yield return null;
		
		var arrow = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "arrow");
		
		while (!(arrow == null)) yield return null;
		
		try {
		target.playHitAnimation();
		target.playHitNoise("Bow");
		target.take_damage((int) (attacker.stats.DealDamage() * damageModifier));
		}
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Shoot (long)"; }
}

public class FireSpell : Attack
{
	public FireSpell() : base(6, 1, 2, 2) {}
	public override IEnumerator Execute(GameAgent attacker, Damageable target)
	{
		//while (attacking) yield return null;
        if (attacker.stats.currentMagicPoints < 2)
        {
            yield break;
        }
        attacker.stats.currentMagicPoints = Mathf.Max((attacker.stats.currentMagicPoints - 2), 0);
        attacking = true;
		
		attacker.transform.LookAt((target as DungeonObject).transform);
		attacker.playAttackAnimation();
		attacker.playAttackNoise("Fire");
		
		while (attacker.animating) yield return null;
		
		Projectile fire = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "fire");
		
		while (!(fire == null)) yield return null;
		
		try {
		target.playHitAnimation();
		target.playHitNoise("Fire");
        target.take_damage((int)(attacker.stats.DealDamage() * damageModifier));
        }
		catch (Exception e) {
			// swallow the error
		}
		
		attacking = false;
	}
	public override string toString() { return "Fire Burst"; }
}

public class FireStormSpell : Attack
{
    public FireStormSpell() : base(6, 1, 2, 5) {}
    public override IEnumerator Execute(GameAgent attacker, Damageable target) 
	{
		//while (attacking) yield return null;
        if (attacker.stats.currentMagicPoints < 5)
        {
            yield break;
        }
        attacker.stats.currentMagicPoints = Mathf.Max((attacker.stats.currentMagicPoints - 5), 0);
        int count = attacker.stats.GetMultiHitCount();

        while (count > 0) {
            attacking = true;

            attacker.transform.LookAt((target as DungeonObject).transform);
            attacker.playAttackAnimation();
            attacker.playAttackNoise("Fire");

            while (attacker.animating) yield return null;

            Projectile fire = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "fire");

            while (!(fire == null)) yield return null;

            try {
                target.playHitAnimation();
                target.playHitNoise("Fire");
                target.take_damage((int)(attacker.stats.GetFireStormDamage() * damageModifier));
            } catch (Exception e) {
                // swallow the error
            }

            // if Damageable is dead, stop loop
            // implement this


            count--;
        }
        Debug.Log("After while loop");
		attacking = false;
    }
    public override string toString() { return "Fire Storm"; }
}

public class Berserk : Attack {
    public Berserk() : base(2, 1, 3, 3) { }
    public override IEnumerator Execute(GameAgent attacker, Damageable target) 
	{
		while (attacking) yield return null;
		
        if (attacker.stats.currentMagicPoints < 3) {
            yield break;
        }
        attacker.stats.currentMagicPoints = Mathf.Max((attacker.stats.currentMagicPoints - 3), 0);
        int count = attacker.stats.GetMultiHitCount();

        while (count > 0) {
            attacking = true;

            attacker.transform.LookAt((target as DungeonObject).transform);
            attacker.playAttackAnimation();
            attacker.playAttackNoise("Melee");

            Debug.Log("Waiting for animation to finish");
            while (attacker.animating) yield return null;

            try {
                target.playHitAnimation();
                target.playHitNoise("Melee");
                target.take_damage((int)(attacker.stats.GetBerserkDamage() * damageModifier));
            } catch (Exception e) {
                // swallow the error
            }
			
            count--;
        }
		attacking = false;
    }
    public override string toString() {
        return "Berserk";
    }
}

public class Multishot : Attack
{
    public Multishot() : base(7, 1, 1.25f, 4) { }
    public override IEnumerator Execute(GameAgent attacker, Damageable target)
    {
		while (attacking) yield return null;
        if (attacker.stats.currentMagicPoints < 4)
        {
            yield break;
        }
        attacker.stats.currentMagicPoints = Mathf.Max((attacker.stats.currentMagicPoints-4), 0);
        int count = attacker.stats.GetMultiHitCount();

        while (count > 0)
        {
            attacking = true;

            attacker.transform.LookAt((target as DungeonObject).transform);
            attacker.playAttackAnimation();
            attacker.playAttackNoise("Bow");

            while (attacker.animating) yield return null;

            Projectile arrow = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "arrow");

            while (!(arrow == null)) yield return null;

            try
            {
                target.playHitAnimation();
                target.playHitNoise("Bow");
                target.take_damage((int)(attacker.stats.DealDamage() * damageModifier));
            }
            catch (Exception e)
            {
                // swallow the error
            }

            // if Damageable is dead, stop loop
            // implement this


            count--;
        }
		attacking = false;
        Debug.Log("After while loop");
    }
    public override string toString() { return "Multishot"; }
}

public class LightningSpell : Attack
{
    public LightningSpell() : base(9, 1, 1.7f, 3) { }
    public override IEnumerator Execute(GameAgent attacker, Damageable target)
    {
		while (attacking) yield return null;
        if (attacker.stats.currentMagicPoints < 3)
        {
            yield break;
        }
        attacker.stats.currentMagicPoints = Mathf.Max((attacker.stats.currentMagicPoints - 3), 0);
        attacking = true;

        attacker.transform.LookAt((target as DungeonObject).transform);
        attacker.playAttackAnimation();
        attacker.playAttackNoise("Lightning");

        while (attacker.animating) yield return null;

        Projectile lightning = MapManager.AnimateProjectile(attacker.grid_pos, (target as DungeonObject).grid_pos, "lightning");

        while (!(lightning == null)) yield return null;

        try
        {
            target.playHitAnimation();
            target.playHitNoise("Fire");
            target.take_damage((int)(attacker.stats.DealDamage() * damageModifier));
        }
        catch (Exception e)
        {
            // swallow the error
        }

        attacking = false;
    }
    public override string toString() { return "Lightning"; }
}


/*public class Taunt : Attack
{

}
*/
