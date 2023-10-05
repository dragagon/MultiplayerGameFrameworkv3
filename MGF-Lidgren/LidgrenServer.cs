using Lidgren.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiplayerGameFramework.Config;
using MultiplayerGameFramework.Interfaces;

namespace MGF_Lidgren
{
    public class LidgrenServer : BackgroundService, IServerApplication, IDisposable
    {
        private readonly ILogger logger;
        private readonly ServerConfiguration config;
        private static NetServer server;

        public LidgrenServer(ILogger<LidgrenServer> logger, ServerConfiguration config)
        {
            this.logger = logger;
            this.config = config;
        }

        public override void Dispose()
        {
            logger.LogInformation("Disposing...");
            base.Dispose();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting daemon: " + config.Name);
            Setup();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                MessageHandler();
                await Task.Delay(500, cancellationToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping service");
            TearDown();
            await base.StopAsync(cancellationToken);
        }

        public void Setup()
        {
            NetPeerConfiguration configuration = new NetPeerConfiguration("RKO");
            configuration.MaximumConnections = config.MaxConnections;
            configuration.Port = config.Port;
            server = new NetServer(configuration);
            server.Start();
        }

        public void TearDown()
        {
            server.Shutdown("Server shutting down...");
        }

        public static void MessageHandler()//object? peer)
        {

            NetIncomingMessage im;
            while ((im = server.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        Output(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            Output(status.ToString());
                        else
                        //EmitSignal(SignalName.Disconnected);

                        if (status == NetConnectionStatus.Disconnected)
                            Output(status.ToString());

                        string reason = im.ReadString();
                        Output(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        string chat = im.ReadString();
                        Output(chat);
                        break;
                    default:
                        Output("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
                server.Recycle(im);
            }
        }

        public static void Output(string text)
        {
            Console.WriteLine(text);
        }
    }
}