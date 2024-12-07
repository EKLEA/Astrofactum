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
			var gm =Instantiate(button,buttonsGrid.transform);
			gm.SetUpButton("core",ActionTypes.EditTerrain);
			buttons.Add(gm);
			
			gm =Instantiate(button,buttonsGrid.transform);
			gm.SetUpButton("foundation",ActionTypes.BuildFoundation);
			buttons.Add(gm);
		
		EditWorldController.Instance.SetUpController(buttons);
		
	}
}
