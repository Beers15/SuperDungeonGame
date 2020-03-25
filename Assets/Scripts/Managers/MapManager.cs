using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using RegionUtils;
using static MapUtils.MapConstants;
using System;

public class MapManager : MonoBehaviour
{
	private class MapCell {
        public bool traversable;
        public bool occupied;
        public DungeonObject resident; // foreground objects that can be interacted with & block tiles
		public DungeonObject environment; // background objects that cannot be interacted with
        public bool reserved = false;
        public bool spawn = false;
        public MapCell(bool traversable) {
            this.traversable = traversable;
            occupied = false;
            resident = null;
        }
    }

	public GameObject mapPrefab, fireProjectile, arrowProjectile, lightningProjectile;
	//public MapManager instance;

	// config variables
	private int width;
	private int height;
	private float cell_size;
	public static int MapWidth {
		get {
			return instance.width;
		}
	}
	public static int MapHeight {
		get {
			return instance.height;
		}
	}

	// map data
	private int[,] map_raw;
	private Region region_tree_root; // the root node of the region tree for the map
    private MapCell[,] map;
	private NavigationHandler nav_map;

	private GameManager parentManager = null;
	private TileSelector tileSelector = null;
	private System.Random rng;
	private List<GameObject> environmentObjects;
	private static MapManager instance;

    // called by gamemanager, initializes map components
    public void Init(GameManager parent)
	{
		parentManager = parent;
		instance = this;
		NetworkManager.mapManager = instance;

		// begin component init
		GameObject mapObject = GameObject.FindGameObjectWithTag("Map");
		if (mapObject == null)
			mapObject = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);

		map_raw = mapObject.GetComponent<MapGenerator>().generate_map();
		region_tree_root = mapObject.GetComponent<MapGenerator>().getMainRegion();

		nav_map = new NavigationHandler(map_raw);

