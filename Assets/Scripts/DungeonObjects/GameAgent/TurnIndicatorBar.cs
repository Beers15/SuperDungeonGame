using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicatorBar : MonoBehaviour
{
	public float Spacing = 64.0f;

	private List<TurnIndicator> turnIndicators;

    // Start is called before the first frame update
    void Start()
    {
		turnIndicators = new List<TurnIndicator>();
    }

	public void AddTurnIndicator(TurnIndicator indicator)
	{
		turnIndicators.Add(indicator);
		GenerateLayout();
	}

	public void RemoveTurnIndicator(TurnIndicator indicator)
	{
		turnIndicators.Remove(indicator);
		GenerateLayout();
	}

	private void GenerateLayout()
	{
		float x = -(turnIndicators.Count - 1) * Spacing / 2.0f;
		for (int i = 0; i < turnIndicators.Count; ++i)
		{
			turnIndicators[i].transform.position = transform.position + new Vector3(x, 0, 0);
			x += Spacing;
		}
	}
}
