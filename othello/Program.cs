using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using System.IO;
using System.Net.Sockets;
using ai;
using Newtonsoft.Json;

namespace othello
{
    class Program
    {
        private const int DefaultPort = 1338;
        
        private class Options
        {
            [Option('h', "host", Required = false, HelpText = "Host to connect to", Default = "localhost")]
            public string Host { get; set; }
            
            [Option('p', "port", Required = false, HelpText = "Port to connect on", Default = DefaultPort)]
            public int Port { get; set; }
            
            [Usage(ApplicationAlias = "dotnet run --")]
            public static IEnumerable<Example> Examples {
                get {
                    yield return new Example($"Connect to localhost:{DefaultPort}", new Options() );
                    yield return new Example("Connect to remote.com:12345", new Options { Host = "remote.com", Port = 54321});
                }
            }
        }
        
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(MainWithOptions)
                .WithNotParsed(HandleParseError);
        }

        private static void MainWithOptions(Options opts)
        {
            Console.WriteLine($"connecting to {opts.Host}:{opts.Port} ...");
            
            MainLoop(opts.Host, opts.Port);
            
            Console.WriteLine("");

            Console.Out.Flush();
            
        }

        private static void MainLoop(string host, int port)
        {

            TcpClient client = null;
            try
            {
                client = new TcpClient(host, port);
                
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("\n" + e.Message);
                client?.Dispose();
                return;
            }

            using (var stream = client.GetStream())
            using (var sr = new StreamReader(stream))
            using (var sw = new StreamWriter(stream))
            {
                var keepGoing = true;
                while (keepGoing)
                {
                    var line = sr.ReadLine();

                    if (line == null)
                    {
                        Console.WriteLine("Game over.");
                        keepGoing = false;
                    }
                    else
                    {
                        var gameMessage = JsonConvert.DeserializeObject<GameMessage>(line);
                        
                        var nextMove = AI.NextMove(gameMessage);
                        
                        var serialized = JsonConvert.SerializeObject(nextMove);
                        
                        sw.WriteLine(serialized);
                        sw.Flush();
                    }

                } 
            }
            
            client.Dispose();
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            foreach (var err in errs)
            {
                if (err is HelpRequestedError)
                {
                    continue;
                }

                Console.WriteLine("Parse error: " + err);
            }
        }
    }

}
