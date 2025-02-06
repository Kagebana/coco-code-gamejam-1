using System;
using UnityEngine;

namespace Src.Interaction.Dialog.Orders
{
	[Serializable]
	public class DialogueLinesGroup
	{
		[TextArea(3, 10)] public string[] Lines;
	}
}