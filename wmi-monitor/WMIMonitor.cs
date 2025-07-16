using System;
using System.Management;
using System.Diagnostics;
using System.Runtime.Versioning;

class WMIMonitor
{
    public static void MonitorWMIActivity()
    {
        try
        {
            var query = new WqlEventQuery(
                "SELECT * FROM __InstanceOperationEvent " +
                "WITHIN 1 " +
                "WHERE TargetInstance ISA '__Win32Provider'"
            );

            var watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += (sender, e) =>
            {
                var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                Console.WriteLine($"Provider: {targetInstance["Name"]}");
                Console.WriteLine($"Namespace: {targetInstance["Namespace"]}");
                Console.WriteLine($"Host Process: {targetInstance["HostProcessIdentifier"]}");
            };

            watcher.Start();
            Console.WriteLine("Monitoring WMI activity... Press Enter to stop.");
            Console.ReadLine();
            watcher.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}