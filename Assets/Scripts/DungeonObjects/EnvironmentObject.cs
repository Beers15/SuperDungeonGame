using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class EnvironmentObject : DungeonObject, Damageable, Environment, Renderable
{
	public bool traversable = false;
	public int health = 0;
	//public int debugX = -1000;
	//public int debugY = -1000;
	Renderer rend;
	ParticleSystem particles;
	List<GameObject> children = new List<GameObject>();
	public void init_environment(Pos position, int health = 10)
	{
		grid_pos = position;
		//debugX = grid_pos.x;
		//debugY = grid_pos.y;
		//this.health = health;
		rend = GetComponent<Renderer>();
		if (rend != null) {
			rend.sharedMaterial.SetTexture("_FOWTex", FogOfWar.fogTex);
			rend.sharedMaterial.SetVector("_MapWidthHeight", new Vector4(MapManager.MapWidth, MapManager.MapHeight, 0, 0));
		}
		particles = GetComponent<ParticleSystem>();
		foreach (Transform child in transform)
			children.Add(child.gameObject);
	}
	void Update()
	{
		if (FogOfWar.IsSemiVisible(grid_pos))
			EnableRendering();
		else
			DisableRendering();
	}
    public void take_damage(int amount)
	{
		health -= amount;
		Debug.Log(gameObject.name);
		if (health < 0)
			GameManager.kill(this, 0.5f);
	}
	public void playHitAnimation() {}
	public void playHitNoise(string type) {}
	private bool renderEnabled = true;
	public void EnableRendering()
	{
		if (rend != null) rend.enabled = true;
		if (particles != null) particles.Play();
		foreach (GameObject child in children) {
			child.SetActive(true);
		}
		renderEnabled = true;
	}
	public void DisableRendering()
	{
		if (rend != null) rend.enabled = false;
		if (particles != null) particles.Stop();
		foreach (GameObject child in children) {
			child.SetActive(false);
		}
		renderEnabled = false;
	}
}
