using System.Diagnostics;
using System.Net.NetworkInformation;

public static class VPN
{
    // Connects to a VPN using the provided connection name, username, and password
    static public void ConnectVPN(string vpnConnectionName, string vpnUsername, string vpnPassword)
    {
        string vpnCommand = $"rasdial \"{vpnConnectionName}\" {vpnUsername} {vpnPassword}";

        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + vpnCommand);
        using (Process process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();
            process.WaitForExit();
        }

        Console.WriteLine("VPN connection established successfully.");
        Thread.Sleep(6000);
    }

    // Checks if a VPN with the given name is connected
    static public bool IsVpnConnected(string vpnName)
    {
        // Get a list of all network interfaces
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        // Iterate through all interfaces to find a VPN connection with the specified name
        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.Name == vpnName && networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                return true; // Return true if a match is found and the interface is up
            }
        }

        return false; // Return false if no VPN is found after checking all interfaces
    }

    // Disconnects from a VPN using the provided connection name
    static public void DisconnectVPN(string vpnConnectionName)
    {
        string vpnCommand = $"rasdial {vpnConnectionName} /disconnect";

        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + vpnCommand);
        using (Process process = new Process())
        {
            process.StartInfo = processInfo;
            process.Start();
            process.WaitForExit();
        }

        Console.WriteLine("VPN connection disconnected successfully.");
        Thread.Sleep(2000);
    }

    // Adds a VPN connection with the specified connection name, username, and password
    public static void AddVPN(string vpnConnection, string vpnUsername, string vpnPassword)
    {
        // Adding a VPN connection
        var addVpnConnectionCmd = $"Add-VpnConnection -Name \"{vpnConnection}\" -ServerAddress \"vpn121127439.opengw.net\" -TunnelType Automatic -AuthenticationMethod MSChapv2 -PassThru";
        ExecutePowerShellCommand(addVpnConnectionCmd);

        // Setting the username and password for the VPN connection
        var setVpnConnectionUsernamePasswordCmd = $"$user = \"{vpnUsername}\"\n$plainpass = \"{vpnPassword}\"\nSet-VpnConnectionUsernamePassword -connectionname \"{vpnConnection}\" -username $user -password $plainpass";
        ExecutePowerShellCommand(setVpnConnectionUsernamePasswordCmd);
    }

    // Executes a PowerShell command
    private static void ExecutePowerShellCommand(string command)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-Command \"{command}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        using var process = new Process { StartInfo = processStartInfo };
        process.Start();
        process.WaitForExit();
    }
}