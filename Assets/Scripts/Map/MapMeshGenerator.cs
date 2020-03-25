	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class MapMeshGenerator : MonoBehaviour
{	
	private const int HMAP_FILLED = -1;
	private const int HMAP_EMPTY = 0;
	private const int HMAP_EDGE = 1;
	
	private int width;
	private int height;
	private float cell_size;
	private float wall_height;
	
	public float steepness;
	public GameObject chunkPrefab;
	
	[Tooltip("The color gradient for the map, coloring based on height. Values towards 0 are closer to the map's surface, values towards 1 are closer to the map's floor")]
	public Gradient gradient;
	
	void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.cell_size = config.cell_size;
		this.wall_height = config.wall_height;
	}
	
	public void generate_map_mesh(int[,] map)
	{
		/* STEP 1: INIT COMPONENTS
		 * Intermediate and height map arrays are constructed 
		 * the intermediate map holds only the data that is relevant to height map construction
		 */
		
			set_config_variables();
			
			int[,] intermediate_map = new int[width, height];
			float[,] height_map = new float[width, height];
			
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					// exclude bridges and platforms, their mesh is generated separately
					if (traversable(map[x, y]) && map[x, y] != BRIDGE && map[x, y] != PLATFORM) {
						
						if (x - 1 >= 0 && !traversable(map[x - 1, y]))
							intermediate_map[x - 1, y] = HMAP_EDGE;
						
						if (x + 1 < width && !traversable(map[x + 1, y]))
							intermediate_map[x + 1, y] = HMAP_EDGE;
						
						if (y - 1 >= 0 && !traversable(map[x, y - 1]))
							intermediate_map[x, y - 1] = HMAP_EDGE;
						
						if (y + 1 < height && !traversable(map[x, y + 1]))
							intermediate_map[x, y + 1] = HMAP_EDGE;
						
						intermediate_map[x, y] = HMAP_FILLED;
					}
				
		/* STEP 2: GENERATE HEIGHT MAP
		 * Intermediate map is iterated over multiple times to determine each cell's distance to a walkable cell
		 * These distances are recorded in the height map, which is used in the next step
		 */
				
			bool finished = false;
			int curr_iteration = 1;
			
			while (!finished) {
				
				finished = true;
				int[,] temp = intermediate_map.Clone() as int[,];
				
				for (int x = 0; x < width; x++) {
					for (int y = 0; y < height; y++) {
						
						if (intermediate_map[x, y] == HMAP_EDGE) {
							
							finished = false;
							height_map[x, y] = height_func(curr_iteration, x, y);
							
							if (x > 0 && intermediate_map[x - 1, y] == HMAP_EMPTY)
								temp[x - 1, y] = HMAP_EDGE;
							
							if (x < width - 1 && intermediate_map[x + 1, y] == HMAP_EMPTY)
								temp[x + 1, y] = HMAP_EDGE;
							
							if (y > 0 && intermediate_map[x, y - 1] == HMAP_EMPTY)
								temp[x, y - 1] = HMAP_EDGE;
							
							if (y < height - 1 && intermediate_map[x, y + 1] == HMAP_EMPTY)
								temp[x, y + 1] = HMAP_EDGE;
							
							temp[x, y] = HMAP_FILLED;
						}
					}
				}
				intermediate_map = temp.Clone() as int[,];
				curr_iteration++;
			}
		
		/* STEP 3: CREATE MESH NODES
		 * Now, a surface mesh is created according to these generated height values
		 * Optionally, a function may be applied to each height value, to amplify or minimize terrain in some fashion 
		 */
		
			Vector3[,] vertex_map = new Vector3[width * 2 + 1, height * 2 + 1];
			
			// initialize vertex_map with vector x and z values
			for(int x = 0; x < width * 2 + 1; x++)
				for(int y = 0; y < height * 2 + 1; y++)
					vertex_map[x, y] = new Vector3((float) x / 2f, 0f, (float) y / 2f);
			
			// Generate vertex_map height values based on height map
			// vertex heights are the average heights of all the tiles they touch
			// e.g: the lower right vertex of a tile actually lies on the intersection of four tiles, so it is the average of their heights
			for (int x = 1; x < width * 2 + 1; x+=2) {
				for (int y = 1; y < height * 2 + 1; y+=2) {
					
					float center_tile, lower_tile, right_tile, lower_right_tile;
					center_tile = height_map[(x - 1) / 2, (y - 1) / 2];
					lower_tile = right_tile = lower_right_tile = center_tile;
					
					if (y < height * 2 - 1)
						lower_tile = height_map[(x - 1) / 2, (y + 1) / 2];
					if (x < width * 2 - 1)
						right_tile = height_map[(x + 1) / 2, (y - 1) / 2];
					if (y < height * 2 - 1 && x < width * 2 - 1)
						lower_right_tile = height_map[(x + 1) / 2, (y + 1) / 2];
					
					vertex_map[x + 0, y + 0].y = (center_tile) / 1f;
					vertex_map[x + 1, y + 0].y = (center_tile + right_tile) / 2f;
					vertex_map[x + 0, y + 1].y = (center_tile + lower_tile) / 2f;
					vertex_map[x + 1, y + 1].y = (center_tile + right_tile + lower_tile + lower_right_tile) / 4f;
				}
			}
			
			// finish up around the edges
			vertex_map[0, 0].y = -wall_height;//height_map[0, 0];
			
			for(int x = 1; x < width * 2 + 1; x+=2) {
				
				/*float center_tile, right_tile;
				right_tile = center_tile = height_map[(x - 1) / 2, 0];
				
				if (x < width * 2 - 1)
					right_tile  = height_map[(x + 1) / 2, 0];*/
				
				//vertex_map[x, 0].y		= (center_tile) / 1f;
				//vertex_map[x + 1, 0].y 	= (center_tile + right_tile) / 2f;
				vertex_map[x, 0].y = -wall_height;
				vertex_map[x + 1, 0].y = -wall_height;
			}
			
			for(int y = 1; y < height * 2 + 1; y+=2) {
				
				/*float center_tile, lower_tile;
				lower_tile = center_tile = height_map[0, (y - 1) / 2];
				
				if (y < height * 2 - 1)
					lower_tile  = height_map[0, (y + 1) / 2];*/
				
				//vertex_map[0, y].y		= (center_tile) / 1f;
				//vertex_map[0, y + 1].y 	= (center_tile + lower_tile) / 2f;
				vertex_map[0, y].y		= -wall_height;
				vertex_map[0, y + 1].y 	= -wall_height;
			}
			
			// anchor all vertices that touch walkable tiles to height 0
			for (int x = 1; x < width * 2 + 1; x+=2) {
				for (int y = 1; y < height * 2 + 1; y+=2) {
					// exclude bridges and platforms in this calculation, they are not part of the terrain
					if (traversable(map[(x - 1) / 2, (y - 1) / 2]) && map[(x - 1) / 2, (y - 1) / 2] != BRIDGE && map[(x - 1) / 2, (y - 1) / 2] != PLATFORM) {
						vertex_map[x - 1, y - 1].y = 0;
						vertex_map[x - 0, y - 1].y = 0;
						vertex_map[x + 1, y - 1].y = 0;
						
						vertex_map[x - 1, y - 0].y = 0;
						vertex_map[x - 0, y - 0].y = 0;
						vertex_map[x + 1, y - 0].y = 0;
						
						vertex_map[x - 1, y + 1].y = 0;
						vertex_map[x - 0, y + 1].y = 0;
						vertex_map[x + 1, y + 1].y = 0;
					}
					// if the tile is an edge, just anchor the center vertex
					else if (map[(x - 1) / 2, (y - 1) / 2] == EDGE) {
						vertex_map[x + 0, y + 0].y = 0;
					}
				}
			}
			
		/* STEP 4: GRADIENT TEXTURE CREATION
		 * Applies gradient to main texture, initializes render material
		 */
		
		Renderer rend = GetComponent<Renderer>();
		Texture2D texture = new Texture2D(512, 1);
		texture.wrapMode = TextureWrapMode.Repeat;
		rend.material.SetTexture("_FluidGradient", texture);
		
		Color[] colors = new Color[512];
		for (int x = 0; x < 512; x++) {
			colors[x] = gradient.Evaluate((float) x / 512f);
		}
		
		texture.SetPixels(colors);
		texture.Apply(true);
		
		rend.material.SetTexture("_FOWTex", FogOfWar.fogTex);
		rend.material.SetVector("_MapWidthHeight", new Vector4(width, height, 0, 0));
		
		/* STEP 5: CHUNK CREATION
		 * Divides map mesh into smaller mesh "chunks" to make rendering more efficient
		 */
		 
		// destroy old chunks
		foreach (Transform child in transform) {
			Destroy(child.gameObject);
		}
		
		int chunkWidth = 8;
		int chunkHeight = 8;
		int chunkXCount = width / chunkWidth;
		int chunkYCount = height / chunkHeight;
		
		for (int chunkX = 0; chunkX < chunkXCount; chunkX++) {
			for (int chunkY = 0; chunkY < chunkYCount; chunkY++) {
				
				/* STEP 6: CREATE TRIANGLES & FINALIZE MESH
				 * Once the mesh nodes are created, an array of triangles, representing the map surface, is constructed from it 
				 */
				
				int startX = chunkX * chunkWidth * 2;
				int startY = chunkY * chunkHeight * 2;
				int finalX = (chunkX + 1) * chunkWidth * 2 > width * 2 ? width * 2 : (chunkX + 1) * chunkWidth * 2;
				int finalY = (chunkY + 1) * chunkHeight * 2 > height * 2 ? height * 2 : (chunkY + 1) * chunkHeight * 2;
				
				int num_vertices = chunkWidth * chunkHeight * 4 * 2 * 3; // width * height tiles, 4 faces per tile, 2 triangles per face, 3 verts/triangle, 
				Vector3[] vertices = new Vector3[num_vertices];
				Vector2[] uvs = new Vector2[num_vertices];
				int[] triangles = new int[num_vertices];
				
				int i = 0; // keeps track of triangles index
				
				for (int x = startX; x < finalX; x++) {
					for (int y = startY; y < finalY; y++) {
						
						Vector3 v0 = vertex_map[x + 0, y + 0];
						Vector3 v1 = vertex_map[x + 1, y + 0];
						Vector3 v2 = vertex_map[x + 0, y + 1];
						Vector3 v3 = vertex_map[x + 1, y + 1];

						// orientation of triangles changes in a regular pattern according to which corner of each tile it occupies
						// a final tile face will be triangulated something like this (excuse the bad text drawing):
						/*
						 * -----------
						 * | \  |   /|  8 triangles, 4 faces per tile, 2 triangles per face
						 * |  \ | /  |
						 * |---------|  All hypotenuse aim towards the center of the tile
						 * |  / | \  |
						 * |/   |   \|
						 * |-----------
						 * 
						 * This creates a repeating pattern in the tile geometry, so that it is not biased towards any particular orientation
						 */   
						 
						if ((x % 2) == 1) {
							SWAPv(ref v0, ref v1);
							SWAPv(ref v2, ref v3);
						}
						
						if ((y % 2) == 1) {
							SWAPv(ref v0, ref v2);
							SWAPv(ref v1, ref v3);
						}
							// because of this re-ordering of vertices, the winding order of the triangles needs to be taken into account
							// With an odd number of swaps, the winding order for the triangles is reversed
							if (((x + y) % 2) == 1) {
								vertices[i + 0] = v0;
								vertices[i + 1] = v1;
								vertices[i + 2] = v3;
								
								vertices[i + 3] = v2;
								vertices[i + 4] = v0;
								vertices[i + 5] = v3;
								
								uvs[i + 0] = new Vector2(v0.x, v0.z);
								uvs[i + 1] = new Vector2(v1.x, v1.z);
								uvs[i + 2] = new Vector2(v3.x, v3.z);
								
								uvs[i + 3] = new Vector2(v2.x, v2.z);
								uvs[i + 4] = new Vector2(v0.x, v0.z);
								uvs[i + 5] = new Vector2(v3.x, v3.z);
							}
							else {
								vertices[i + 0] = v3;
								vertices[i + 1] = v1;
								vertices[i + 2] = v0;
								
								vertices[i + 3] = v3;
								vertices[i + 4] = v0;
								vertices[i + 5] = v2;
								
								uvs[i + 0] = new Vector2(v3.x, v3.z);
								uvs[i + 1] = new Vector2(v1.x, v1.z);
								uvs[i + 2] = new Vector2(v0.x, v0.z);
								
								uvs[i + 3] = new Vector2(v3.x, v3.z);
								uvs[i + 4] = new Vector2(v0.x, v0.z);
								uvs[i + 5] = new Vector2(v2.x, v2.z);
							}
						
							triangles[i + 0] = i;
							triangles[i + 1] = i + 1;
							triangles[i + 2] = i + 2;
							
							triangles[i + 3] = i + 3;
							triangles[i + 4] = i + 4;
							triangles[i + 5] = i + 5;
						
						i += 6;
					}
				}
				
				/* STEP 7: FINALISATION
				 * Assigns vertices, triangles, and uvs to chunk mesh
				 */
						
				GameObject chunk = Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity, this.transform);
				
				Renderer chunkRend = chunk.GetComponent<Renderer>();
				chunkRend.material = rend.material;
				
				// this turns out to not help at all
				//chunk.GetComponent<ChunkOptimizer>().Init(startX, startY, finalX - startX, finalY - startY);
				
				MeshFilter mf = chunk.GetComponent<MeshFilter>();
				// unless indexFormat Uint32 is specified, mesh index goes up to only 65536 (16-bit)
				//mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
				mf.mesh.triangles = null;
				
				mf.mesh.vertices = vertices;
				mf.mesh.uv = uvs;
				mf.mesh.triangles = triangles;
				mf.mesh.RecalculateNormals();
			}
		}

		/*Vector3[] vertices = new Vector3[width * height * 4 * 2 * 3]; // width * height tiles, 4 faces per tile, 2 triangles per face, 3 verts/triangle, 
		Vector2[] uvs = new Vector2[width * height * 4 * 2 * 3];
		//Vector2[] uv2s = new Vector2[width * height * 4 * 2 * 3];
		int[] triangles = new int[width * height * 4 * 2 * 3];
		
		int i = 0; // keeps track of triangles index
		
		for (int x = 0; x < width * 2; x++) {
			for (int y = 0; y < height * 2; y++) {
				
				Vector3 v0 = vertex_map[x + 0, y + 0];
				Vector3 v1 = vertex_map[x + 1, y + 0];
				Vector3 v2 = vertex_map[x + 0, y + 1];
				Vector3 v3 = vertex_map[x + 1, y + 1];

				// orientation of triangles changes in a regular pattern according to which corner of each tile it occupies
				// a final tile face will be triangulated something like this (excuse the bad text drawing):
				/*
				 * -----------
				 * | \  |   /|  8 triangles, 4 faces per tile, 2 triangles per face
				 * |  \ | /  |
				 * |---------|  All hypotenuse aim towards the center of the tile
				 * |  / | \  |
				 * |/   |   \|
				 * |-----------
				 * 
				 * This creates a repeating pattern in the tile geometry, so that it is not biased towards any particular orientation
				 */   
				 
				/*if ((x % 2) == 1) {
					SWAPv(ref v0, ref v1);
					SWAPv(ref v2, ref v3);
				}
				
				if ((y % 2) == 1) {
					SWAPv(ref v0, ref v2);
					SWAPv(ref v1, ref v3);
				}
					// because of this re-ordering of vertices, the winding order of the triangles needs to be taken into account
					// With an odd number of swaps, the winding order for the triangles is reversed
					if (((x + y) % 2) == 1) {
						vertices[i + 0] = v0;
						vertices[i + 1] = v1;
						vertices[i + 2] = v3;
						
						vertices[i + 3] = v2;
						vertices[i + 4] = v0;
						vertices[i + 5] = v3;
						
						uvs[i + 0] = new Vector2(v0.x, v0.z);
						uvs[i + 1] = new Vector2(v1.x, v1.z);
						uvs[i + 2] = new Vector2(v3.x, v3.z);
						
						uvs[i + 3] = new Vector2(v2.x, v2.z);
						uvs[i + 4] = new Vector2(v0.x, v0.z);
						uvs[i + 5] = new Vector2(v3.x, v3.z);
					}
					else {
						vertices[i + 0] = v3;
						vertices[i + 1] = v1;
						vertices[i + 2] = v0;
						
						vertices[i + 3] = v3;
						vertices[i + 4] = v0;
						vertices[i + 5] = v2;
						
						uvs[i + 0] = new Vector2(v3.x, v3.z);
						uvs[i + 1] = new Vector2(v1.x, v1.z);
						uvs[i + 2] = new Vector2(v0.x, v0.z);
						
						uvs[i + 3] = new Vector2(v3.x, v3.z);
						uvs[i + 4] = new Vector2(v0.x, v0.z);
						uvs[i + 5] = new Vector2(v2.x, v2.z);
					}
				
					triangles[i + 0] = i;
					triangles[i + 1] = i + 1;
					triangles[i + 2] = i + 2;
					
					triangles[i + 3] = i + 3;
					triangles[i + 4] = i + 4;
					triangles[i + 5] = i + 5;
				
				i += 6;
			}
		}
		
		/*MeshFilter mf = GetComponent<MeshFilter>();
		
		// unless indexFormat Uint32 is specified, mesh index goes up to only 65536 (16-bit)
		mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mf.mesh.triangles = null;
		
		mf.mesh.vertices = vertices;
		mf.mesh.uv = uvs;
		//mf.mesh.uv2 = uv2s;
		mf.mesh.triangles = triangles;
		mf.mesh.RecalculateNormals();*/
	}
	
	private void SWAPv(ref Vector3 a, ref Vector3 b)
	{
		Vector3 tmp = a;
		a = b;
		b = tmp;
	}

	private float height_func(int hval, int x, int y)
	{
		return -wall_height * ((sigmoid(hval * steepness) - 0.5f) * 2);
	}
	
	private float sigmoid(float x)
	{
		return 1f / (1 + Mathf.Exp(-x));
	}
	
	private char intermediate_char(int int_val)
	{
		switch(int_val)
		{
			case -1 : return ' ';
			case 0  : return '#';
			case 1  : return 'E';
			default : return ' ';
		}
	}
	
	private char height_char(int height_val)
	{
		switch(height_val)
		{
			case 0 : return ' ';
			case 1 : return '/';
			case 2 : return '%';
			case 3 : return '@';
			case 4 : return '&';
			default: return '#';
		}
	}
	
	// code jail:
	/*int adj_tiles = 0;
				int surf_tiles = 0;
				float height_avg = 0;
				
				if (x < width && y < height) {
					height_avg += height_map[x, y];
					surf_tiles += height_map[x, y] == 0 ? 1 : 0;
					adj_tiles++;
				}
				if (x - 1 >= 0 && y < height) {
					height_avg += height_map[x - 1, y];
					surf_tiles += height_map[x - 1, y] == 0 ? 1 : 0;
					adj_tiles++;
				}
				if (x < width && y - 1 >= 0) {
					height_avg += height_map[x, y - 1];
					surf_tiles += height_map[x, y - 1] == 0 ? 1 : 0;
					adj_tiles++;
				}
				if (x - 1 >= 0 && y - 1 >= 0) {
					height_avg += height_map[x - 1, y - 1];
					surf_tiles += height_map[x - 1, y - 1] == 0 ? 1 : 0;
					adj_tiles++;
				}
				
				height_avg /= (float) adj_tiles;

				if (surf_tiles > 0)
					height_avg = 0;
				
				vertex_map[x, y] = new Vector3(x * cell_size, -height_avg, y * cell_size) - offset;*/
				
	/*GradientColorKey[] color_key = new GradientColorKey[6];
		color_key[0].color = new Color(0.9f, 0.1f, 0.1f);
		color_key[0].time = 1f;
		color_key[1].color = new Color(168f / 255f, 0f, 0f);
		color_key[1].time = 4f / 5f;
		color_key[2].color = new Color(0.4f, 0.4f, 0.4f);
		color_key[2].time = 3f / 5f;
		color_key[3].color = new Color(238f / 255f, 1f, 0f);
		color_key[3].time = 2f / 5f;
		color_key[4].color = new Color(140f / 255f, 196f / 255f, 0f);
		color_key[4].time = 1f / 5f;
		color_key[5].color = new Color(84f / 255f, 117f / 255f, 0f);
		color_key[5].time = 0f;*/
}
