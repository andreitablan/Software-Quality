using Microsoft.AspNetCore.SignalR;
using ProjectSQ.Interfaces.Processor;
using ProjectSQ.Models;

public class RealTimeHub : Hub
{
    private readonly IProcessorService processorService;

    public RealTimeHub(IProcessorService processorService)
    {
        this.processorService = processorService;
    }

    public async Task SendMessage(string message)
    {
        char character = message[0];
        processorService.WriteValueToKeyboardBuffer((ushort)character);
        var videoMemoryValue = processorService.ReadFromVideoMemory();
        await Clients.All.SendAsync("ReceiveMessage", videoMemoryValue);
    }

    public async Task SendBackspace()
    {
        processorService.RemoveFromVideoMemory();
        var videoMemoryValue = processorService.ReadFromVideoMemory();
        await Clients.All.SendAsync("ReceiveMessage", videoMemoryValue);
    }

    public async Task SendWipeVideoMemory()
    {
        Memory.WipeVideoMemory();
    }

    public async Task ReadNumber(string number)
    {
        var pair = number.Split('-');

        var operand = pair[0];

        number = pair[1];
        foreach (var item in number)
        {
            processorService.WriteValueToKeyboardBuffer(item);
        }

        //processorService.SaveTo
        //var videoMemoryValue = processorService.ReadFromVideoMemory();
        //await Clients.All.SendAsync("ReceiveMessage", videoMemoryValue);
    }
}