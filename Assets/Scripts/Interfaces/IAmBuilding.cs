using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public interface IAmBuilding 
{
	public string id {get;}
	public GameObject[] inPorts { get;}
	public GameObject[] OutPorts { get;}
	void Init(string id){}
	void Proccessing(){}
}