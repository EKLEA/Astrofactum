using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultScreenController : MonoBehaviour
{
	public GameObject buttonsGrid;
	List<ActionButton> buttons=new();
	public ActionButton button;
	void Start()
	{
		for (int i = 0;i<4;i++)
		{
			var gm =Instantiate(button,buttonsGrid.transform);
			gm.SetUpButton("Core",ActionTypes.EditTerrain);
			buttons.Add(gm);
		}
		
		EditWorldController.Instance.SetUpController(buttons);
		
	}
}
