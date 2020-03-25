using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.EnumUtils;

public class Player : GameAgent
{	
	private MapManager map_manager; // reference to MapManager instance with map data
    private MapConfiguration config;
	private TileSelector tile_selector; // reference to map tile selector
    private List<Pos> selectableTiles;
	
	public bool godMode = false;

    // player turn options
    private bool playerMovedThisTurn = false;
    private bool playerActedThisTurn = false;
    private bool playerUsedPotionThisTurn = false;
    private bool playerWaitingThisTurn = false;
	private bool playerExtracted = false;

    [Header("Player Stats")]
    public string name;
    public float _attack;
    public float maxHealth;
    public float currentHealth;
    public float range;
    public float _speed;
    public int level;
    public string viewableState;

    // 0 - unarmed, 1 - sword, 2 - bow, 3 - staff
    public int weapon = 1;

    //CharacterClassDefiner classDefiner; // moved to GameAgent

    // Get rid of this when you get rid of using keys to change player class
    List<Player> playersForTestingPurposes;

    //sound effects
    private AudioSource source;
    public AudioClip[] swordSwing;
    public AudioClip[] axeSwing;
    public AudioClip[] bowShot;
    public AudioClip[] fireSpell;
    public AudioClip lightningSpell;
    public AudioClip[] footsteps;
	public AudioClip[] deathRattle;
	public AudioClip[] hitNoise;
	public AudioClip[] armorHitNoise;
	
	private int max_move_budget;
	
    // Gets references to necessary game components
    public override void init_agent(Pos position, GameAgentStats stats, string name = null)
    {
		map_manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapManager>();
        grid_pos = position;

        this.stats = stats;
        UpdateViewableEditorPlayerStats();
		max_move_budget = 15;
		move_budget = max_move_budget;
		speed = 10;
		this.nickname = name;

        animator = GetComponent<CharacterAnimator>();
        classDefiner = GetComponent<CharacterClassDefiner>();
        animator.init();
        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);

        selectableTiles = new List<Pos>();

        currentState = GameAgentState.Alive;
		animating = false;

        source = GetComponent<AudioSource>();
		inventory.AddItem(new HealthPot(5));
		inventory.AddItem(new ManaPot(5));

