using UnityEngine;

namespace NaturalSelection.Mirror.Game.Lobby
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private NetworkManagerLobby networkMananger = null;

        [Header("UI")]
        [SerializeField] private GameObject landingPagePanel = null;

        public void HostLobby()
        {
            networkMananger.StartHost();

            landingPagePanel.SetActive(false);
        }
    }
}
