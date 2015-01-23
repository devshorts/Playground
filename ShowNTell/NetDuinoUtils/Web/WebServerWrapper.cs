using System.Collections;

namespace NWebREST.Web
{
/// <summary>
/// Wrapper class on top of a multi threaded web server
/// Allows classes to register REST style endpoints
/// </summary>
public static class WebServerWrapper 
{
    private static WebServer _server;
    private static ArrayList _endPoints;

    /// <summary>
    /// Register REST endpoint for callback invocation with the web server
    /// </summary>
    /// <param name="endPoints"></param>
    private static void RegisterEndPoints(ArrayList endPoints)
    {
        if(_endPoints == null)
        {
            _endPoints = new ArrayList();
        }

        foreach(var endPoint in endPoints)
        {
            _endPoints.Add(endPoint);
        }
    }

    public static void InitializeWebEndPoints(ArrayList items)
    {
        foreach (IEndPointProvider endpoint in items)
        {
            endpoint.Initialize();
            RegisterEndPoints(endpoint.AvailableEndPoints());
        }
    }
    /// <summary>
    /// Start listening on the port and enable any registered callbacks
    /// </summary>
    /// <param name="port"></param>
    /// <param name="enabledLedStatus"></param>
    public static void StartWebServer(int port = 80, bool enabledLedStatus = true)
    {
        _server = new WebServer(port, enabledLedStatus);

        _server.EndPointReceived += EndPointHandler;

        foreach (EndPoint endpoint in _endPoints)
        {
            _server.RegisterEndPoint(endpoint);
        }
            

        // Initialize the server.
        _server.Start();
    }

    /// <summary>
    /// We'll get an endpoint invokcation from the web server
    /// so we can execute the endpoint action and response based on its supplied arguments
    /// in a seperate thread, hence the event. we'll set the event return string
    /// so the web server can know how to respond back to the ui in a seperate thread
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private static void EndPointHandler(object source, EndPoinEventArgs e)
    {
        var misc = new EndPointActionArguments
                        {
                            Connection = e.Connection
                        };

        e.ReturnString = e.Command.Execute(misc);

        // we can override the manual use of the socket if we returned a value other than null
        if (e.ReturnString != null && e.Command.UsesManualSocket)
        {
            e.ManualSent = false;
        }
        else
        {
            e.ManualSent = e.Command.UsesManualSocket;
        }
    }
}
}
