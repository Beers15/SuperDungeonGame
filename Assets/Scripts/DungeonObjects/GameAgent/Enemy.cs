using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using MapUtils;
using UnityEngine;

public class Enemy : GameAgent
{
    private MapManager map_manager; // reference to MapManager instance with map data

	private bool enemy_turn = false;
	
    //private CharacterClassDefiner classDefiner; // moved to GameAgent
	//temp attribute to make item drops from enemies appear interactible
	public Material interactTilesMaterial;
	public GameObject[] lootDrops;
	private int randomItem;

    [Header("Enemy Stats")]
    public float _attack;
    public float maxHealth;
    public float currentHealth;
    public float range;
    public float _speed;
    public float moveTime = 0.1f;
	public int moveBudget;
    public int level;
    public GameAgentState viewableState;

    //sound effects
    //private AudioSource source;
    //public AudioClip[] swordSwing;
    //public AudioClip[] axeSwing;
    //public AudioClip[] bowShot;
    //public AudioClip[] fireSpell;
    //public AudioClip[] footsteps;
	//public AudioClip[] deathRattle;
	//public AudioClip[] hitNoise;

	[FMODUnity.EventRef]
	public string sword;

	[FMODUnity.EventRef]
	public string bowShot;

	[FMODUnity.EventRef]
	public string daggerStab;

	[FMODUnity.EventRef]
	public string lightingAttack;

	[FMODUnity.EventRef]
	public string staffSwing;

	[FMODUnity.EventRef]
	public string clubSwing;

	[FMODUnity.EventRef]
	public string fireBurst;

	[FMODUnity.EventRef]
	public string fireStrom;

	[FMODUnity.EventRef]
	public string armorHit;

	[FMODUnity.EventRef]
	public string fleshHit;

	[FMODUnity.EventRef]
	public string deathNoise;

	[FMODUnity.EventRef]
	public string grunt;

	[FMODUnity.EventRef]
	public string footStep;


	FMOD.Studio.EventInstance soundevent;
	FMOD.Studio.EventInstance bowShotEvent;
	FMOD.Studio.EventInstance daggerStabEvent;
	FMOD.Studio.EventInstance lightingAttackEvent;
	FMOD.Studio.EventInstance staffSwingEvent;
	FMOD.Studio.EventInstance clubSwingEvent;
	FMOD.Studio.EventInstance fireBurstEvent;
	FMOD.Studio.EventInstance fireStromEvent;
	FMOD.Studio.EventInstance armorHitEvent;
	FMOD.Studio.EventInstance fleshHitEvent;
	FMOD.Studio.EventInstance deathNoiseEvent;
	FMOD.Studio.EventInstance gruntEvent;
	FMOD.Studio.EventInstance footStepEvent;

	void Update() {
		if (FogOfWar.IsVisible(map_manager.world_to_grid(transform.position))) {
			EnableRendering();
		}
		else {
			DisableRendering();
		}
	}

    public override void init_agent(Pos position, GameAgentStats stats, string name = null) {
        map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        grid_pos = position;
        animator = GetComponent<CharacterAnimator>();

        this.stats = stats;
        _attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;
		if (name == null)
		this.nickname = CharacterRaceOptions.getString(stats.characterRace) + " " + CharacterClassOptions.getWeaponDescriptor(stats.playerCharacterClass.weapon);
		else this.nickname = name;
		
		weapon = stats.playerCharacterClass.weapon;
		
		speed = 10;
		move_budget = 10;
		
        level = stats.level;
        viewableState = stats.currentState;

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);

		//source = GetComponent<AudioSource>();

		soundevent = FMODUnity.RuntimeManager.CreateInstance(sword);
		bowShotEvent = FMODUnity.RuntimeManager.CreateInstance(bowShot);
		daggerStabEvent = FMODUnity.RuntimeManager.CreateInstance(daggerStab);
		lightingAttackEvent = FMODUnity.RuntimeManager.CreateInstance(lightingAttack);
		staffSwingEvent = FMODUnity.RuntimeManager.CreateInstance(staffSwing);
		clubSwingEvent = FMODUnity.RuntimeManager.CreateInstance(clubSwing);
		fireBurstEvent = FMODUnity.RuntimeManager.CreateInstance(fireBurst);
		fireStromEvent = FMODUnity.RuntimeManager.CreateInstance(fireStrom);
		armorHitEvent = FMODUnity.RuntimeManager.CreateInstance(armorHit);
		fleshHitEvent = FMODUnity.RuntimeManager.CreateInstance(fleshHit);
		deathNoiseEvent = FMODUnity.RuntimeManager.CreateInstance(deathNoise);
		gruntEvent = FMODUnity.RuntimeManager.CreateInstance(grunt);
		footStepEvent = FMODUnity.RuntimeManager.CreateInstance(footStep);

