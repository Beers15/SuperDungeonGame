using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class BridgeMeshGenerator : MonoBehaviour
{
	private enum BMETA { PLATFORM_UNCHECKED, PLATFORM_CHECKED, BRIDGE_UNCHECKED, BRIDGE_UD, BRIDGE_LR, NONE };
	private bool shouldStopBuild(BMETA meta) { return meta == BMETA.PLATFORM_CHECKED || meta == BMETA.PLATFORM_UNCHECKED || meta == BMETA.NONE; }
	private string getString(BMETA meta) 
	{ 
		switch (meta) {
			case BMETA.PLATFORM_CHECKED: return "PC";
			case BMETA.PLATFORM_UNCHECKED: return "PU";
			case BMETA.BRIDGE_UNCHECKED: return "BU";
			case BMETA.BRIDGE_UD: return "UD";
			case BMETA.BRIDGE_LR: return "LR";
			default: return "  ";
		}
	}
	
	// the meshes to be used for each bridge tile
	public Mesh bridgeBase;
	public Mesh bridgeLegs;
	public Mesh platformBase;
	public Mesh platformLegs;
	
	[Tooltip("The color gradient for the map, coloring based on height. Values towards 0 are closer to the map's surface, values towards 1 are closer to the map's floor")]
	public Gradient gradient;
	
	// bridge mesh generation algorithm
	public void generateBridgeMesh(int[,] map)
	{
		/* STEP ONE - INITIALIZATION
		 * Initialize all relevant variables, e.g width/height etc.
		 * Create the metadata map for use later down the line 
		 * The metadata map contains information that is relevant for bridge generation, e.g bridges and platforms */
		
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		
		int width = config.width;
		int height = config.height;
		float wallHeight = config.wall_height;
		float cellSize = config.cell_size;
		
		BMETA[,] bridgeMeta = new BMETA[width, height];
		
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)
				switch (map[x, y]) {
					case BRIDGE: bridgeMeta[x, y] = BMETA.BRIDGE_UNCHECKED; break;
					case PLATFORM: bridgeMeta[x, y] = BMETA.PLATFORM_UNCHECKED; break;
					default: bridgeMeta[x, y] = BMETA.NONE; break;
				}
				
		/* STEP TWO - BRIDGE ALGORITHM
		 * It is important to determine which orientation each bridge should be before creating the bridge mesh
		 * This algorithm checks up/down/left/right relative to each platform to determine which direction the bridges should face
		 * Once it determines the direction, the algorithm steps in that direction, "building" bridge tiles until it reaches another platform */
		
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				BMETA cell = bridgeMeta[x, y];
				if (cell == BMETA.PLATFORM_UNCHECKED) {
					
					bridgeMeta[x, y] = BMETA.PLATFORM_CHECKED;
					
					Dir build = Dir.NONE;
					if (bridgeMeta[x-1, y] == BMETA.BRIDGE_UNCHECKED)
						build = Dir.LEFT;
					else if (bridgeMeta[x+1, y] == BMETA.BRIDGE_UNCHECKED)
						build = Dir.RIGHT;
					else if (bridgeMeta[x, y-1] == BMETA.BRIDGE_UNCHECKED)
						build = Dir.UP;
					else if (bridgeMeta[x, y+1] == BMETA.BRIDGE_UNCHECKED)
						build = Dir.DOWN;
					
					if (build == Dir.NONE) continue;
					
					int bx = build.toVector().x;
					int by = build.toVector().y;
					
					BMETA buildTile = bx != 0 ? BMETA.BRIDGE_LR : BMETA.BRIDGE_UD;
					int currx = x + bx, curry = y + by;
					while (Pos.in_bounds(new Pos(currx, curry), width, height) && !shouldStopBuild(bridgeMeta[currx, curry])) {
						bridgeMeta[currx, curry] = buildTile;
						currx += bx;
						curry += by;
					}
				}
				
			}
		}
		
		// optional debug
		/*string debugString = "";
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++)
				debugString += getString(bridgeMeta[x, y]);
			debugString += "\n";
		}	
		Debug.Log(debugString);*/
		
		/* STEP 3 - MESH CREATION
		 * Now that the orientation for each bridge tile has been determined, it's time to generate the mesh
		 * Each in-world bridge tile will consist of:
		 * 1: a base mesh, i.e what is on the ground level of the bridge.
		 * 2: N leg meshes, stacked on top of each other. How many legs there are is based on how high the surface walls are 
		 * There are two types of bridge tiles; bridges and platforms. 
		 * Bridges have an orientation based on what metadata value they were assigned, while platforms are omnidirectional */
		
		List<int> bridgeTriangles = new List<int>();
		List<Vector3> bridgeVertices = new List<Vector3>();
		List<Vector2> bridgeUVs = new List<Vector2>();
				
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				
				Mesh baseMesh, legsMesh;
				switch (bridgeMeta[x, y]) {
					case BMETA.PLATFORM_CHECKED:
						baseMesh = platformBase;
						legsMesh = platformLegs; break;
					case BMETA.BRIDGE_LR: // both bridge types use the same model
					case BMETA.BRIDGE_UD:
						baseMesh = bridgeBase;
						legsMesh = bridgeLegs; break;
					default:
						continue;
				}
				
				// determines whether to rotate the model by 90 degress, depending on what type of bridge it is
				Quaternion rotation = bridgeMeta[x, y] == BMETA.BRIDGE_LR ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);
				Vector3 translation = config.gridToWorld(new Pos(x, y)); // transforms mesh vertices to the in-world position they should be
				Vector3 scaling		= new Vector3(0.5f, 0.5f, 0.5f); // scaling is on 0.5 because I forgot to scale down my models in blender (oops!)
				Matrix4x4 transform = Matrix4x4.TRS(translation, rotation, scaling); // all of these transformations are cumulated into the transform matrix
				
				// copies the base mesh's vertices, transforms them, then adds them to the vertices list
				Vector3[] newBaseVerts = baseMesh.vertices.Clone() as Vector3[];
				for (int i = 0; i < newBaseVerts.Length; i++) {
					newBaseVerts[i] = transform.MultiplyPoint3x4(newBaseVerts[i]);
				}
				// copies the triangles as well, simply adds the current vertex index count to each index value
				int[] newBaseTriangles = baseMesh.triangles.Clone() as int[];
				int startingIndex = bridgeVertices.Count;
				for (int i = 0; i < newBaseTriangles.Length; i++) {
					newBaseTriangles[i] += startingIndex;
				}
				bridgeTriangles.AddRange(newBaseTriangles);
				bridgeVertices.AddRange(newBaseVerts);
				bridgeUVs.AddRange(baseMesh.uv); // uvs don't need to be modified
				
				// determines the height of the legs mesh, so we know how many times we need to tile it downwards
				float legsYStart = (baseMesh.bounds.max.y - baseMesh.bounds.min.y) * 0.5f;
				float legsYSpan  = (legsMesh.bounds.max.y - legsMesh.bounds.min.y) * 0.5f; // scales by 0.5 because of aforementioned blender goof
				Vector3 legsPos  = translation + new Vector3(0, -legsYStart, 0);
				
				// performs the same operation we did for the base mesh for the legs mesh
				// tiles the legs downwards until they reach wallHeight
				while (legsPos.y > -wallHeight) {
					transform = Matrix4x4.TRS(legsPos, rotation, scaling);
					
					Vector3[] newLegsVerts = legsMesh.vertices.Clone() as Vector3[];
					for (int i = 0; i < newLegsVerts.Length; i++) {
						newLegsVerts[i] = transform.MultiplyPoint3x4(newLegsVerts[i]);
					}
					int[] newLegsTriangles = legsMesh.triangles.Clone() as int[];
					startingIndex = bridgeVertices.Count;
					for (int i = 0; i < newLegsTriangles.Length; i++) {
						newLegsTriangles[i] += startingIndex;
					}
					bridgeTriangles.AddRange(newLegsTriangles);
					bridgeVertices.AddRange(newLegsVerts);
					bridgeUVs.AddRange(legsMesh.uv);
					
					legsPos.y -= legsYSpan;
				}
			}
		}
		
		/* STEP 4 - GRADIENT GENERATION
		 * So this code is copy-pasted from the MapMeshGenerator code
		 * I would prefer this step to only happen once, and be written to an image somewhere for both bridges and surface to use
		 * But for some god damn reason I can't seem to find a way to create a blank image with an alpha channel, so I have to do it in code
		 * This gradient is used to create the lava/water/fog effect for the map shader */
		
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
		
		/* STEP 5 - FINALIZATION
		 * Finally, we write our new vertices & triangles to the mesh filter */
		
		MeshFilter mf = GetComponent<MeshFilter>();
		// blank any existing triangles so that unity won't throw a fit when we set the new vertices
		mf.mesh.triangles = null; 
		mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // by default, vertex indexing only goes up to 65536 (16-bit), which often isn't enough
		
		mf.mesh.vertices = bridgeVertices.ToArray();
		mf.mesh.uv = bridgeUVs.ToArray();
		mf.mesh.triangles = bridgeTriangles.ToArray();
		mf.mesh.RecalculateNormals();
		mf.mesh.RecalculateBounds();
	}
}