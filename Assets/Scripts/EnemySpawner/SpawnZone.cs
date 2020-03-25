using MapUtils;
using System.Collections.Generic;
using UnityEngine;

// A number of tiles which enemies are able to spawn in
public class SpawnZone {
    // Center of the Spawn Zone
    private Pos position;
    // Reach of the Spawn Zone
    private float radius;
    // Traversable unpopulated tiles within the Spawn Zone
    private List<Pos> unpopulatedZoneTiles;
    // Populated tiles within the Spawn Zone
    private List<Pos> populatedZoneTiles;
    // Region
    public int region;

    private bool isPopulated = false;

    public SpawnZone(Pos position, float radius, int region) {
        this.position = position;
        this.radius = radius;
        this.region = region;
        unpopulatedZoneTiles = new List<Pos>();
        populatedZoneTiles = new List<Pos>();
    }

    // Returns the center of the Spawn Zone
    public Pos GetPosition() {
        return position;
    }

    // Returns the radius of the Spawn Zone
    public float GetRadius() {
        return radius;
    }

    // Sets the traversable tiles within the radius of the Spawn Zone
    public void SetZoneTiles(List<Pos> unpopulatedZoneTiles) {
        this.unpopulatedZoneTiles = unpopulatedZoneTiles;
    }

    // Gets the traversable tiles within the radius of the Spawn Zone
    public List<Pos> GetUnpopulatedZoneTiles() {
        return unpopulatedZoneTiles;
    }

    // Returns the number of unpopulated traversable tiles within the radius
    // of the Spawn Zone
    public int GetNumberOfUnpopulatedTilesInZone() {
        return unpopulatedZoneTiles.Count;
    }

    // Returns the number of populated tiles within the radius
    // of the Spawn Zone
    public int GetNumberOfPopulatedTilesInZone() {
        return populatedZoneTiles.Count;
    }

    public bool IsPopulated() {
        return isPopulated;
    }

    // Creates tiles that are populated with objects
    public void PopulateTiles(List<Pos> populatedZoneTiles) {
        if (populatedZoneTiles.Count > 0) {
            isPopulated = true;
            this.populatedZoneTiles = populatedZoneTiles;
        }
    }

    public void SetPosition (Pos pos) {
        position = pos;
    }
}
