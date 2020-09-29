using Grpc.Core;
using System;

namespace gRpcServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 9000;
            const string host = "0.0.0.0";

            Server server = new Server
            {
                Services = { DataInputt.ZeitService.Api.ZeitService.BindService(new ZeitService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine($"Starting server {host}:{port}");
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();
        }
    }
}
