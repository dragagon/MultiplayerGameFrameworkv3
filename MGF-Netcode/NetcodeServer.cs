using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiplayerGameFramework.Interfaces;
using NetcodeIO.NET;
using NetcodeIO.NET.Utils;

namespace MGF_Netcode
{
    public class NetcodeServer : IServerApplication, IHostedService, IDisposable
    {
        private readonly ILogger logger;
        private readonly IOptions<NetcodeConfig> config;
        private CancellationToken _token;
        private readonly byte[] privateKey = new byte[32]
        {
            0x35, 0xdd, 0xab, 0xf5, 0x6e, 0x6e, 0xa9, 0x68,
            0x54, 0xdf, 0x91, 0xd0, 0x29, 0xcd, 0x26, 0xaf,
            0x12, 0xcb, 0x93, 0x06, 0xa6, 0x4c, 0xba, 0x65,
            0xa8, 0x4f, 0x3e, 0x8a, 0xb4, 0xa7, 0x94, 0xf0
        };
        private readonly ulong protocolId = 0xdeadbeefL;
        private Server server;

        public NetcodeServer(ILogger<NetcodeServer> logger, IOptions<NetcodeConfig> config)
        {
            this.logger = logger;
            this.config = config;
            this.privateKey = new byte[32];
            KeyUtils.GenerateKey(this.privateKey);
        }

        public void Dispose()
        {
            logger.LogInformation("Disposing...");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting daemon: " + config.Value.ApplicationName);
            Setup();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping daemon.");
            TearDown();
            return Task.CompletedTask;
        }

        public void Setup()
        {
            server = new Server(100, "localhost", 8080, protocolId, privateKey);
            server.Start();
            server.OnClientMessageReceived += MessageHandler;
            server.OnLogMessage += OnLogMessage;
        }

        private void OnLogMessage(string message, NetcodeLogLevel logLevel)
        {
            Output(message);
        }

        public void TearDown()
        {
            server.Stop();
        }

        public void MessageHandler(RemoteClient client, byte[] payload, int payloadSize)
        {

        }

        public static void Output(string text)
        {
            Console.WriteLine(text);
        }

    }
}
