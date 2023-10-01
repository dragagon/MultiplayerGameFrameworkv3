using Lidgren.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiplayerGameFramework.Interfaces;

namespace MGF_Lidgren
{
    public class LidgrenServer : IServerApplication, IHostedService, IDisposable
    {
        private readonly ILogger logger;
        private readonly IOptions<LidgrenConfig> config;
        private static NetServer server;
        private CancellationToken _token;

        public LidgrenServer(ILogger<LidgrenServer> logger, IOptions<LidgrenConfig> config)
        {
            this.logger = logger;
            this.config = config;
        }

        public void Dispose()
        {
            logger.LogInformation("Disposing...");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting daemon: " + config.Value.ApplicationName);
            Setup();
            _token = cancellationToken;
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Starting Service");
                while (true)
                {
                    Thread.Sleep(500);
                    MessageHandler();
                    if (_token.IsCancellationRequested)
                    {
                        Console.WriteLine("Stopping service");
                        break;
                    }
                }
            }, _token);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping daemon.");
            TearDown();
            return Task.CompletedTask;
        }

        public void Setup()
        {
            NetPeerConfiguration configuration = new NetPeerConfiguration("RKO");
            configuration.MaximumConnections = 100;
            configuration.Port = 8080;
            server = new NetServer(configuration);
            server.Start();
        }

        public void TearDown()
        {
            server.Shutdown("Server shutting down...");
        }

        public static void MessageHandler()
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