		set_config_variables();
		map = new MapCell[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map[x, y] = new MapCell(traversable(map_raw[x, y]));
			}
		}
		
		rng = new System.Random(Settings.MasterSeed);
		environmentObjects = new List<GameObject>();
	}
	
	// set all map configuration variables
	private void set_config_variables()
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		this.width = config.width;
		this.height = config.height;
		this.cell_size = config.cell_size;
	}

    /*****************/
    /* MAP FUNCTIONS */
    /*****************/

    public void setTileReserved(Pos position)
    {
        map[position.x, position.y].reserved = true;
    }

    public void setTileSpawn(Pos position)
    {
        map[position.x, position.y].reserved = true;
        map[position.x, position.y].spawn = true;
    }

    // instantiates an agent into the map at a random position
    public GameObject instantiate_randomly(GameObject type)
	{
		int x = rng.Next(0, width - 1);
		int y = rng.Next(0, height - 1);

		while (!IsWalkable(new Pos(x, y))) {
			x = rng.Next(0, width - 1);
			y = rng.Next(0, height - 1);
		}

		return instantiate(type, new Pos(x, y));
	}


	// instantiates an agent into the map
    public GameObject instantiate(GameObject prefab, Pos pos, GameAgentStats stats = null, string name = null)
	{
		if (!IsWalkable(pos)) return null;
		
		GameObject clone = Instantiate(prefab, grid_to_world(pos), Quaternion.identity);
		GameAgent agent = clone.GetComponent<GameAgent>();
        //string[] names = new string[] { "Keawa", "Benjamin", "Diana", "Jerry", "Joe" };

        if (stats == null) {
            agent.init_agent(pos, new GameAgentStats(CharacterRaceOptions.Human, CharacterClassOptions.Knight, 1, CharacterClassOptions.Sword), name);
        } else {
            agent.init_agent(pos, stats, name);
        }

		nav_map.removeTraversableTile(pos);
		map[pos.x, pos.y].resident = agent;
		map[pos.x, pos.y].occupied = true;
		return clone;
	}
	
	private Pos random_traversable_pos()
	{
		Pos candidate;
		do {
			candidate = new Pos(rng.Next(width), rng.Next(height));
		} while (!IsTraversable(candidate));
		return candidate;
	}
	
	public GameObject instantiate_environment_randomly(GameObject environmentObject, bool traversable = true)
	{
		
		return instantiate_environment(environmentObject, random_traversable_pos(), traversable);
	}
	
	public GameObject instantiate_environment(GameObject environmentObject, Pos pos, bool traversable = true)
	{
        int randomY = rng.Next(1, 4) * 90;
		
		// detect whether this object is a prefab or has already been spawned
		if (environmentObject.scene.name != null) {
			environmentObject.transform.position = grid_to_world(pos);
			environmentObject.transform.rotation = Quaternion.Euler(new Vector3(0, randomY, 0));
		}
		else {
			environmentObject = Instantiate(environmentObject, grid_to_world(pos), Quaternion.Euler(new Vector3(0, randomY, 0)));
		}
		
		DungeonObject env = environmentObject.GetComponent<DungeonObject>();
		(env as Environment).init_environment(pos);
		
		if (!traversable) {
			nav_map.removeTraversableTile(pos);
			map[pos.x, pos.y].traversable = true;
			map[pos.x, pos.y].occupied = true;
			map[pos.x, pos.y].resident = env;
		}
		else {
			nav_map.insertTraversableTile(pos);
			map[pos.x, pos.y].traversable = true;
			map[pos.x, pos.y].occupied = false;
			map[pos.x, pos.y].environment = env;
		}
		
		return environmentObject;
	}
	
	// place a player's object back onto the map
	public void re_instantiate(GameObject agentObject, Pos pos)
	{
		agentObject.transform.position = grid_to_world(pos);
		agentObject.transform.rotation = Quaternion.identity;
		
		Player player = agentObject.GetComponent<Player>();
		player.re_init(pos);
		
		nav_map.removeTraversableTile(pos);
		map[pos.x, pos.y].resident = player;
		map[pos.x, pos.y].occupied = true;
	}

	// removes an object from the map, destroying its game object
	public void de_instantiate(Pos pos, float waitTime = 5f)
	{
		Destroy(map[pos.x, pos.y].resident.gameObject, waitTime);

		nav_map.insertTraversableTile(pos);
		map[pos.x, pos.y].resident = null;
		map[pos.x, pos.y].occupied = false;
		map[pos.x, pos.y].environment = null;
	}

	// destroys all game objects currently on the map
	public void clear_map()
	{
		for (int x = 0; x < width; x++)
			for (int y = 0; y < height; y++)  {
				if (map[x, y].environment != null)
					Destroy(map[x, y].environment.gameObject);
				if (map[x, y].resident != null) {
					Destroy(map[x, y].resident.gameObject);
					map[x, y].occupied = false;
					map[x, y].resident = null;
				}
			}
	}
	
	// move a character from source to dest
	public void move(Pos source, Pos dest)
	{
		DungeonObject obj = map[source.x, source.y].resident;
		if (obj == null || !(obj is GameAgent)) { 
			Debug.Log("Move command was invalid!"); 
			return; 
		}
		
		GameAgent agent = obj as GameAgent;
		
		Path path = get_path(source, dest);
		
		nav_map.removeTraversableTile(dest);
		map[dest.x, dest.y].occupied = true;
		map[dest.x, dest.y].resident = agent;
		
		nav_map.insertTraversableTile(source);
		map[source.x, source.y].occupied = false;
		map[source.x, source.y].resident = null;
		
		StartCoroutine(agent.smooth_movement(path));
	}
	
	// applies damage to agent at position, if one is there
	public void attack(Pos source, Pos dest, int actionNo=0)
	{
		DungeonObject obj_attacker = map[source.x, source.y].resident;
		DungeonObject obj_target = map[dest.x, dest.y].resident;
		if (obj_attacker == null || obj_target == null || !(obj_attacker is GameAgent) || !((obj_target is GameAgent) || (obj_target is Damageable))) {
			Debug.Log("Attack command was invalid!");
			return;
		}
		
		GameAgent attacker = obj_attacker as GameAgent;
		attacker.SetCurrentAction(actionNo);
		
		if (obj_target is Damageable) {
			Damageable target = obj_target as Damageable;
			attacker.attack(target);
		}
	}
	
	public void interact(Pos source, Pos dest)
	{
		DungeonObject obj_interactor = map[source.x, source.y].resident;
		DungeonObject obj_interactee = map[dest.x, dest.y].resident;
		if (obj_interactor == null || obj_interactee == null || !(obj_interactor is GameAgent) || !(obj_interactee is Interactable)) {
			Debug.Log("Interact command was invalid!");
			return;
		}
		
		GameAgent interactor = obj_interactor as GameAgent;
		Interactable interactee = obj_interactee as Interactable;
		
		interactee.interact(interactor);
	}
	
	public static Projectile AnimateProjectile(Pos start, Pos end, string type = "fire")
	{
		Vector3 startPos = instance.grid_to_world(start);
		Vector3 endPos = instance.grid_to_world(end);
		GameObject projectilePrefab;
		
		switch (type) {
			case "fire": projectilePrefab = instance.fireProjectile; break;
			case "arrow": projectilePrefab = instance.arrowProjectile; break;
            case "lightning": projectilePrefab = instance.lightningProjectile; break;
			default: projectilePrefab = instance.fireProjectile; break;
		}	
		
		var clone = Instantiate(projectilePrefab, startPos, Quaternion.identity);
		var projectile = clone.GetComponent<Projectile>();
		projectile	.Init(startPos, endPos);
		return projectile;
	}
	
	public static void ExtractAgent(Player agent)
	{
		Pos agentPos = agent.grid_pos;
		instance.map[agentPos.x, agentPos.y].resident = null;
		instance.map[agentPos.x, agentPos.y].occupied = false;
		agent.extract();
	}
	
	/*************************/
	/* PATHFINDING FUNCTIONS */
	/*************************/
	// NOTE: if you're trying to find multiple paths, opt for get_paths or getDistances, which run much more efficiently

	// get a single path from source to dest
	public Path get_path(Pos source, Pos dest)
	{
		if (source == null || dest == null || source == dest) return null;
		
		if (!IsWalkable(source)) nav_map.insertTraversableTile(source);
		
		Path result = new Path(nav_map.shortestPath(source, dest));
		
		if (!IsWalkable(source)) nav_map.removeTraversableTile(source);
		
		return result;
	}
	
	// get the distance of a single path from source to dest
	public int getDistance(Pos source, Pos dest)
	{
		return get_path(source, dest).distance();
	}

	// Gets a list of paths from a source point to a number of destination points
	/* <param name="source"> 
	 * 		the origin point that paths are searched relative to </param>
	 * <param name="destinations">
	 * 		the list of destination points we want to find paths to </param>
	 * <param name="preserve_null"> 
	 * 		when a path is not found, by default it is not added as an entry to the results list
	 *		when preserve_null is set to true, distances that are not found are instead added as -1 </param>
	 * <param name="maxDistance">
	 *		the maximum allowed distance for resulting paths. By default this value is zero, which means there is no limit to distance
	 *		enabling this can significantly improve pathfinding performance </param>
	 * <returns>
	 * A list of paths from the source to each of the destination points </returns> */
	public List<Path> get_paths(Pos source, List<Pos> destinations, bool preserve_null = false, int maxDistance = 0)
	{
		List<List<Pos>> results = null;
		
		if (!IsWalkable(source)) nav_map.insertTraversableTile(source);
			if (maxDistance == 0)
				results = nav_map.shortestPathBatched(source, destinations);
			else
				results = nav_map.shortestPathBatchedInRange(source, destinations, maxDistance);
		if (!IsWalkable(source)) nav_map.removeTraversableTile(source);
		
		if (results == null) return null;
		
		if (preserve_null) {
			List<List<Pos>> new_results = new List<List<Pos>>();
			int i = 0, j = 0;
			while (i < destinations.Count) {
				if (j < results.Count && destinations[i] == results[j].Last()) {
					new_results.Add(results[j]);
					j++;
				}
				else {
					new_results.Add(null);
				}
				i++;
			}
			results = new_results;
		}
		
		List<Path> paths = new List<Path>();
		foreach (List<Pos> result in results)
			paths.Add(new Path(result));
		
		return paths;
	}
	
	// Gets a list of map distances from a source point to a number of destination points
	/* <param name="source"> 
	 * 		the origin point that paths are searched relative to </param>
	 * <param name="destinations">
	 * 		the list of destination points we want to find distances to </param>
	 * <param name="preserve_null"> 
	 * 		when a path is not found, by default it is not added as an entry to the results list
	 *		when preserve_null is set to true, paths that are not found are instead added as null entries </param>
	 * <param name="maxDistance">
	 *		the maximum allowed distance for resulting distances. By default this value is zero, which means there is no limit to distance </param>
	 *		enabling this can significantly improve pathfinding performance
	 * <returns>
	 * A list of distances from the source to each o the destination points, or null if no results were found </returns> */
	public List<int> getDistances(Pos source, List<Pos> destinations, bool preserve_null = false, int maxDistance=0)
	{
		// getDistances will ignore whether or not the destination tiles are traversable, just gets distances to them
		foreach (Pos dest in destinations) { if (!IsWalkable(dest)) nav_map.insertTraversableTile(dest); }
		
		List<Path> paths = get_paths(source, destinations, preserve_null, maxDistance);
		
		foreach (Pos dest in destinations) { if (!IsWalkable(dest)) nav_map.removeTraversableTile(dest); }
		
		if (paths == null || paths.Count == 0) return null;
		
		List<int> distances = new List<int>();
		foreach (Path path in paths) {
			distances.Add(path.distance());
		}
		return distances;
	}

    /*********************/
    /* UTILITY FUNCTIONS */
    /*********************/

    // returns true if tile terrain at position is a spawn point.
    public bool IsSpawnPoint(Pos pos)
    {
        if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
            return false;
        return map[pos.x, pos.y].spawn;
    }

    // returns true if tile terrain at position is reserved.
    public bool IsReserved(Pos pos)
    {
        if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
            return false;
        return map[pos.x, pos.y].reserved;
    }

    private class RegionNode
	{
		public Region region;
		public int height = 0;
		public List<RegionNode> children;
		// constructs the entire tree
		public RegionNode(Region region)
		{
			this.region = region;
			this.children = new List<RegionNode>();
			foreach (Region child in region.connections) {
				children.Add(new RegionNode(child));
			}
		}
		// assumes the region children have been sorted by height in findSpawnpoints()
		public Region find_nth_furthest_region(int n)
		{
			if (children.Count <= n)
				return region;
			else return children[n].find_nth_furthest_region(0);
		}
	}

    public Region[] findFurthestRegions(int amount)
    {
        RegionNode root = new RegionNode(region_tree_root);
        Stack<RegionNode> bottom = new Stack<RegionNode>();
        Stack<RegionNode> top = new Stack<RegionNode>();

        // traverses tree from bottom->top, pushing nodes to a stack so we can traverse them top->bottom later
        bottom.Push(root);
        while (bottom.Count > 0)
        {
            RegionNode curr = bottom.Pop();
            top.Push(curr);
            foreach (RegionNode child in curr.children)
            {
                bottom.Push(child);
            }
        }

        RegionNode diameterNode = null; // root node of the subtree with the largest diameter
        int maxDiameter = 0;
        // gets the node in the region tree where the two furthest-most leaf nodes are from one another
        while (top.Count > 0)
        {
            RegionNode curr = top.Pop();
            int diameter;
            // if this node is a leaf node, set height/diameter to 1
            if (curr.children.Count == 0)
            {
                curr.height = 1;
                diameter = 1;
            }
            // if this node only has one child, set height/diameter to child height + 1
            else if (curr.children.Count == 1)
            {
                curr.height = curr.children[0].height + 1;
                diameter = curr.height;
            }
            // otherwise, find the two tallest children, set height to the height of the tallest child, and set diameter to the sum of the tallest children + 1
            else
            {
                curr.children.Sort( // sorts children by height
                    delegate (RegionNode a, RegionNode b) {
                        return a.height.CompareTo(b.height);
                    });
                curr.children.Reverse(); // sorts from tallest->smallest

                curr.height = curr.children[0].height + 1;
                diameter = curr.children[0].height + curr.children[1].height + 1;
                Debug.Log("diameter:" + diameter);
            }
            if (diameter > maxDiameter)
            {
                maxDiameter = diameter;
                diameterNode = curr;
            }
        }

        Debug.Log("Max diameter: " + maxDiameter);
        // once the diameter root has been found, get the farthest-most regions
        Region spawnRegion = diameterNode.find_nth_furthest_region(0);
        Region endRegion = diameterNode.find_nth_furthest_region(1);

        return new Region[] { spawnRegion, endRegion };
    }

    // finds <amount> spawnpoints + 1 endpoint, for initial placement of players/level end. endpoint is always at 0th position in list
    // traverses region tree to find the two furthest away regions
    public List<Pos> findSpawnpoints(int amount)
	{
		RegionNode root = new RegionNode(region_tree_root);
		Stack<RegionNode> bottom = new Stack<RegionNode>();
		Stack<RegionNode> top = new Stack<RegionNode>();
		
		// traverses tree from bottom->top, pushing nodes to a stack so we can traverse them top->bottom later
		bottom.Push(root);
		while (bottom.Count > 0) {
			RegionNode curr = bottom.Pop();
			top.Push(curr);
			foreach (RegionNode child in curr.children) {
				bottom.Push(child);
			}
		}
		
		RegionNode diameterNode = null; // root node of the subtree with the largest diameter
		int maxDiameter = 0;
		// gets the node in the region tree where the two furthest-most leaf nodes are from one another
		while (top.Count > 0) {
			RegionNode curr = top.Pop();
			int diameter;
			// if this node is a leaf node, set height/diameter to 1
			if (curr.children.Count == 0) {
				curr.height = 1;
				diameter = 1;
			}
			// if this node only has one child, set height/diameter to child height + 1
			else if (curr.children.Count == 1) {
				curr.height = curr.children[0].height + 1;
				diameter = curr.height;
			}
			// otherwise, find the two tallest children, set height to the height of the tallest child, and set diameter to the sum of the tallest children + 1
			else {
				curr.children.Sort( // sorts children by height
					delegate(RegionNode a, RegionNode b) {
						return a.height.CompareTo(b.height); } );
				curr.children.Reverse(); // sorts from tallest->smallest
				
				curr.height = curr.children[0].height + 1;
				diameter = curr.children[0].height + curr.children[1].height + 1;
				Debug.Log("diameter:"+diameter);
			}
			if (diameter > maxDiameter) {
				maxDiameter = diameter;
				diameterNode = curr;
			}
		}
		
		Debug.Log("Max diameter: " + maxDiameter);
		// once the diameter root has been found, get the farthest-most regions
		Region spawnRegion = diameterNode.find_nth_furthest_region(0);
		Region endRegion   = diameterNode.find_nth_furthest_region(1);
		
		// throws random locations at the map until all suitable player spawn/level end locations are found
		List<Pos> spawnPositions = new List<Pos>();
		int x, y;
		// gets all spawn positions
		while (spawnPositions.Count < amount) {
			do {
				x = rng.Next(0, width - 1);
				y = rng.Next(0, height - 1);
			} while (map_raw[x, y] != spawnRegion.ID || !IsWalkable(new Pos(x, y)));
			
			spawnPositions.Add(new Pos(x, y));
		}
		// gets end point
		do {
			x = rng.Next(0, width - 1);
			y = rng.Next(0, height - 1);
		} while (map_raw[x, y] != endRegion.ID);
		spawnPositions.Insert(0, new Pos(x, y));
		
		return spawnPositions;
	}
	
	public void spawnChests(GameObject chestPrefab)
	{
		int chestsToSpawn = rng.Next((int) Math.Ceiling(Math.Sqrt(width * height) / 10f));
		for (int i = 0; i < chestsToSpawn; i++) {
			instantiate_environment_randomly(chestPrefab, false);
		}
	}
	
	public void SPAWN_ILMI_DEVOURER_OF_WORLDS(GameObject ilmiPrefab, int level)
	{
		instantiate(ilmiPrefab, random_traversable_pos(), new GameAgentStats(CharacterRaceOptions.Human, CharacterClassOptions.Healer, level * 100), "ILMI, DEVOURER OF WORLDS");
	}

	// returns true if tile terrain at position is traversable
    public bool IsTraversable(Pos pos)
	{
		if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
			return false;
		return map[pos.x, pos.y].traversable;
    }
	
	public bool IsInteractable(Pos pos)
	{
		if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
			return false;
		return map[pos.x, pos.y].resident is Interactable;
	}
	
	public bool IsBridge(Pos pos)
	{
		return map_raw[pos.x, pos.y] == BRIDGE || map_raw[pos.x, pos.y] == PLATFORM;
	}

    public bool IsTileInRegion(Pos tile, int ID) {
        return (map_raw[tile.x, tile.y] == ID);
    }

    public int GetTileRegion(Pos tile) {
        return map_raw[tile.x, tile.y];
    }

    public int[,] GetRegionMap() {
        return map_raw;
    }

    // returns true if tile at position contains an agent
    public bool IsOccupied(Pos pos) {
        if (pos.x >= width || pos.x < 0 || pos.y >= height || pos.y < 0)
			return false;
		return map[pos.x, pos.y].occupied;
    }
	
	// wrapper function, return true if tile at position is traversable AND not occupied
	public bool IsWalkable(Pos pos)
	{
		return IsTraversable(pos) && !IsOccupied(pos);
	}

    public GameAgentState GetGameAgentState(Pos dest) {
        DungeonObject obj = map[dest.x, dest.y].resident;
		if (obj == null || !(obj is GameAgent))
			return GameAgentState.Null;
		
		GameAgent agent = obj as GameAgent;

        return agent.stats.currentState;
    }

    public bool GetHealed(Pos dest, int healAmount) {
        DungeonObject obj = map[dest.x, dest.y].resident;
		if (obj == null || !(obj is GameAgent))
			return false;
		
		GameAgent agent = obj as GameAgent;

        agent.GetHealed(healAmount);
        return true;
    }
	
	// gets the transform of agent at position on map, if there is any
    public Transform GetUnitTransform(Pos pos) {
        if (!map[pos.x, pos.y].occupied)
            return null;
        return map[pos.x, pos.y].resident.transform;
    }

    public Transform GetNearestUnitTransform(Pos pos, List<Pos> agents) {

        if (agents.Count > 0) {
            int minDistance = Int32.MaxValue;
            Pos closestAgent = agents[0];

            foreach (Pos agent in agents) {
                int distance = Pos.abs_dist(pos, agent);
                if (distance < minDistance) {
                    closestAgent = agent;
                    minDistance = distance;
                }
            }
            return map[closestAgent.x, closestAgent.y].resident.transform;
        }

        return null;
    }

    // converts grid position (int)(x, y) to world coordinates (float)(x, y, z)
	public Vector3 grid_to_world(Pos pos)
	{
		return new Vector3(pos.x * cell_size + cell_size / 2f, 0f, pos.y * cell_size + cell_size / 2f);
	}

	// converts world position (float)(x, y, z) to grid position (int)(x, y)
	public Pos world_to_grid(Vector3 pos)
	{
		pos = pos;
		return new Pos((int) pos.x, (int) pos.z);
	}
	
	/*******************/
	/* DEBUG FUNCTIONS */
	/*******************/
	
	// draws line from point a to point b on the map
	public void DrawLine(Pos a, Pos b, Color color, float time=5.0f)
	{
		Vector3 origin = grid_to_world(a);
		Vector3 destination = grid_to_world(b);
		
		Debug.DrawLine(origin, destination, color, time);
	}
}