        // AI init
        team = 0;
		AI = null; // players don't have AI
		TurnManager.instance.addToRoster(this); //add player to player list
    }
	
	public void re_init(Pos position)
	{
		grid_pos = position;
		playerExtracted = false;
		playerMovedThisTurn = false;
		playerActedThisTurn = false;
		playerUsedPotionThisTurn = false;
		playerWaitingThisTurn = false;
		TurnManager.instance.addToRoster(this); //add player to player list
		EnableRendering();
	}
	
	private bool moving = false;
    public override IEnumerator smooth_movement(Path path)
	{
		if (path.getPositions() == null) yield break;
		while (moving) yield return null; // wait for any previous movement to finish
		moving = true;
		move_budget -= path.distance();

        StartCoroutine(animator.StartMovementAnimation());
        //source.PlayOneShot(footsteps);
			Vector3 origin, target;
			foreach(Pos step in path.getPositions()) {
				grid_pos = step;

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
		grid_pos = path.endPos();
		
        playerMovedThisTurn = true;
	}

	public override void attack(Damageable target)
	{
        if (stats.currentMagicPoints >= currentAttack.MPcost)
        {
            animating = true;
            StartCoroutine(currentAttack.Execute(this, target));
            StartCoroutine(waitForAttackEnd());
        }
	}
	
	private IEnumerator waitForAttackEnd()
	{
		while (currentAttack.attacking) yield return null;
		playerActedThisTurn = true;
	}
	
	public void Hit(){ animating = false; }
	public void Shoot(){ animating = false; }
	
	public override void playAttackAnimation()
	{
		StartCoroutine(animator.PlayAttackAnimation());
	}
	
	public override void playHitAnimation()
	{
		StartCoroutine(animator.PlayHitAnimation());
	}
	
	public override void playAttackNoise(string type)
	{
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
            case "Lightning":
                source.PlayOneShot(lightningSpell);
                break;
            
		}
	}
	// TODO: once we have more hit noises, switch based on type of projectile/weapon we are hit by
	public override void playHitNoise(string type)
	{
		switch (type) {
			default:
			source.PlayOneShot(randomSFX(hitNoise));
			source.PlayOneShot(randomSFX(armorHitNoise));
			break;
		}
	}
	
	public override bool animationFinished()
	{
		return (currentAttack == null || !currentAttack.attacking) && !moving;
	}
	
	public override void take_damage(int amount)
	{
        if (stats.currentState == GameAgentState.Alive) {
            if (!godMode) stats.TakeDamage((int)(amount * 0.05));
            //if (!godMode) stats.TakeDamage(amount);

            if (stats.currentState == GameAgentState.Unconscious) {
                StartCoroutine(animator.PlayKilledAimation());
				GameManager.kill(this);
			}
        }

        UpdateViewableEditorPlayerStats();
    }
	
	/*private IEnumerator slide_to_position()
	{
		Vector3 originalPos = transform.position;
		yield return new WaitForSeconds(1f);
		transform.position = pos;
	}*/

    public override void GetHealed(int amount) {
        if (stats.currentState == GameAgentState.Alive) {
            if (!godMode) stats.GetHealed(amount);

            StartCoroutine(animator.PlayUseItemAnimation());
        }

        UpdateViewableEditorPlayerStats();
    }

    public override void take_turn()
	{
        if (stats.currentState == GameAgentState.Alive) {
            playerMovedThisTurn = false;
            playerActedThisTurn = false;
            playerUsedPotionThisTurn = false;
            playerWaitingThisTurn = false;
			move_budget = max_move_budget;
        }

        UpdateViewableEditorPlayerStats();
    }

    private void UpdateViewableEditorPlayerStats() {
        _attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = stats.currentHealth;
        range = stats.range;
        _speed = stats.speed;
        level = stats.level;

        switch (stats.currentState) {
            case GameAgentState.Alive:
                viewableState = "Alive";
                break;
            case GameAgentState.Unconscious:
                viewableState = "Unconscious";
                break;
            case GameAgentState.Dead:
                viewableState = "Dead";
                break;
        }
    }

	/*** UNUSED ANIMATION RECEIVERS ***/
	public void FootR(){
        source.PlayOneShot(randomSFX(footsteps));
    }
	public void FootL(){
        source.PlayOneShot(randomSFX(footsteps));
    }
	public void WeaponSwitch(){}
	/*** END ANIMATION RECEIVERS ***/
	
	public string[] getActionNames()
	{
		List<string> actionNames = new List<string>();
		foreach (Attack act in stats.playerCharacterClass.GetAvailableActs())
			actionNames.Add(act.toString()); // GetString() defined in MapUtils.EnumUtils
		return actionNames.ToArray();
	}
	
	public override void wait() { playerWaitingThisTurn = true; }
	public override void potion() { playerUsedPotionThisTurn = true; }
	public override void move() { playerMovedThisTurn = true; }
	public override void act() { playerActedThisTurn = true; }
	public override bool turn_over() {
		return playerWaitingThisTurn || playerActedThisTurn || playerUsedPotionThisTurn || playerExtracted;
    }
	public void extract() {
		playerExtracted = true;
		DisableRendering();
		TurnManager.instance.removeFromRoster(this);
	}
	
	public bool can_take_action() { return !playerExtracted && animationFinished() && !turn_over() && Network.allPlayersReady(); }
	
	public void SetCharacterClass(string classname) {
		
        int weapon, classID;
        switch (classname) {
			case "Warrior":
				classID = CharacterClassOptions.Knight;
				weapon = CharacterClassOptions.Sword;
				break;
			case "Mage":
				classID = CharacterClassOptions.Mage;
				weapon = CharacterClassOptions.Staff;
				break;
			case "Hunter":
				classID = CharacterClassOptions.Hunter;
				weapon = CharacterClassOptions.Bow;
				break;
			case "Healer":
				classID = CharacterClassOptions.Healer;
				weapon = CharacterClassOptions.Staff;
				break;
			default:
				classID = CharacterClassOptions.Knight;
				weapon = CharacterClassOptions.Sword;
				break;
		}

        stats = new GameAgentStats(CharacterRaceOptions.Human, classID, 1, weapon);
        _attack = stats.attack;
        maxHealth = stats.maxHealth;
        currentHealth = maxHealth;
        range = stats.range;
        _speed = stats.speed;

        classDefiner.init(stats.characterRace, stats.characterClassOption, stats.playerCharacterClass.weapon);
    }
	
	private static int nextSFX = 0;
	private AudioClip randomSFX(AudioClip[] library)
	{
		return library[nextSFX++%library.Length];
	}
	
	
	// VVVVVVVVVVVVVVVVV CODE JAIL VVVVVVVVVVVVVVVVVV //
	// 			INTRUDERS WILL BE EXECUTED			  //
	
	
	/*public void RespondToKeyboardInput(char key)
	{
		switch (key) {
		    case '1': StartCoroutine(animator.PlayRotateAnimation()); break;
		    case '2': StartCoroutine(animator.PlayAttackAnimation()); break;
		    case '3': StartCoroutine(animator.PlayUseItemAnimation()); break;
		    case '4': StartCoroutine(animator.PlayHitAnimation()); break;
		    case '5': StartCoroutine(animator.PlayBlockAnimation()); break;
		    case '6': StartCoroutine(animator.PlayKilledAimation()); break;
            /*case 'a': TestCharacterClass(CharacterClassOptions.Knight); break;
            case 's': TestCharacterClass(CharacterClassOptions.Hunter); break;
            case 'd': TestCharacterClass(CharacterClassOptions.Mage); break;
            case 'f': TestCharacterClass(CharacterClassOptions.Healer); break;*/
        /*}
    }*/
	
	// disabling this for now while I test changes
	/*public string getActionMode(int action)
	{
		GameAgentAction[] actions = stats.playerCharacterClass.GetAvailableActs();
		return actions[action].GetMode(); // mode can be ACT or AOE
	}*/
	
	// a lot of the WaitForXXX functions seemed redundant... we can add this functionality back later if necessary
	/*IEnumerator WaitForAttackEnd(Pos attackPos)
	{
		isAttacking = true;
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        this.transform.LookAt(map_manager.GetUnitTransform(attackPos));

        while (isAttacking) yield return null;
        map_manager.attack(attackPos, stats.DealDamage());

        playerActedThisTurn = true;
    }

    IEnumerator WaitForRangedAttackMultiShotEnd(Pos attackPos, int multiShotCount) {
        isAttacking = true;
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        this.transform.LookAt(map_manager.GetUnitTransform(attackPos));

        while (isAttacking) yield return null;
        map_manager.attack(this, attackPos, stats.GetMultiShotDamage());

        // Stop attacking if target is dead
        if (map_manager.GetGameAgentState(attackPos) != GameAgentState.Alive) {
            multiShotCount = 0;
        }

        if (multiShotCount > 0) {
            while (animator.AnimatorIsPlaying()) yield return null;
            StartCoroutine(animator.PlayAttackAnimation());
            StartCoroutine(WaitForRangedAttackMultiShotEnd(attackPos, --multiShotCount));
        } else {
            playerActedThisTurn = true;
        }

        //PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.ACT);
    }

    IEnumerator WaitForTauntEnd() {
        isAttacking = true;
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        Transform lookDirection = map_manager.GetNearestUnitTransform(grid_pos, tile_selector.GetPositionOfAgentsInNonselectableActTiles());
        if (lookDirection != null) {
            this.transform.LookAt(lookDirection);
        }
         //&& map_manager.GetGameAgentState(tile_selector.grid_position) == GameAgentState.Alive
        while (animator.AnimatorIsPlaying()) yield return null;
        isAttacking = false;

        playerActedThisTurn = true;
    }

    IEnumerator WaitForAOEEnd(List<Pos> targetTiles) {
        // Have player look at the target it's attacking
        // Consider making this a smooth movement
        Transform lookDirection = map_manager.GetNearestUnitTransform(grid_pos, tile_selector.GetPositionOfAgentsInActAOETiles());
        if (lookDirection != null) {
            this.transform.LookAt(lookDirection);
        }

        if (currentAction == GameAgentAction.MagicAttackAOE) {
            isAttacking = true;
            while (isAttacking) yield return null;
            foreach (Pos tile in targetTiles) {
                map_manager.attack(tile, stats.DealDamage());
            }
        } else if (currentAction == GameAgentAction.Heal) {
            isAttacking = true;
            while (isAttacking) yield return null;
            foreach (Pos tile in targetTiles) {
                map_manager.GetHealed(tile, stats.GetHealAmount());
            }
        }

        playerActedThisTurn = true;
    }*/

	// if right mouse button is pressed, move player model to hover position
    /*public void RespondToMouseClick()
    {
		if (!moving && !isAttacking && hoveringActionTileSelector) {
			switch (currentAction) {
			    case GameAgentAction.Move:
				    if ((tile_selector.hoveringValidMoveTile() || godMode) && map_manager.move(grid_pos, tile_selector.grid_position)) {

                        hoveringActionTileSelector = false;
                        tile_selector.showSelectableMoveTiles = false;
                        tile_selector.showPathLine = false;
                    }
				    break;
			    case GameAgentAction.MeleeAttack:
                case GameAgentAction.MagicAttackSingleTarget:
                case GameAgentAction.RangedAttack:
				    if ((tile_selector.hoveringValidSelectTile() || godMode) && map_manager.IsOccupied(tile_selector.grid_position) && map_manager.GetGameAgentState(tile_selector.grid_position) == GameAgentState.Alive) {

					    Pos attackPos = tile_selector.grid_position;
                        StartCoroutine(animator.PlayAttackAnimation());
					    StartCoroutine(WaitForAttackEnd(attackPos));
                    }
                    break;
                case GameAgentAction.RangedAttackMultiShot:
                    if ((tile_selector.hoveringValidSelectTile() || godMode) && map_manager.IsOccupied(tile_selector.grid_position) && map_manager.GetGameAgentState(tile_selector.grid_position) == GameAgentState.Alive) {

                        Pos attackPos = tile_selector.grid_position;
                        StartCoroutine(animator.PlayAttackAnimation());
                        StartCoroutine(WaitForRangedAttackMultiShotEnd(attackPos, stats.GetMultiShotCount()));
                    }
                    break;
                case GameAgentAction.Taunt:
                    StartCoroutine(animator.PlayTauntAnimation());
                    StartCoroutine(WaitForTauntEnd());
                    break;
                case GameAgentAction.MagicAttackAOE:
                case GameAgentAction.Heal:
                    if (tile_selector.hoveringValidActAOETile() || godMode) {

                        Pos attackPos = tile_selector.grid_position;
                        StartCoroutine(animator.PlayAttackAnimation());
                        StartCoroutine(WaitForAOEEnd(tile_selector.GetPositionOfAgentsInActAOETiles()));
                    }
                    break;
            }
		}
	}*/
	
    /*public override void move() {
		if (playerMovedThisTurn || turn_over())
            return;
		
        // Hide move selection if open
        if (tile_selector.showSelectableMoveTiles) {
            TurnOffSelectors();
            return;
        }

        if (playerMovedThisTurn || turn_over() || stats.currentState != GameAgentState.Alive)
            return;

        currentAction = GameAgentAction.Move;
		tile_selector.CreateListOfSelectableMovementTiles(grid_pos, (int)stats.speed);//, currentAction);

        hoveringActionTileSelector = true;
        tile_selector.showPathLine = true;
        tile_selector.showSelectableMoveTiles = true;
    }

    public override void act() {
        // Hide move selection if open
        if (tile_selector.showSelectableMoveTiles) {
            TurnOffSelectors();
        }

        if (turn_over() || playerActedThisTurn || stats.currentState != GameAgentState.Alive)
            return;

        PlayerActMenu.SetPlayerActMenuActive(true, stats.playerCharacterClass.GetAvailableActs());
    }

    public void action1() {
        // Stop showing action2
        if (currentAction == GameAgentAction.Heal || currentAction == GameAgentAction.MagicAttackAOE
            || currentAction == GameAgentAction.Taunt || currentAction == GameAgentAction.RangedAttackMultiShot) {
            TurnOffSelectors();
        // stop showing action1
        } else if ((currentAction == GameAgentAction.MeleeAttack || currentAction == GameAgentAction.MagicAttackSingleTarget
            || currentAction == GameAgentAction.RangedAttack) && tile_selector.showSelectableActTiles) {
            TurnOffSelectors();
            return;
        }

        if (stats.playerCharacterClass.GetAvailableActs().Length >= 1) {
            currentAction = (stats.playerCharacterClass.GetAvailableActs())[0];
        }

        if (currentAction == GameAgentAction.MeleeAttack || currentAction == GameAgentAction.MagicAttackSingleTarget
            || currentAction == GameAgentAction.RangedAttack) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range);//, currentAction);

            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
            tile_selector.showAOETiles = false;
        }
    }

    public void action2() {
		if (turn_over())
            return;
		
        // stop showing action1
        if (currentAction == GameAgentAction.MeleeAttack || currentAction == GameAgentAction.MagicAttackSingleTarget
            || currentAction == GameAgentAction.RangedAttack) {
            TurnOffSelectors();
        // stop showing action2
        } else if ((currentAction == GameAgentAction.Heal || currentAction == GameAgentAction.MagicAttackAOE
            || currentAction == GameAgentAction.Taunt || currentAction == GameAgentAction.RangedAttackMultiShot)
            && tile_selector.showSelectableActTiles) {
            TurnOffSelectors();
            return;
        }

        if (stats.playerCharacterClass.GetAvailableActs().Length >= 2) {
            currentAction = (stats.playerCharacterClass.GetAvailableActs())[1];
        }
        if (currentAction == GameAgentAction.Heal || currentAction == GameAgentAction.MagicAttackAOE) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range);//, currentAction);
            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
            tile_selector.showAOETiles = true;
        }

        if (currentAction == GameAgentAction.Taunt || currentAction == GameAgentAction.RangedAttackMultiShot) {
            tile_selector.CreateListOfSelectableActTiles(grid_pos, (int)stats.range);//, currentAction);
            hoveringActionTileSelector = true;
            tile_selector.showSelectableMoveTiles = false;
            tile_selector.showSelectableActTiles = true;
        }
    }

    public void action3() {

    }

    public void action4() {
        // Return to the battle menu
        if (PlayerActMenu.IsPlayerActMenuActive()) {
            PlayerActMenu.SetPlayerActMenuActive(false);
            TurnOffSelectors();
        }
    }

    public override void wait() {
        if (turn_over() || playerWaitingThisTurn || stats.currentState != GameAgentState.Alive)
            return;
		
        currentAction = GameAgentAction.Wait;

        PlayerActMenu.SetPlayerActMenuActive(false);
        TurnOffSelectors();

        playerWaitingThisTurn = true;
    }

    public override void potion() {

        if (turn_over() || playerUsedPotionThisTurn || stats.currentState != GameAgentState.Alive)
            return;
        PlayerActMenu.SetPlayerActMenuActive(false);
        TurnOffSelectors();

        currentAction = GameAgentAction.Potion;
        StartCoroutine(animator.PlayUseItemAnimation());
        stats.UsePotion();
        playerUsedPotionThisTurn = true;

        UpdateViewableEditorPlayerStats();
		PlayerActMenu.MakeButtonNoninteractable(ActMenuButtons.POTION);
    }

    public void UpdatePlayerStatsMenu(List<Player> players) {
        // Get rid of this when you get rid of using keys to change player class
        playersForTestingPurposes = players;

        int[] sortedPlayersIndex = SortPlayerListAlphabetically(players);
        for (int i = 0; i < sortedPlayersIndex.Length; i++) {
            PlayerActMenu.UpdatePlayerStatsMenu(i, players[sortedPlayersIndex[i]].name, players[sortedPlayersIndex[i]].stats);
        }

        // Deactivate the other nonactive players
        for (int i = sortedPlayersIndex.Length; i < 4; i++ ) {
            PlayerActMenu.UpdatePlayerStatsMenu(i, "", null, true);
        }
    }

    // returns an array of alphabetically arranged player indexes based on player.name
    private int[] SortPlayerListAlphabetically(List<Player> players) {
        var playerNames = new List<string>();
        int[] sortedPlayersIndex = new int[players.Count];
        int playerNameIndex = 0;

        foreach (Player player in players) {
            playerNames.Add(player.name);
        }

        // sorts alphabetically
        playerNames.Sort();

        // get a list of sorted alphabetical indexs
        for (int i=0; i < playerNames.Count; i++) {
            for (int j=0; j < players.Count; j++) {
                if (playerNames[i] == players[j].name) {
                    if (name == players[j].name) {
                        playerNameIndex = i;
                    }
                    sortedPlayersIndex[i] = j;
                    break;
                }
            }
        }

        // Move this player to the first spot
        if (playerNameIndex != 0) {
            int temp = sortedPlayersIndex[0];
            sortedPlayersIndex[0] = sortedPlayersIndex[playerNameIndex];
            sortedPlayersIndex[playerNameIndex] = temp;
        }

        return sortedPlayersIndex;
    }*/

   
}
