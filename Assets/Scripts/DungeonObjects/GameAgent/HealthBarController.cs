using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public GameObject healthbar;
    HealthBar bar;
	Text nickname;
    private static GameObject canvas;
    GameObject instance;
    GameAgent parent;
	
	public void Disable()
	{
		instance.SetActive(false);
	}
	
	public void Enable()
	{
		instance.SetActive(true);
	}

    void Start()
    {
        canvas = GameObject.Find("HPBars"); //get canvas ref

        //Debug.Log("I am an upset child");
        instance = Instantiate(healthbar); //instantiate prefab
        Vector2 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        parent = GetComponent<GameAgent>();
        bar = instance.GetComponentInChildren<HealthBar>();
		nickname = instance.GetComponentInChildren<Text>();
		nickname.text = parent.nickname;

        instance.transform.SetParent(canvas.transform, false); //set canvas as parent
        instance.transform.position = screenPos;

        bar.SetSliderValue(1);
        bar.SetMPSliderValue(1);

        float barwidth = Mathf.Min(parent.stats.maxHealth, parent.stats.maxMagicPoints, 40);
		RectTransform barTransform = instance.transform.Find("HPBar").GetComponent<RectTransform>();
		barTransform.sizeDelta = new Vector2(barwidth, 15);
        RectTransform barTransform2 = instance.transform.Find("MPBar").GetComponent<RectTransform>();
        barTransform2.sizeDelta = new Vector2(barwidth, 10);
    }

    void LateUpdate()
    {
        bar.SetSliderValue(parent.stats.currentHealth / parent.stats.maxHealth);
        bar.SetMPSliderValue(parent.stats.currentMagicPoints / parent.stats.maxMagicPoints);
		float camera_zoom_ratio = 10 / CameraControl.currentZoom;
        Vector3 offset = new Vector3(0, 40, 0) * camera_zoom_ratio;
        Vector3 wantedPosition = Camera.main.WorldToScreenPoint(gameObject.transform.position) + offset;
		bar.transform.localScale = Vector3.one * camera_zoom_ratio;
        instance.transform.position = wantedPosition;
		if (parent.turn_over()) {
			nickname.color = Color.red;
		}
		else {
			nickname.color = Color.white;
		}
    }

    private void OnDestroy()
    {
        Destroy(instance);
    }
}
