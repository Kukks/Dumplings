﻿using Dumplings.Checking;
using Dumplings.Helpers;
using Dumplings.Scanning;
using Dumplings.Stats;
using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dumplings.Cli
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Logger.InitializeDefaults();

            Logger.LogInfo("Parsing arguments...");
            ParseArgs(args, out Command command, out NetworkCredential rpcCred, out var server);

            var rpcConf = new RPCCredentialString
            {
                Server = server,
                UserPassword = rpcCred
            };
            var client = new RPCClient(rpcConf, Network.Main);

            using (BenchmarkLogger.Measure(operationName: $"{command} Command"))
            {
                if (command == Command.Resync)
                {
                    var scanner = new Scanner(client);
                    await scanner.ScanAsync(rescan: true);
                }
                else if (command == Command.Sync)
                {
                    var scanner = new Scanner(client);
                    await scanner.ScanAsync(rescan: false);
                }
                else if (command == Command.Check)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var checker = new Checker(loadedScannerFiles);
                    checker.Check();
                }
                else if (command == Command.MonthlyVolumes)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateMonthlyVolumes();
                }
                else if (command == Command.FreshBitcoins)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateFreshBitcoins();
                }
                else if (command == Command.FreshBitcoinAmounts)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateFreshBitcoinAmounts();
                }
                else if (command == Command.FreshBitcoinsDaily)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateFreshBitcoinsDaily();
                }
                else if (command == Command.NeverMixed)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateNeverMixed(client);
                }
                else if (command == Command.CoinJoinEquality)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateEquality();
                }
                else if (command == Command.CoinJoinIncome)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateIncome();
                }
                else if (command == Command.PostMixConsolidation)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculatePostMixConsolidation();
                }
                else if (command == Command.SmallerThanMinimum)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateSmallerThanMinimumWasabiInputs();
                }
                else if (command == Command.MonthlyEqualVolumes)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateMonthlyEqualVolumes();
                }
                else if (command == Command.AverageUserCount)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateMonthlyAverageMonthlyUserCounts();
                }
                else if (command == Command.AverageNetworkFeePaidByUserPerCoinjoin)
                {
                    var loadedScannerFiles = Scanner.Load();
                    var stat = new Statista(loadedScannerFiles);
                    stat.CalculateMonthlyNetworkFeePaidByUserPerCoinjoin();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press a button to exit...");
            Console.ReadKey();
        }


        private static void ParseArgs(string[] args, out Command command, out NetworkCredential cred,
            out string server)
        {
            string rpcUser = Environment.GetEnvironmentVariable("RPCUSER");
            string rpcPassword = Environment.GetEnvironmentVariable("RPCPASSWORD");
            server = Environment.GetEnvironmentVariable("RPCSERVER");
            command = (Command)Enum.Parse(typeof(Command), Environment.GetEnvironmentVariable("COMMAND")??args[0], ignoreCase: true);

            var rpcUserArg = "--rpcuser=";
            var rpcPasswordArg = "--rpcpassword=";
            var rpcServerArg = "--rpcserver=";
            foreach (var arg in args)
            {
                var idx = arg.IndexOf(rpcUserArg, StringComparison.Ordinal);
                if (idx == 0)
                {
                    rpcUser = arg.Substring(idx + rpcUserArg.Length);
                }

                idx = arg.IndexOf(rpcPasswordArg, StringComparison.Ordinal);
                if (idx == 0)
                {
                    rpcPassword = arg.Substring(idx + rpcPasswordArg.Length);
                }

                idx = arg.IndexOf(rpcServerArg, StringComparison.Ordinal);
                if (idx == 0)
                {
                    server = arg.Substring(idx + rpcServerArg.Length);
                }
            }

            cred = new NetworkCredential(rpcUser, rpcPassword);
        }
    }
}
