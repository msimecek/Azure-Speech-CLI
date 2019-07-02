using Moq;
using CRIS;
using SpeechCLI.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CRIS.Models;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using System.Net.Http;
using Moq.Protected;
using System.Threading;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using SpeechCLI.Tests.Utils;
using System.Diagnostics;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using SpeechCLI.Interfaces;
using Microsoft.Rest;

namespace SpeechCLI.Tests.CommandTests
{
    public class TranscriptCommandTests
    {
        public CommandLineApplication<MainApp> InitApp(ISpeechServicesAPIv20 apiObject)
        {
            var services = new ServiceCollection()
                .AddSingleton<IConfig, Config>()
                .AddSingleton<ISpeechServicesAPIv20>(apiObject)
                .BuildServiceProvider();

            var writer = new MockTestWriter();
            var app = new CommandLineApplication<MainApp>(new MockConsole(writer));
            app.Conventions.UseDefaultConventions().UseConstructorInjection(services);

            return app;
        }

        static string[] CommandIntoArgs(string command)
        {
            return command.Split(' ');
        }

        [Fact]
        public void List_Success()
        {
            // ARRANGE
            var response = new List<Transcription>()
            {
                new Transcription()
                {
                    Id = Guid.Empty,
                    Name = "Moq",
                    Status = SpeechCLI.Utils.Constants.SUCCEEDED_STATUS
                }
            };

            var mock = new Mock<ISpeechServicesAPIv20>();
            mock
                .Setup(
                    m => m.GetTranscriptionsWithHttpMessagesAsync(null, CancellationToken.None)
                )
                .ReturnsAsync(
                    new HttpOperationResponse<object, GetTranscriptionsHeaders>() { Body = response }
                );

            //var mockServer = FluentMockServer.Start();
            //mockServer
            //    .Given(Request.Create().WithPath("/api/speechtotext/v2.0/transcriptions").UsingGet())
            //    .RespondWith(Response.Create().WithStatusCode(200).WithBody("OK"));

            //var hc = new HttpClient();
            //hc.BaseAddress = new Uri($"http://localhost:{mockServer.Ports[0]}");
            //var sdk = new SpeechServicesAPIv20(hc, true);

            var app = InitApp(mock.Object);

            // ACT
            var args = CommandIntoArgs("transcript list");
            app.Execute(args);

            // ASSES
            Assert.Equal("Getting transcriptions...\r\n00000000-0000-0000-0000-000000000000 Moq Succeeded\r\n", ((MockTestWriter)app.Out).ReadAsString());
        }

    }
}
