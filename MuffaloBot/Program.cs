using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using MuffaloBot.Converters;

namespace MuffaloBot
{
    class Program
    {
        public DiscordClient client;
        public CommandsNextExtension commandsNext;
        static Program _i;
        public static Program instance
        {
            get
            {
                if (_i == null) _i = new Program();
                return _i;
            }
        }
        public Program()
        {
            client = new DiscordClient(new DiscordConfiguration()
            {
                //UseInternalLogHandler = true,
#if DEBUG
                Minimum​Log​Level = LogLevel.Debug,
#else
                Minimum​Log​Level = LogLevel.Information,
#endif
                TokenType = TokenType.Bot,
                Token = AuthResources.BotToken, // Create a new AuthResources resource file
                Intents = DiscordIntents.All
            });
            commandsNext = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" },
                EnableMentionPrefix = true,
                EnableDefaultHelp = true
            });
            commandsNext.RegisterCommands(Assembly.GetExecutingAssembly());
            client.Logger.Log(LogLevel.Information, $"MuffaloBot Registered {commandsNext.RegisteredCommands.Count} commands", DateTime.Now);
            commandsNext.SetHelpFormatter<MuffaloBotHelpFormatter>();
            LoadModules();
        }
        public void LoadModules()
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(BaseExtension).IsAssignableFrom(t))
                {
                    try
                    {
                        client.AddExtension((BaseExtension)Activator.CreateInstance(t));
                        client.Logger.Log(LogLevel.Information, $"MuffaloBot Loaded module {t.FullName}", DateTime.Now);
                    }
                    catch (Exception e)
                    {
                        client.Logger.Log(LogLevel.Error, $"MuffaloBot Could not load module {t.FullName}: {e}", DateTime.Now);
                    }
                }
            }
        }
        public async Task StartAsync()
        {
            await client.ConnectAsync();
            await Task.Delay(-1);
        }
        static void Main(string[] args)
        {
            Program.instance.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
