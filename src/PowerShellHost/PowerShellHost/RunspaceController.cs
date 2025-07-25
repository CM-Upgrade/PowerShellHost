using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Xml.Linq;

public class RunspaceController : IDisposable
{
    private Runspace runspace;
    private Pipeline pipeline;

    public RunspaceController()
    {
        runspace = RunspaceFactory.CreateRunspace();
        runspace.Open();
    }

    public Collection<PSObject> RunScript(string scriptName, string scriptBody, Dictionary<string, object> parameters)
    {
        // Step 1: Define a named PowerShell function from the script body
        string scriptFunctionName = $"Script_{Guid.NewGuid().ToString("N")}";
        string functionDefinition = $"function {scriptFunctionName} {{ {scriptBody} }}";

        using (var setupPipeline = runspace.CreatePipeline())
        {
            setupPipeline.Commands.Add(new Command(functionDefinition, true));
            setupPipeline.Invoke();  // Register the function in the session
        }

        // Step 2: Invoke the function with parameters
        pipeline = runspace.CreatePipeline();
        var command = new Command(scriptFunctionName, false);

        foreach (var param in parameters)
        {
            command.Parameters.Add(param.Key, param.Value);
        }

        // Merge errors with output
        command.MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
        pipeline.Commands.Add(command);

        return pipeline.Invoke();
    }

    public void Dispose()
    {
        pipeline?.Dispose();
        if (runspace != null)
        {
            runspace.Dispose();
            runspace = null;
        }
    }
}
