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
			gm.SetUpButton("core");
			buttons.Add(gm);
			
			gm =Instantiate(button,buttonsGrid.transform);
			gm.SetUpButton("foundation");
			buttons.Add(gm);
		
		EditWorldController.Instance.SetUpController(buttons);
		
	}
}
