using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapUtils;
using static MapUtils.MapConstants;

public class TileSelector : MonoBehaviour
{	
	private int width;
	private int height;
	private float cell_size;

    private Transform select_square;
	private MapManager map_manager;
	private LineRenderer path_render;
	private Player player_main;

    private List<Path> selectableMovementTiles;
    private List<Pos> selectableActTiles;
    private List<Pos> nonselectableActTiles;
    private List<Pos> actAOETiles;

    public Vector3 hover_position;
	public Pos grid_position;
	
	public Mesh tileMesh;
	public Material moveableTilesMaterial;
	public Material selectableTilesMaterial;
    public Material nonselectableTilesMaterial;
	public Material interactTilesMaterial;
	
	private string modeStr;
	public string mode {
		get {
			return modeStr;
		}
		set {
			modeStr = value;
			switch (value) {
				case "MOVE":
					CreateListOfSelectableMovementTiles(); break;
				case "ACT":
					CreateListOfSelectableActTiles(); break;
				case "INTERACT":
					CreateListOfSelectableInteractTiles(); break;
			}
		}
	}

    // called by the gameManager script
    public void Init(MapManager map_manager)
	{
		MapConfiguration config = GameObject.FindGameObjectWithTag("Map").GetComponent<MapConfiguration>();
		cell_size = config.cell_size;
		width = config.width;
		height = config.height;
		
		this.map_manager = map_manager;
		this.path_render = GetComponent<LineRenderer>();
		
		select_square = transform.Find("Selector");
	}
	
	// called by the Player script
	public void setPlayer(Player p) 
	{
		this.player_main = p;
		grid_position = p.grid_pos;
	}
	
