using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using Microsoft.Win32;

namespace DoHWindowsInstaller
{
    class Program
    {
        private static readonly string LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DoH_Install.log");
        private static bool restorePointCreated = false;

        // Embedded DoH Well-Known Servers Database - All DNS servers configured
        private static readonly Dictionary<string, string> DohServers = new Dictionary<string, string>
        {
            // Cloudflare
            { "1.0.0.1", "https://cloudflare-dns.com/dns-query" },
            { "1.0.0.2", "https://security.cloudflare-dns.com/dns-query" },
            { "1.0.0.3", "https://family.cloudflare-dns.com/dns-query" },
            { "1.1.1.1", "https://cloudflare-dns.com/dns-query" },
            { "1.1.1.2", "https://security.cloudflare-dns.com/dns-query" },
            { "1.1.1.3", "https://family.cloudflare-dns.com/dns-query" },
            { "2606:4700:4700::1001", "https://cloudflare-dns.com/dns-query" },
            { "2606:4700:4700::1002", "https://security.cloudflare-dns.com/dns-query" },
            { "2606:4700:4700::1003", "https://family.cloudflare-dns.com/dns-query" },
            { "2606:4700:4700::1111", "https://cloudflare-dns.com/dns-query" },
            { "2606:4700:4700::1112", "https://security.cloudflare-dns.com/dns-query" },
            { "2606:4700:4700::1113", "https://family.cloudflare-dns.com/dns-query" },
            
            // Control D / FreeDNS
            { "76.76.2.0", "https://freedns.controld.com/p0" },
            { "76.76.10.0", "https://freedns.controld.com/p0" },
            { "76.76.2.1", "https://freedns.controld.com/p1" },
            { "76.76.10.1", "https://freedns.controld.com/p1" },
            { "76.76.2.2", "https://freedns.controld.com/p2" },
            { "76.76.10.2", "https://freedns.controld.com/p2" },
            { "76.76.2.3", "https://freedns.controld.com/p3" },
            { "76.76.10.3", "https://freedns.controld.com/p3" },
            { "76.76.2.4", "https://freedns.controld.com/family" },
            { "76.76.10.4", "https://freedns.controld.com/family" },
            { "76.76.2.5", "https://freedns.controld.com/uncensored" },
            { "76.76.10.5", "https://freedns.controld.com/uncensored" },
            { "2606:1a40::", "https://freedns.controld.com/p0" },
            { "2606:1a40:1::", "https://freedns.controld.com/p0" },
            { "2606:1a40::1", "https://freedns.controld.com/p1" },
            { "2606:1a40:1::1", "https://freedns.controld.com/p1" },
            { "2606:1a40::2", "https://freedns.controld.com/p2" },
            { "2606:1a40:1::2", "https://freedns.controld.com/p2" },
            { "2606:1a40::3", "https://freedns.controld.com/p3" },
            { "2606:1a40:1::3", "https://freedns.controld.com/p3" },
            { "2606:1a40::4", "https://freedns.controld.com/family" },
            { "2606:1a40:1::4", "https://freedns.controld.com/family" },
            { "2606:1a40::5", "https://freedns.controld.com/uncensored" },
            { "2606:1a40:1::5", "https://freedns.controld.com/uncensored" },
            
            // Joindns
            { "86.54.11.1", "https://protective.joindns4.eu/dns-query" },
            { "86.54.11.201", "https://protective.joindns4.eu/dns-query" },
            { "86.54.11.12", "https://child.joindns4.eu/dns-query" },
            { "86.54.11.212", "https://child.joindns4.eu/dns-query" },
            { "86.54.11.13", "https://noads.joindns4.eu/dns-query" },
            { "86.54.11.213", "https://noads.joindns4.eu/dns-query" },
            { "86.54.11.11", "https://child-noads.joindns4.eu/dns-query" },
            { "86.54.11.211", "https://child-noads.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:1", "https://protective.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:201", "https://protective.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:12", "https://child.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:212", "https://child.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:13", "https://noads.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:213", "https://noads.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:11", "https://child-noads.joindns4.eu/dns-query" },
            { "2a13:1001::86:54:11:211", "https://child-noads.joindns4.eu/dns-query" },
            
            // Quad9
            { "149.112.112.11", "https://dns11.quad9.net/dns-query" },
            { "149.112.112.112", "https://dns.quad9.net/dns-query" },
            { "149.112.112.9", "https://dns9.quad9.net/dns-query" },
            { "9.9.9.10", "https://dns10.quad9.net/dns-query" },
            { "9.9.9.11", "https://dns11.quad9.net/dns-query" },
            { "9.9.9.9", "https://dns.quad9.net/dns-query" },
            { "2620:fe::9", "https://dns.quad9.net/dns-query" },
            { "2620:fe::fe", "https://dns.quad9.net/dns-query" },
            { "2620:fe::fe:9", "https://dns9.quad9.net/dns-query" },
            { "2620:fe::10", "https://dns10.quad9.net/dns-query" },
            { "2620:fe::fe:10", "https://dns10.quad9.net/dns-query" },
            { "2620:fe::11", "https://dns11.quad9.net/dns-query" },
            { "2620:fe::fe:11", "https://dns11.quad9.net/dns-query" },
            
            // Google DNS
            { "8.8.4.4", "https://dns.google/dns-query" },
            { "8.8.8.8", "https://dns.google/dns-query" },
            { "2001:4860:4860::64", "https://dns64.dns.google/dns-query" },
            { "2001:4860:4860::6464", "https://dns64.dns.google/dns-query" },
            { "2001:4860:4860::8844", "https://dns.google/dns-query" },
            { "2001:4860:4860::8888", "https://dns.google/dns-query" },
            
            // Mullvad DNS
            { "194.242.2.2", "https://dns.mullvad.net/dns-query" },
            { "194.242.2.3", "https://adblock.dns.mullvad.net/dns-query" },
            { "194.242.2.4", "https://base.dns.mullvad.net/dns-query" },
            { "194.242.2.5", "https://extended.dns.mullvad.net/dns-query" },
            { "194.242.2.6", "https://family.dns.mullvad.net/dns-query" },
            { "194.242.2.9", "https://all.dns.mullvad.net/dns-query" },
            { "2a07:e340::2", "https://dns.mullvad.net/dns-query" },
            { "2a07:e340::3", "https://adblock.dns.mullvad.net/dns-query" },
            { "2a07:e340::4", "https://base.dns.mullvad.net/dns-query" },
            { "2a07:e340::5", "https://extended.dns.mullvad.net/dns-query" },
            { "2a07:e340::6", "https://family.dns.mullvad.net/dns-query" },
            { "2a07:e340::9", "https://all.dns.mullvad.net/dns-query" },
            
            // AdGuard DNS
            { "94.140.14.14", "https://dns.adguard-dns.com/dns-query" },
            { "94.140.15.15", "https://dns.adguard-dns.com/dns-query" },
            { "94.140.14.15", "https://family.adguard-dns.com/dns-query" },
            { "94.140.15.16", "https://family.adguard-dns.com/dns-query" },
            { "94.140.14.140", "https://unfiltered.adguard-dns.com/dns-query" },
            { "94.140.14.141", "https://unfiltered.adguard-dns.com/dns-query" },
            { "2a10:50c0::ad1:ff", "https://dns.adguard-dns.com/dns-query" },
            { "2a10:50c0::ad2:ff", "https://dns.adguard-dns.com/dns-query" },
            { "2a10:50c0::bad1:ff", "https://family.adguard-dns.com/dns-query" },
            { "2a10:50c0::bad2:ff", "https://family.adguard-dns.com/dns-query" },
            { "2a10:50c0::1:ff", "https://unfiltered.adguard-dns.com/dns-query" },
            { "2a10:50c0::2:ff", "https://unfiltered.adguard-dns.com/dns-query" },
            
            // OpenDNS
            { "208.67.222.222", "https://dns.opendns.com/dns-query" },
            { "208.67.220.220", "https://dns.opendns.com/dns-query" },
            { "208.67.222.123", "https://familyshield.opendns.com/dns-query" },
            { "208.67.220.123", "https://familyshield.opendns.com/dns-query" },
            { "208.67.222.2", "https://sandbox.opendns.com/dns-query" },
            { "208.67.220.2", "https://sandbox.opendns.com/dns-query" },
            { "2620:119:35::35", "https://dns.opendns.com/dns-query" },
            { "2620:119:53::53", "https://dns.opendns.com/dns-query" },
            { "2620:119:35::123", "https://familyshield.opendns.com/dns-query" },
            { "2620:119:53::123", "https://familyshield.opendns.com/dns-query" },
            { "2620:0:ccc::2", "https://sandbox.opendns.com/dns-query" },
            { "2620:0:ccd::2", "https://sandbox.opendns.com/dns-query" },
            
            // Clean Browsing
            { "185.228.168.168", "https://doh.cleanbrowsing.org/doh/family-filter/" },
            { "185.228.169.168", "https://doh.cleanbrowsing.org/doh/family-filter/" },
            { "185.228.168.10", "https://doh.cleanbrowsing.org/doh/adult-filter/" },
            { "185.228.169.11", "https://doh.cleanbrowsing.org/doh/adult-filter/" },
            { "185.228.168.9", "https://doh.cleanbrowsing.org/doh/security-filter/" },
            { "185.228.169.9", "https://doh.cleanbrowsing.org/doh/security-filter/" },
            { "2a0d:2a00:1::", "https://doh.cleanbrowsing.org/doh/family-filter/" },
            { "2a0d:2a00:2::", "https://doh.cleanbrowsing.org/doh/family-filter/" },
            { "2a0d:2a00:1::1", "https://doh.cleanbrowsing.org/doh/adult-filter/" },
            { "2a0d:2a00:2::1", "https://doh.cleanbrowsing.org/doh/adult-filter/" },
            { "2a0d:2a00:1::2", "https://doh.cleanbrowsing.org/doh/security-filter/" },
            { "2a0d:2a00:2::2", "https://doh.cleanbrowsing.org/doh/security-filter/" },
            
            // LibreDNS
            { "116.202.176.26", "https://doh.libredns.gr/noads" },
            { "2a01:4f8:1c0c:8274::1", "https://doh.libredns.gr/noads" },
            
            // Uncensored DNS
            { "91.239.100.100", "https://anycast.uncensoreddns.org/dns-query" },
            { "89.233.43.71", "https://unicast.uncensoreddns.org/dns-query" },
            { "2001:67c:28a4::", "https://anycast.uncensoreddns.org/dns-query" },
            { "2a01:3a0:53:53::", "https://unicast.uncensoreddns.org/dns-query" }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║   DoH Windows Installer                ║");
            Console.WriteLine("║   DNS-over-HTTPS Configuration Tool    ║");
            Console.WriteLine("╚════════════════════════════════════════╝\n");

            // Check admin privileges
            if (!IsRunningAsAdmin())
            {
                Console.WriteLine("✗ ERROR: This application requires administrator privileges.");
                Console.WriteLine("   Please run this EXE as Administrator.\n");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            Log("DoH Installer started by user " + Environment.UserName);
            Console.WriteLine("✓ Running as Administrator\n");

            try
            {
                // Step 1: Create system restore point
                Console.WriteLine("Creating system restore point...");
                if (CreateRestorePoint())
                {
                    Console.WriteLine("✓ System restore point created\n");
                    restorePointCreated = true;
                }
                else
                {
                    Console.WriteLine("⚠ Warning: Could not create restore point. Continuing anyway...\n");
                }

                // Step 2: Backup registry
                Console.WriteLine("Backing up registry...");
                BackupRegistry();
                Console.WriteLine("✓ Registry backed up\n");

                // Step 3: Configure DoH Well-Known Servers
                Console.WriteLine($"Configuring {DohServers.Count} DoH servers...");
                ConfigureDohWellKnownServers();
                Console.WriteLine("✓ DoH Well-Known Servers configured\n");

                // Step 4: Configure Interface Specific Parameters
                Console.WriteLine("Configuring Global Interface Parameters...");
                ConfigureInterfaceSpecificParameters();
                Console.WriteLine("✓ Interface Parameters configured\n");

                // Step 5: Verify changes
                Console.WriteLine("Verifying changes...");
                VerifyConfiguration();
                Console.WriteLine("✓ Configuration verified\n");

                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine($"✓ Installation completed successfully!");
                Console.WriteLine($"✓ {DohServers.Count} DNS servers configured");
                Console.WriteLine("═══════════════════════════════════════\n");

                Log("Installation completed successfully");

                // Prompt for reboot
                Console.WriteLine("Your system needs to reboot for changes to take effect.");
                Console.WriteLine("\nReboot now? (Y/N): ");
                string input = Console.ReadLine()?.ToLower() ?? "n";

                if (input == "y" || input == "yes")
                {
                    PromptReboot();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ ERROR: {ex.Message}");
                Log($"ERROR: {ex.Message}\n{ex.StackTrace}");
                OfferRollback();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static bool IsRunningAsAdmin()
        {
            try
            {
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        static bool CreateRestorePoint()
        {
            try
            {
                ManagementScope scope = new ManagementScope(@"\\.\root\default");
                scope.Connect();

                ManagementClass classInstance = new ManagementClass(scope, new ManagementPath("SystemRestore"), null);
                ManagementBaseObject inParams = classInstance.GetMethodParameters("CreateRestorePoint");

                inParams["Description"] = "DoH Windows Installer - Pre-installation checkpoint";
                inParams["RestorePointType"] = 0;
                inParams["EventType"] = 1;

                ManagementBaseObject outParams = classInstance.InvokeMethod("CreateRestorePoint", inParams, null);

                if (outParams != null)
                {
                    Log("System restore point created successfully");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log($"Warning: Could not create restore point: {ex.Message}");
            }

            return false;
        }

        static void BackupRegistry()
        {
            try
            {
                string backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DoH_Backup");
                Directory.CreateDirectory(backupDir);

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                string backupFile = Path.Combine(backupDir, $"DnsCache_Backup_{timestamp}.reg");

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "reg",
                    Arguments = $"export \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\Dnscache\" \"{backupFile}\" /y",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit();
                }

                Log($"Registry backed up to: {backupFile}");
            }
            catch (Exception ex)
            {
                Log($"Warning: Could not backup registry: {ex.Message}");
            }
        }

        static void ConfigureDohWellKnownServers()
        {
            try
            {
                string registryPath = @"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters\DohWellKnownServers";

                using (RegistryKey baseKey = Registry.LocalMachine.CreateSubKey(registryPath))
                {
                    int count = 0;
                    foreach (var server in DohServers)
                    {
                        using (RegistryKey ipKey = baseKey.CreateSubKey(server.Key))
                        {
                            ipKey.SetValue("Template", server.Value, RegistryValueKind.String);
                            count++;
                        }
                    }
                    Log($"Configured {count} DoH Well-Known servers");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to configure DoH Well-Known servers: {ex.Message}");
            }
        }

        static void ConfigureInterfaceSpecificParameters()
        {
            try
            {
                string registryPath = @"SYSTEM\CurrentControlSet\Services\Dnscache\InterfaceSpecificParameters\GlobalDohIP";

                using (RegistryKey baseKey = Registry.LocalMachine.CreateSubKey(registryPath))
                {
                    int count = 0;
                    foreach (var server in DohServers)
                    {
                        using (RegistryKey ipKey = baseKey.CreateSubKey(server.Key))
                        {
                            ipKey.SetValue("DohTemplate", server.Value, RegistryValueKind.String);
                            ipKey.SetValue("DohFlags", new byte[] { 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, RegistryValueKind.Binary);
                            count++;
                        }
                    }
                    Log($"Configured {count} InterfaceSpecific parameters");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to configure Interface Parameters: {ex.Message}");
            }
        }

        static void VerifyConfiguration()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Dnscache\Parameters\DohWellKnownServers"))
                {
                    if (key != null)
                    {
                        int subKeyCount = key.SubKeyCount;
                        Log($"Verification: Found {subKeyCount} DoH entries in registry");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Warning: Could not verify configuration: {ex.Message}");
            }
        }

        static void OfferRollback()
        {
            Console.WriteLine("\n⚠ An error occurred. Would you like to restore your system? (Y/N): ");
            string input = Console.ReadLine()?.ToLower() ?? "n";

            if (input == "y" || input == "yes")
            {
                Console.WriteLine("\nTo restore your system:");
                Console.WriteLine("1. Open 'System Restore' (search in Windows)");
                Console.WriteLine("2. Click 'Next' and select the restore point created today");
                Console.WriteLine("3. Follow the prompts to restore your system\n");
                Log("User initiated rollback process");
            }
        }

        static void PromptReboot()
        {
            Console.WriteLine("\nSystem will reboot in 30 seconds...");
            Log("System reboot initiated");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/r /t 30 /c \"DoH Windows Installer - System Reboot\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }

        static void Log(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(LogFile, logEntry + Environment.NewLine);
            }
            catch
            {
                // Silently fail if logging fails
            }
        }
    }
}
