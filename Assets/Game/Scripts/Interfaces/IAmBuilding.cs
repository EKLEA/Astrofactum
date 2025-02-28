using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public interface IAmBuilding 
{
	public string id {get;}
	public bool IsWork {get;}
	abstract void Init(string id);
	abstract void Proccessing();
	abstract IEnumerator ProccessingCoroutine();
}