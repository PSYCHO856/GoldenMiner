/****************************************************************************
 * 2020.11 DESKTOP-SJGOCT1
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public partial class PlayPanelMap
	{
		[SerializeField] public UnityEngine.UI.Image MoveIcon;
		[SerializeField] public UnityEngine.RectTransform StartPoint;
		[SerializeField] public UnityEngine.RectTransform EndPoint;

		public void Clear()
		{
			MoveIcon = null;
			StartPoint = null;
			EndPoint = null;
		}

		public override string ComponentName
		{
			get { return "PlayPanelMap";}
		}
	}
}
