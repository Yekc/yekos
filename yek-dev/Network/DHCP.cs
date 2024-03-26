using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DHCP;

namespace Yek.Network
{
    public static class DHCP
    {
        public static void Release()
        {
            var xClient = new DHCPClient();
            xClient.SendReleasePacket();
            xClient.Close();

            NetworkConfiguration.ClearConfigs();
        }

        public static bool Ask()
        {
            var xClient = new DHCPClient();
            xClient.SendDiscoverPacket();
            xClient.Close();
            return true;
            /*
            if (xClient.SendDiscoverPacket() != -1)
            {
                xClient.Close();
                System.Console.WriteLine($"ip: {NetworkConfiguration.CurrentAddress}");
                return true;
            }
            else
            {
                NetworkConfiguration.ClearConfigs();
                xClient.Close();
                System.Console.WriteLine($"timed out :(");
                return false;
            }
            */
        }
    }
}
