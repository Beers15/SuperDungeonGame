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
	private int weapon;

    //sound effects
    private AudioSource source;
    public AudioClip[] swordSwing;
    public AudioClip[] axeSwing;
    public AudioClip[] bowShot;
    public AudioClip[] fireSpell;
    public AudioClip[] footsteps;
	public AudioClip[] deathRattle;
	public AudioClip[] hitNoise;
	
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

        source = GetComponent<AudioSource>();

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
		Debug.Log(currentAttack);
		StartCoroutine(currentAttack.Execute(this, target));
	}
	
    public void Hit() { animating = false; Debug.Log("Just set animating to false"); }
    public void Shoot() { animating = false; Debug.Log("Just set animating to false"); }
	
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
					source.PlayOneShot(randomSFX(swordSwing));
					break;
				case 2:
					source.PlayOneShot(randomSFX(bowShot));
					break;
				case 3:
					source.PlayOneShot(randomSFX(fireSpell));
					break;
				default:
					source.PlayOneShot(randomSFX(axeSwing));
					break;
			}
			break;
		}
	}
	
	// TODO: once we have more hit noises, switch based on type of projectile/weapon we are hit by
	public override void playHitNoise(string type) {
		switch (type) {
			default:
			source.PlayOneShot(randomSFX(hitNoise));
			break;
		}
	}
	
	public override bool animationFinished() {
		Debug.Log(!currentAttack.attacking + ", " + !moving);
		return (!currentAttack.attacking) && !moving;
	}
	
    public override void take_damage(int amount)  {	
        stats.TakeDamage(amount);
		Debug.Log("This mob's lvl is: " +level.ToString());

        if (stats.currentState == GameAgentState.Unconscious) {
            StartCoroutine(animator.PlayKilledAimation());
            stats.currentState = GameAgentState.Dead;

			GameManager.kill(this);

			//death and item drop code logic----------------
			//team 1 equals enemy mob
			if(team == 1) {
				var lootRoll = 0.0f;
				var lootThreshold = 1.0f;
				var mobDifficultyModifier = 1.0f / (10.0f - Convert.ToSingle(level));

				if(mobDifficultyModifier <= 0.1f)
					lootRoll = lootThreshold + 1.0f;
				else
					lootRoll = UnityEngine.Random.Range(1.0f, 100.0f * (mobDifficultyModifier));
				Debug.Log("Loot roll is " + lootRoll + " (>" + lootThreshold + " equals item drop) ");

				if(lootRoll > lootThreshold) {
					randomItem = UnityEngine.Random.Range(0, lootDrops.Length - 1);

					//check enemy prefab if changes are made to ensure lootDrops[0] is set to the itemChest prefab
					//pass on to the item object's item script the lvl of the mob that was slain
					if(randomItem == 0)
						(lootDrops[randomItem].GetComponent<DungeonObject>() as RandomItemsChest).setLvlOfSlainMob(level);
					else
						(lootDrops[randomItem].GetComponent<DungeonObject>() as RandomItemsSpawner).setLvlOfSlainMob(level);

					map_manager.instantiate_environment(lootDrops[randomItem], new Pos(this.grid_pos.x, this.grid_pos.y), true, true);
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
        source.PlayOneShot(randomSFX(footsteps));
    }
    public void FootL() {
        source.PlayOneShot(randomSFX(footsteps));
    }
    public void WeaponSwitch() { }
	
	private static int nextSFX = 0;
	private AudioClip randomSFX(AudioClip[] library)
	{
		return library[nextSFX++%library.Length];
	}
}
