using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PediatricSim
{

	public class UIButton : Button
	{

		public override void OnPointerDown (UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (IsInteractable ()) {
				AudioManager.Singleton.PlayClickSound ();
				if(this.gameObject.name == "Lab")GameObject.Find("Database").GetComponent<Database>().Lab = true;
				else if(this.gameObject.name == "NonLab")GameObject.Find("Database").GetComponent<Database>().Lab = false;
			}
			base.OnPointerDown (eventData);
		}

	}

}