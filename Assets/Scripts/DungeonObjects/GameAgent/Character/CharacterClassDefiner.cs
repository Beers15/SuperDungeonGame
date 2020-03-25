using UnityEngine;

public class CharacterClassDefiner : MonoBehaviour
{
    #region Variables
    // Referenced conponents.    
    GameAgent character;
    Animator animator;
    Transform characterAvatar;

    // Enemy variation variables. (Do not change)
    int minOrcRange = 4;
    int maxOrcRange = 10;
    int minSkeletonRange = 12;
    int maxSkeletonRange = 17;

    // Weapon variation variables.
    public GameObject activeWeapon;
	public int weaponNum;

    // Weapon objects.
    public GameObject sword;
    public GameObject bow;
    public GameObject staff;
    public GameObject axe;
    public GameObject club;

    private System.Random rng;
    #endregion

    public void init(int characterRace, int characterClass, int characterWeapon) {
        // Get required components.
        character = GetComponent<GameAgent>();
        animator = GetComponent<Animator>();

        MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
        rng = config.GetRNG();

        // Hide all weapon objects.
        hideAllWeapons();

        if (characterClass > 0) {
            SetCharacterClass(characterRace, characterClass, characterWeapon);
        }
    }
	
	public void DisableRendering()
	{
		if (activeWeapon != null)
			activeWeapon.SetActive(false);
		characterAvatar.gameObject.SetActive(false);
	}
	
	public void EnableRendering()
	{
		if (activeWeapon != null)
			activeWeapon.SetActive(true);
		characterAvatar.gameObject.SetActive(true);
	}

    // SetCharacterClass(int characterID), SetCharacterModel(int modelID), SetCharacterWeapon(int weaponID)
    #region Main Methods
    public void SetCharacterClass(int characterRace, int characterClass, int characterWeapon)
    {
        int characterModel = 1;

        switch (characterClass) {
            case CharacterClassOptions.Knight:
                switch (characterRace) {
                    case CharacterRaceOptions.Human:
                        characterModel = CharacterClassOptions.Knight;
                        break;
                    case CharacterRaceOptions.Orc:
                        characterModel = CharacterRaceOptions.Orc;
                        break;
                    case CharacterRaceOptions.Skeleton:
                        characterModel = CharacterRaceOptions.Skeleton;
                        break;
                }
                break;
            case CharacterClassOptions.Hunter:
                switch (characterRace) {
                    case CharacterRaceOptions.Human:
                        characterModel = CharacterClassOptions.Hunter;
                        break;
                    case CharacterRaceOptions.Orc:
                        characterModel = CharacterRaceOptions.Orc;
                        break;
                    case CharacterRaceOptions.Skeleton:
                        characterModel = CharacterRaceOptions.Skeleton;
                        break;
                }
                break;
            case CharacterClassOptions.Mage:
                switch (characterRace) {
                    case CharacterRaceOptions.Human:
                        characterModel = CharacterClassOptions.Mage;
                        break;
                    case CharacterRaceOptions.Orc:
                        characterModel = CharacterRaceOptions.Orc;
                        break;
                    case CharacterRaceOptions.Skeleton:
                        characterModel = CharacterRaceOptions.Skeleton;
                        break;
                }
                break;
            case CharacterClassOptions.Healer:
                switch (characterRace) {
                    case CharacterRaceOptions.Human:
                        characterModel = CharacterClassOptions.Healer;
                        break;
                    case CharacterRaceOptions.Orc:
                        characterModel = CharacterRaceOptions.Orc;
                        break;
                    case CharacterRaceOptions.Skeleton:
                        characterModel = CharacterRaceOptions.Skeleton;
                        break;
                }
                break;

        }
		
		weaponNum = characterWeapon;

        SetCharacterModel(characterModel);
        SetCharacterWeapon(characterWeapon);
    }

    public void SetCharacterModel(int modelID)
    {
        Transform currentCharacterAvatar = transform.GetChild(GetActiveChracterModel());
        currentCharacterAvatar.gameObject.SetActive(false);

        if (modelID == 1) characterAvatar = transform.Find("Chr_Dungeon_KnightMale_01");
        else if (modelID == 2) characterAvatar = transform.Find("Chr_Vikings_ShieldMaiden_01");
        else if (modelID == 3) characterAvatar = transform.Find("Chr_Fantasy_Wizard_01");
        else if (modelID == 4) characterAvatar = transform.GetChild(GetRandomOrc());
        else if (modelID == 5) characterAvatar = transform.GetChild(GetRandomSkeleton());
        else characterAvatar = transform.Find("Chr_Western_Woman_01");

        characterAvatar.gameObject.SetActive(true);
    }

    public void SetCharacterWeapon(int weaponID)
    {
        hideAllWeapons();

        if (weaponID == CharacterClassOptions.Sword) // Sword
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Sword);
            sword.SetActive(true);
			activeWeapon = sword;
        }
        else if (weaponID == CharacterClassOptions.Bow) // Bow
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Bow);
            bow.SetActive(true);
			activeWeapon = bow;
        }
        else if (weaponID == CharacterClassOptions.Staff) // Staff
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Staff);
            staff.SetActive(true);
			activeWeapon = staff;
        }
        else if (weaponID == CharacterClassOptions.Axe) // Axe
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Axe);
            axe.SetActive(true);
			activeWeapon = axe;
        }
        else if (weaponID == CharacterClassOptions.Club) // Club
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Club);
            club.SetActive(true);
			activeWeapon = club;
        } 
        else if (weaponID == CharacterClassOptions.Unarmed) // Staff
        {
            animator.SetInteger("Weapon", CharacterClassOptions.Unarmed);
			activeWeapon = null;
        }

        animator.SetTrigger("InstantSwitchTrigger");
    }
    #endregion

    // GetActiveChracterModel(), GetRandomOrc(), GetRandomSkeleton(), hideAllWeapons()
    #region Helper Methods
    int GetActiveChracterModel()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.active) return i;
        }
        return -1;
    }

    int GetRandomOrc()
    {
        return rng.Next(minOrcRange, maxOrcRange);
    }

    int GetRandomSkeleton()
    {
        return rng.Next(minSkeletonRange, maxSkeletonRange);
    }

    void hideAllWeapons()
    {
        if (sword != null)
        {
            sword.SetActive(false);
        }
        if (bow != null)
        {
            bow.SetActive(false);
        }
        if (staff != null)
        {
            staff.SetActive(false);
        }
        if (axe != null)
        {
            axe.SetActive(false);
        }
        if (club != null)
        {
            club.SetActive(false);
        }
    }
    #endregion
}
