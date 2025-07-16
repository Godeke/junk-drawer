using System;
using System.Management;
using System.Diagnostics;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")] // Indicates that this code is Windows-specific

class WMIMonitor
{
    // Main entry point for the application
    static void Main(string[] args)
    {
        // Call the method to start monitoring WMI activity
        MonitorWMIActivity();
    }

    /// <summary>
    /// Monitors WMI activity, specifically focusing on new process creation events.
    /// </summary>
    public static void MonitorWMIActivity()
    {
        try
        {
            // Define the WQL (WMI Query Language) event query.
            // This query listens for '__InstanceCreationEvent' events,
            // which occur when a new instance of a class is created.
            // 'WITHIN 1' specifies that events should be delivered within 1 second of occurrence.
            // 'TargetInstance ISA 'Win32_Process'' filters these events to only include
            // new instances of the 'Win32_Process' class, meaning new processes being started.
            var query = new WqlEventQuery(
                "SELECT * FROM __InstanceCreationEvent " +
                "WITHIN 1 " +
                "WHERE TargetInstance ISA 'Win32_Process'"
            );

            // Create a ManagementEventWatcher to listen for WMI events.
            // This object will monitor for events matching the defined query.
            var watcher = new ManagementEventWatcher(query);

            // Attach an event handler to the 'EventArrived' event.
            // This lambda expression will be executed every time a new event matching the query occurs.
            watcher.EventArrived += (sender, e) =>
            {
                // The 'NewEvent' property of the EventArrivedEventArgs contains the WMI event object.
                // For an __InstanceCreationEvent, 'TargetInstance' holds the newly created instance.
                // We cast it to ManagementBaseObject to access its properties.
                var newProcess = (ManagementBaseObject)e.NewEvent["TargetInstance"];

                // Extract and print relevant information about the new process.
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine($"New Process Created:");
                Console.WriteLine($"  Name: {newProcess["Name"]}"); // Name of the executable
                Console.WriteLine($"  Process ID: {newProcess["ProcessId"]}"); // Unique ID of the process
                Console.WriteLine($"  Executable Path: {newProcess["ExecutablePath"]}"); // Full path to the executable
                Console.WriteLine($"  Command Line: {newProcess["CommandLine"]}"); // Command line arguments
                Console.WriteLine($"  Parent Process ID: {newProcess["ParentProcessId"]}"); // ID of the process that launched this one
                Console.WriteLine("--------------------------------------------------");
            };

            // Start the watcher. This begins listening for events.
            watcher.Start();
            Console.WriteLine("Monitoring WMI activity (new process creation)... Press Enter to stop.");

            // Keep the console application running until the user presses Enter.
            Console.ReadLine();

            // Stop the watcher when the user presses Enter, releasing resources.
            watcher.Stop();
            Console.WriteLine("Monitoring stopped.");
        }
        catch (PlatformNotSupportedException)
        {
            Console.WriteLine("This application requires Windows to run WMI queries.");
        }
        catch (Exception ex)
        {
            // Catch and display any exceptions that occur during the WMI monitoring process.
            Console.WriteLine($"An error occurred: {ex.Message}");
            // For more detailed error information, you might want to print ex.StackTrace
        }
    }
}
