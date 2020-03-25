using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class FogOfWar : MonoBehaviour
{
	static FogOfWar instance;
	byte[] fog;
	int width, height;
	
	const byte VISIBLE = 255;
	const byte SEMI = 128;
	const byte INVISIBLE = 0;
	
	const int VIEW_RANGE = 14;
	
	public static Texture2D fogTex;
	private static Texture2D FogClear;
	private static Texture2D FogActual;
	
    public void Init()
	{
		instance = this;
		MapConfiguration config = GetComponent<MapConfiguration>();
		
		width = config.width;
		height = config.height; // pads out width/height
		fog = new byte[width * height];
		
		FogActual = new Texture2D(width, height, TextureFormat.Alpha8, false);
		FogActual.wrapMode = TextureWrapMode.Clamp;
		UpdateTexture();
		
		fogTex = FogActual;
		
		/*FogClear = new Texture2D(width, height, TextureFormat.Alpha8, false);
		FogClear.wrapMode = TextureWrapMode.Clamp;
		byte[] visible = new byte[width * height];
		for (int i = 0; i < width * height; i++) {
			visible[i] = VISIBLE;
		}
		FogClear.LoadRawTextureData(visible);
		FogClear.Apply();*/
	}
	
	void clearActiveVisibility()
	{
		for (int i = 0; i < fog.Length; i++)
			fog[i] = fog[i] == VISIBLE ? SEMI : fog[i];
	}
	
	void Update()
	{
		clearActiveVisibility();
		
		foreach (Player player in Network.getPlayers()) {
			UpdateVisibility(player.grid_pos);
		}
		
		if (Input.GetKeyDown("c")) {
			fogTex = FogClear;
		}
		else if (Input.GetKeyDown("v")) {
			fogTex = FogActual;
		}
		UpdateTexture();
	}
	
	void UpdateVisibility(Pos location)
	{
		int startx = location.x - VIEW_RANGE < 0 ? 0 : location.x - VIEW_RANGE;
		int starty = location.y - VIEW_RANGE < 0 ? 0 : location.y - VIEW_RANGE;
		int finalx = location.x + VIEW_RANGE >= width ? width - 1 : location.x + VIEW_RANGE;
		int finaly = location.y + VIEW_RANGE >= height ? height - 1 : location.y + VIEW_RANGE;
		
		for (int x = startx; x <= finalx; x++)
			for (int y = starty; y <= finaly; y++) {
				int index = y * width + x;
				//if (Pos.abs_dist(new Pos(x, y), location) <= VIEW_RANGE) 
				fog[index] = VISIBLE;
			}
	}
	
	void UpdateTexture()
	{
		FogActual.LoadRawTextureData(fog);
		FogActual.Apply();
	}
	
	public static bool IsVisible(Pos position)
	{
		return instance.fog[position.y * instance.width + position.x] == VISIBLE;
	}
	
	public static bool IsSemiVisible(Pos position)
	{
		return instance.fog[position.y * instance.width + position.x] != INVISIBLE;
	}
	
	public static bool VisibleInRange(int sx, int sy, int _width, int _height)
	{
		for (int x = sx; x < sx + _width; x++) {
			for (int y = sy; y < sy + _height; y++) {
				int index = y * instance.width + x;
				if (index >= instance.width * instance.height)
					return false;
				if (instance.fog[index] == VISIBLE)
					return true;
			}
		}
		return false;
	}
}
