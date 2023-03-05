using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointChanger : MonoBehaviour
{
	public bool TriggerPointActive = false;

    public void SetTriggerPoint(bool active)
	{
		TriggerPointActive = active;
	}

	public bool GetTriggerPoint()
	{
		return TriggerPointActive;
	}
}
