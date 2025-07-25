using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;

namespace PowerShellRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            // Input parameter
            string name = args.Length > 0 ? args[0] : "Emre";

            // Read PowerShell script from file
            string scriptPath = "Hello.ps1"; // Ensure this file exists in the same folder as the EXE
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"Script file '{scriptPath}' not found.");
                return;
            }

            string scriptBody = File.ReadAllText(scriptPath);

            var parameters = new Dictionary<string, object>
            {
                { "name", name }
            };

            using var controller = new RunspaceController();
            Collection<PSObject> results = controller.RunScript("HelloScript", scriptBody, parameters);

            Console.WriteLine("PowerShell Output:");
            foreach (var output in results)
            {
                Console.WriteLine(output?.ToString());
            }
        }
    }
}
