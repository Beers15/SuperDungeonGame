using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;

public class MapConfiguration : MonoBehaviour
{
	public int _width;
	public int _height;
	public int _padding;
    public int width { get { return _width + _padding*2; } }
	public int height { get { return _height + _padding*2; } }
	[Range(0, 100)]
	public int fill_percent;
	[Range(0, 30)]
	public int smoothness;
	[Range(0, 100)]
	public int region_cull_threshold;
	
	[Range(0, 10)]
	public float cell_size;
	[Range(0, 10)]
	public float wall_height;
	public float wall_texture_scale;
	public float surface_texture_scale;
	public float bridge_texture_scale;
	public float floor_texture_scale;
    public float object_size_scale;
	
	public int seed { get { return Settings.MapSeed; } }

    public Vector3 GetCenter() {
        return new Vector3(width / (2f * cell_size), 0.0f, height / (2f * cell_size));
    }
	
	public Rect GetRect() {
		return new Rect((-width * cell_size) / 2f, (-height * cell_size) / 2f, width * cell_size * 2f, height * cell_size * 2f);
	}

    public System.Random GetRNG() {
        return new System.Random(seed);
    }
	
	public Vector3 gridToWorld(Pos pos) {
		return new Vector3(pos.x * cell_size + cell_size / 2f, 0f, pos.y * cell_size + cell_size / 2f);
	}
}