		// AI init
		team = 1;
		AI = new AIComponent(this); // AI component that decides the actions for this enemy to take
		TurnManager.instance.addToRoster(this);
		SetCurrentAction(0);
    }

	private bool moving = false;
    public override IEnumerator smooth_movement(Path path) {
		//Debug.Log("started...");
		grid_pos = path.endPos();
		if (!FogOfWar.IsVisible(grid_pos)) {
			transform.position = map_manager.grid_to_world(grid_pos);
			yield break;
		}
		
        moving = true;
        StartCoroutine(animator.StartMovementAnimation());

        //source.PlayOneShot(footsteps);

			Vector3 origin, target;
			foreach(Pos step in path.getPositions()) {

				origin = transform.position;
				target = map_manager.grid_to_world(step);
				float dist = Vector3.Distance(origin, target);
				float time = 0f;

				transform.LookAt(target);

					while(time < 1f && dist > 0f) {
						time += (Time.deltaTime * speed) / dist;
						transform.position = Vector3.Lerp(origin, target, time);
						yield return null;
					}
			}
			transform.position = map_manager.grid_to_world(path.endPos());

        StartCoroutine(animator.StopMovementAnimation());
        moving = false;
		//Debug.Log("ended...");
    }
	
	public override void attack(Damageable target) {
		//Debug.Log(currentAttack);
		StartCoroutine(currentAttack.Execute(this, target));
	}
	
    public void Hit() { animating = false;}
    public void Shoot() { animating = false;}
	
	public override void playAttackAnimation() {
		animating = true;
		StartCoroutine(animator.PlayAttackAnimation());		
	}
	
	public override void playHitAnimation() {
		StartCoroutine(animator.PlayHitAnimation());
	}
	
	public override void playAttackNoise(string type) {
		switch (type) {
			case "Melee":
			switch (weapon) {
				case 1:
						//source.PlayOneShot(randomSFX(swordSwing));
						soundevent.start();

						break;
				case 2:
						//source.PlayOneShot(randomSFX(bowShot));
						bowShotEvent.start();
						break;
				case 3:
						//source.PlayOneShot(randomSFX(fireSpell));
						fireBurstEvent.start();
						break;
				default:
						//source.PlayOneShot(randomSFX(axeSwing));
						daggerStabEvent.start();
						break;
			}
			break;
		}
	}
	
	// TODO: once we have more hit noises, switch based on type of projectile/weapon we are hit by
	public override void playHitNoise(string type) {
		switch (type) {
			default:
				//source.PlayOneShot(randomSFX(hitNoise));
				gruntEvent.start();
				armorHitEvent.start();
				break;
		}
	}
	
	public override bool animationFinished() {
		//Debug.Log(!currentAttack.attacking + ", " + !moving);
		return (!currentAttack.attacking) && !moving;
	}
	
    public override void take_damage(int amount, int classOfAttacker, GameAgent attacker)  {	
        stats.TakeDamage(amount);
		//Debug.Log("This mob's lvl is: " +level.ToString());

        if (stats.currentState == GameAgentState.Unconscious) {
			//give attacker gold (points) for the kill based on enemy level
			attacker.inventory.AddItem(new Item(9999999, "gold", "99", (level * 50)));

            StartCoroutine(animator.PlayKilledAimation());
            stats.currentState = GameAgentState.Dead;

			GameManager.kill(this);
			deathNoiseEvent.start();

			if (team == 1) {
				var lootRoll = 0.0f;
				var lootThreshold = 1.0f;
				var mobDifficultyModifier = 1.0f / (10.0f - Convert.ToSingle(level));

				if(mobDifficultyModifier <= 0.1f)
					lootRoll = lootThreshold + 1.0f;
				else
					lootRoll = Settings.globalRNG.Next(1, (int)(100 * (mobDifficultyModifier)));
					Debug.Log("Loot roll is " + lootRoll + " (>" + lootThreshold + " equals item drop) ");

				if(lootRoll > lootThreshold) {
					//chest more common than not since they spawn a majority of the item types
					if(Settings.globalRNG.Next(0, 8) % 2 == 0)
						randomItem = 0;
					else 
						randomItem = Settings.globalRNG.Next(0, lootDrops.Length - 1);

					var spawnedDrop = map_manager.instantiate_environment(lootDrops[randomItem], new Pos(this.grid_pos.x, this.grid_pos.y), true, true);
					if(randomItem == 0) {
						(spawnedDrop.GetComponent<DungeonObject>() as RandomItemsChest).setLvlOfSlainMob(level);	
						(spawnedDrop.GetComponent<DungeonObject>() as RandomItemsChest).setClassOfAttacker(classOfAttacker);
						Debug.Log("CLASS OF ATTACKER " + (spawnedDrop.GetComponent<DungeonObject>() as RandomItemsChest).getClassOfAttacker());
					}		
					else
						(spawnedDrop.GetComponent<DungeonObject>() as RandomItemsSpawner).setLvlOfSlainMob(level);
				}
			}
		}
		
        currentHealth = stats.currentHealth;
    }
	
	/*private IEnumerator wait_to_reset_position()
	{
		Vector3 pos = transform.position;
		yield return new WaitForSeconds(1f);
		transform.position = pos;
	}*/

    public override void GetHealed(int amount) 
	{
        if (stats.currentState == GameAgentState.Alive) {
            stats.GetHealed(amount);

            StartCoroutine(animator.PlayUseItemAnimation());
        }
        currentHealth = stats.currentHealth;
    }

    public override void take_turn() 
	{	
		StartCoroutine(AI.advance());
    }
	
	public override bool turn_over() {
		return AI.finished;
	}

    public override void move() {
    }

    public override void act() {
    }

    public override void wait() {
    }

    public override void potion() {
    }

    public void FootR() {
		//source.PlayOneShot(randomSFX(footsteps));
		footStepEvent.start();
	}
    public void FootL() {
		//source.PlayOneShot(randomSFX(footsteps));
		footStepEvent.start();
	}
    public void WeaponSwitch() { }
	
	private static int nextSFX = 0;
	private AudioClip randomSFX(AudioClip[] library)
	{
		return library[nextSFX++%library.Length];
	}
}
