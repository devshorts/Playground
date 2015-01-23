using System.Collections;

namespace NWebREST.Web
{
public interface IEndPointProvider
{
    void Initialize();
    ArrayList AvailableEndPoints();
}
}
