using Corno.Data.Dtos.Import;
using Microsoft.AspNet.SignalR;

namespace Corno.OnlineExam.Hubs;

public class ProgressHub : Hub
{
    /*public void SendProgress(int progress)
    {
        Clients.All.receiveProgress(progress);
    }*/

    public void SendProgress(ProgressDto dto)
    {
        Clients.All.receiveProgress(dto);
    }
}