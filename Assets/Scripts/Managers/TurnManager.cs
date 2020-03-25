using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //static instance of game manager which allows it to be accessed by any other script
    public static TurnManager instance = null;
	
    // lists of all active players/enemies
    private List<GameAgent>[] teamRoster = new List<GameAgent>[16];
	
	private GameManager parentManager = null;
	private IEnumerator mainLoop = null;
	
	void Awake()
	{
		instance = this;
		for (int i = 0; i < 16; i++) 
			teamRoster[i] = new List<GameAgent>();
	}
	
	public void Init(GameManager parent)
	{
		instance = this;
		parentManager = parent;
		
		AIManager.roster = teamRoster;
		mainLoop = TurnLoop();
	}
	
	public void StartLoop()
	{
		
		StartCoroutine(mainLoop);
	}
	
    IEnumerator TurnLoop()
	{	
		while (true) {
			
			yield return null;
			for (int team = 0; team < 16; team++) {
				if (teamRoster[team].Count == 0) {
					continue;
				}
				
				StartCoroutine(AIManager.update(team));
				while (!AIManager.turnOver(team)) {
					yield return null;
				}
				Debug.Log("turn over! " + teamsLeft());
				if (teamsLeft() == 1 && teamRoster[0].Count == 0) {
					if (EndPortal.AllPlayersExtracted()) {
						GameManager.NextLevel();
						yield break;
					}
					else {
						GameManager.GameOver();
						yield break;
					}
				}
				
			}
		}
    }
	
	private int teamsLeft()
	{
		int teams = 0;
		foreach (List<GameAgent> faction in teamRoster)
			if (faction.Count > 0)
				teams++;
		
		return teams;
	}
	
	public void removeFromRoster(GameAgent agent)
	{
		foreach (List<GameAgent> faction in teamRoster) {
			if (faction.Contains(agent)) {
				faction.Remove(agent);
				return;
			}
		}
	}

	public void addToRoster(GameAgent agent)
	{
		teamRoster[agent.team].Add(agent);
	}

    public void Terminate()
	{
		foreach (List<GameAgent> faction in teamRoster)
			faction.Clear();
		StopCoroutine(mainLoop);
	}
}
