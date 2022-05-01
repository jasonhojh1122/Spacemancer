using UnityEngine;
using System.Collections;
using Steamworks;

namespace Steam
{
	public class SteamScript : MonoBehaviour
	{
		protected Callback<GameOverlayActivated_t> gameOverlayActivated;

		private void OnEnable()
		{
			if (SteamManager.Initialized)
			{
				gameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlay);
			}
		}

		void OnGameOverlay(GameOverlayActivated_t callback)
		{
			if (callback.m_bActive != 0)
			{
				Input.InputManager.Instance.pause = true;
			}
			else
			{
				Input.InputManager.Instance.pause = false;
			}
		}
	}

}