	void Update()
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit)) {
			
			Pos hitp = map_manager.world_to_grid(hit.point);
			
			if (map_manager.IsTraversable(hitp) && hitp != grid_position) {
				
				grid_position = hitp;
				select_square.gameObject.SetActive(true);
				select_square.position = map_manager.grid_to_world(hitp);
				
			} // check if hovering same position as last turn so as to not cause flickering
			else if (hitp != grid_position) {
				
				select_square.gameObject.SetActive(false);
				grid_position = null;
			}
		}
		
		DrawTiles();
    }
	
	void render_path_line(Path path)
	{
		if (path == null || path.empty()) return;
		
		List<Pos> path_raw = path.getPositions();
		Vector3[] path_verts = new Vector3[path_raw.Count];
		
		for(int i = 0; i < path_raw.Count; i++) {
			path_verts[i] = map_manager.grid_to_world(path_raw[i]) + Vector3.up * 0.2f;
		}
		
		path_render.positionCount = path_raw.Count;
		path_render.SetPositions(path_verts);
	}
	
	public void clear_path_line()
	{
		path_render.positionCount = 0;
		
		Vector3[] blank = new Vector3[0];
		path_render.SetPositions(blank);
	}
	
    // Creates a list of all selectable tiles within a given radius of a position
    // Consider turning this static for enemy AI if this is the only method they need from this class
    public void CreateListOfSelectableMovementTiles() 
	{	
		Pos position = player_main.grid_pos;
		int move_budget = player_main.move_budget;
	
		int startx 	= position.x - move_budget >= 0 ? position.x - move_budget : 0;
		int endx	= position.x + move_budget < width ? position.x + move_budget : width - 1;
		
		int starty 	= position.y - move_budget >= 0 ? position.y - move_budget : 0;
		int endy	= position.y + move_budget < height ? position.y + move_budget : height - 1;

		List<Pos> candidates = new List<Pos>();
		for (int x = startx; x <= endx; x++) {
			for (int y = starty; y <= endy; y++) {
				if (x == position.x && y == position.y) continue;
			
				Pos candidate = new Pos(x, y);
				if (map_manager.IsWalkable(candidate) && Pos.abs_dist(position, candidate) <= move_budget) {
					candidates.Add(candidate);
				}
			}
		}
		
		selectableMovementTiles = map_manager.get_paths(position, candidates, false, move_budget);
    }
	
	public bool hoveringValidMoveTile()
	{
		return getSelectableTilePath(grid_position) != null;
	}
	
	// Creates a list of all selectable tiles within a given radius of a position
    // Consider turning this static for enemy AI if this is the only method they need from this class
    public void CreateListOfSelectableActTiles()//, GameAgentAction action) 
	{
		Pos position = player_main.grid_pos;
		int range = player_main.currentAttack.range;
		
        selectableActTiles = new List<Pos>();
        nonselectableActTiles = new List<Pos>();

        int startx = position.x - range >= 0 ? position.x - range : 0;
        int endx = position.x + range < width ? position.x + range : width - 1;

        int starty = position.y - range >= 0 ? position.y - range : 0;
        int endy = position.y + range < height ? position.y + range : height - 1;

        // TODO figure out if you are going to create a new method that doesn't fuck around with the List<Path>
        //if (action == GameAgentAction.MeleeAttack || action == GameAgentAction.MagicAttackSingleTarget
        //    || action == GameAgentAction.RangedAttack || action == GameAgentAction.RangedAttackMultiShot) {

		for (int x = startx; x <= endx; x++) {
			for (int y = starty; y <= endy; y++) {
				if (x == position.x && y == position.y) continue;

				Pos candidate = new Pos(x, y);
				
				if (Pos.abs_dist(position, candidate) <= range) 
					
					if (map_manager.IsOccupied(candidate))
						selectableActTiles.Add(candidate);

					else if (map_manager.IsTraversable(candidate))
						nonselectableActTiles.Add(candidate);
			}
		}
    }
	
	public void CreateListOfSelectableInteractTiles()
	{
		Pos position = player_main.grid_pos;
		int range = 3;
		
        selectableActTiles = new List<Pos>();
        nonselectableActTiles = new List<Pos>();

        int startx = position.x - range >= 0 ? position.x - range : 0;
        int endx = position.x + range < width ? position.x + range : width - 1;

        int starty = position.y - range >= 0 ? position.y - range : 0;
        int endy = position.y + range < height ? position.y + range : height - 1;

        // TODO figure out if you are going to create a new method that doesn't fuck around with the List<Path>
        //if (action == GameAgentAction.MeleeAttack || action == GameAgentAction.MagicAttackSingleTarget
        //    || action == GameAgentAction.RangedAttack || action == GameAgentAction.RangedAttackMultiShot) {

		for (int x = startx; x <= endx; x++) {
			for (int y = starty; y <= endy; y++) {
				if (x == position.x && y == position.y) continue;

				Pos candidate = new Pos(x, y);
				
				if (Pos.abs_dist(position, candidate) <= range) 
					
					if (map_manager.IsInteractable(candidate))
						selectableActTiles.Add(candidate);

					else if (map_manager.IsTraversable(candidate))
						nonselectableActTiles.Add(candidate);
			}
		}
	}
	
	// disabling AOE while I test changes
	/*private void CreateActAOETiles() {
		Pos pos = player_main.grid_pos; // i know this isn't right, but... it's a quick fix
		
        actAOETiles = new List<Pos>();

        if (map_manager.IsTraversable(pos) && ((nonselectableActTiles.Count > 0 && nonselectableActTiles.Contains(pos)) ||
                                                (selectableActTiles.Count > 0 && selectableActTiles.Contains(pos)))) {
            actAOETiles.Add(pos);

            Pos topPos = new Pos(pos.x, pos.y + 1);
            if (map_manager.IsTraversable(topPos)) {
                actAOETiles.Add(topPos);
            }

            Pos bottomPos = new Pos(pos.x, pos.y - 1);
            if (map_manager.IsTraversable(bottomPos)) {
                actAOETiles.Add(bottomPos);
            }

            Pos leftPos = new Pos(pos.x - 1, pos.y);
            if (map_manager.IsTraversable(leftPos)) {
                actAOETiles.Add(leftPos);
            }

            Pos rightPos = new Pos(pos.x + 1, pos.y);
            if (map_manager.IsTraversable(rightPos)) {
                actAOETiles.Add(rightPos);
            }
        }
    }*/
	
	/*public bool hoveringValidActAOETile() {
        return GetActAOETile(grid_position) != null;
    }*/
	
	/*private Pos GetActAOETile(Pos tilePos) {
        foreach (Pos tile in actAOETiles)
            if (tile == tilePos)
                return tile;
        return null;
    }*/
	
	public bool hoveringValidActTile()
	{
		return GetSelectableActTile(grid_position) != null;
	}

    private Path getSelectableTilePath(Pos tile_pos) 
	{
        foreach(Path path in selectableMovementTiles)
			if (path.endPos() == tile_pos)
				return path;
		return null;
    }

    private Pos GetSelectableActTile(Pos tilePos) {
        foreach (Pos tile in selectableActTiles)
            if (tile == tilePos)
                return tile;
        return null;
    }

    public List<Pos> GetPositionOfAgentsInNonselectableActTiles() {
        List<Pos> agents = new List<Pos>();
        foreach (Pos tile in nonselectableActTiles) {
            if (map_manager.IsOccupied(tile)) {
                agents.Add(tile);
            }
        }
        return agents;
    }

    public List<Pos> GetPositionOfAgentsInActAOETiles() {
        List<Pos> agents = new List<Pos>();

        if (actAOETiles != null & actAOETiles.Count > 0) {
            foreach (Pos tile in actAOETiles) {
                if (map_manager.IsOccupied(tile)) {
                    agents.Add(tile);
                }
            }
        }
        return agents;
    }

    public List<Pos> GetActAOETiles() {
        return actAOETiles;
    }
	
	void DrawTiles() {
		clear_path_line();
        switch (mode) {
			case "MOVE":
				// render path line
				if (grid_position != null) {
					Path line_path = getSelectableTilePath(grid_position);
					if (line_path != null) {
						path_render.startColor = Color.white;
						path_render.endColor = Color.white;
						render_path_line(line_path);
					}
					else {
						Path alt = map_manager.get_path(player_main.grid_pos, grid_position);
						if (alt != null && !alt.empty()) {
							path_render.startColor = Color.red;
							path_render.endColor = Color.red;
							render_path_line(alt);
						}
					}
				}
				// render selectable tile indicators
				foreach (Path path in selectableMovementTiles) {
					Pos tile = path.endPos();
					Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), moveableTilesMaterial, 0);
				}
				break;
			case "INTERACT":
				foreach (Pos tile in selectableActTiles) {
					Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), interactTilesMaterial, 0);
				}
				foreach (Pos tile in nonselectableActTiles) {
					Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), nonselectableTilesMaterial, 0);
				}
				break;
			case "ACT":
				foreach (Pos tile in selectableActTiles) {
					Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), selectableTilesMaterial, 0);
				}
				foreach (Pos tile in nonselectableActTiles) {
					Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), nonselectableTilesMaterial, 0);
				}
				break;
			case "AOE":
				if (actAOETiles != null && actAOETiles.Count > 0) {
					foreach (Pos tile in actAOETiles) {
						Graphics.DrawMesh(tileMesh, map_manager.grid_to_world(tile) + Vector3.up * 0.1f, Quaternion.Euler(90, 0, 0), selectableTilesMaterial, 0);
					}
				}
				break;
		}
    }
}