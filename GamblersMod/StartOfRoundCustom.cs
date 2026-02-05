using Unity.Netcode;

namespace GamblersMod
{
    public class StartOfRoundCustom : NetworkBehaviour
    {
        private void Awake()
        {
        }

        [ServerRpc(RequireOwnership = false)]
        public void DespawnGamblingMachineServerRpc()
        {
            if (!IsServer)
            {
                return;
            }

            GamblingMachineManager.Instance.DespawnAll();
        }
    }
}
