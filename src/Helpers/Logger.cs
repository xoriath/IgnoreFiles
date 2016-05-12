using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

public class OutputWindowTraceListener : TraceListener
{
    private string _name;
    private IVsOutputWindowPane pane;
    private object _syncRoot = new object();
    private IServiceProvider _provider;

    private OutputWindowTraceListener(IServiceProvider provider, string name)
    {
        _provider = provider;
        _name = name;
    }

    public static void Initialize(IServiceProvider provider, string name)
    {
        var logger = new OutputWindowTraceListener(provider, name);

        Trace.Listeners.Add(logger);
        Trace.AutoFlush = true;
    }

    public override void Write(string message)
    {
        if (EnsurePane())
        {
            pane.OutputString(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
        }
    }

    public override void WriteLine(string message)
    {
        Write(message);
    }

    private bool EnsurePane()
    {
        if (pane == null)
        {
            Guid guid = Guid.NewGuid();
            IVsOutputWindow output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
            output.CreatePane(ref guid, _name, 1, 1);
            output.GetPane(ref guid, out pane);
        }

        return pane != null;
    }
}