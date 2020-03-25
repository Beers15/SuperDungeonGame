using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class EndPortal : DungeonObject, Environment, Interactable, Renderable
{
	private static int extractedPlayersCount = 0;
	public void init_environment(Pos grid_pos, int health = 10000000)
	{
		this.grid_pos = grid_pos;
		extractedPlayersCount = 0;
	}
	void Update()
	{
		if (FogOfWar.IsSemiVisible(grid_pos))
			EnableRendering();
		else
			DisableRendering();
	}
	public void interact(GameAgent interactor)
	{
		MapManager.ExtractAgent(interactor as Player);
		extractedPlayersCount++;
	}
	public static bool AllPlayersExtracted()
	{
		return extractedPlayersCount == Network.playerCount();
	}
	public void EnableRendering()
	{
		transform.GetComponentInChildren<ParticleSystem>().Play();
	}
	public void DisableRendering()
	{
		transform.GetComponentInChildren<ParticleSystem>().Pause();
	}
}
