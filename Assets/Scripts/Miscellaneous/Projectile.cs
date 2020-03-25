using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private Vector3 start, end;
	private float distance;
	private bool initialized = false;
	
	public bool reachedDestination = false;
	
    public void Init(Vector3 start, Vector3 end) { 
		this.start = start;
		this.end = end;
		this.distance = Vector3.Distance(start, end);
		initialized = true;
		transform.LookAt(end);
	}
	
	private float progress = 0f;

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;
		else if (progress >= 1f) {
			reachedDestination = true;
			initialized = false;
			Destroy(this.gameObject); // applies slight delay to destruction so that reachedDestination variable can be read in time
		}
		
		transform.position = Vector3.Lerp(start, end, progress);
		progress += 1f / (distance * 5f);
    }
}
