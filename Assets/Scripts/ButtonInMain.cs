using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PediatricSim
{

	public class ButtonInMain : Button
	{

		public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (IsInteractable())
			{
				
				AudioManager.Singleton.PlayClickSound();
				GameObject.Find("Chat").GetComponent<MainMessages>().ChatButton(this.gameObject);
			}
			base.OnPointerDown(eventData);
		}

	}

}
