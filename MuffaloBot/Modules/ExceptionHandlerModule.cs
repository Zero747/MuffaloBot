using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBot.Modules
{
    public class ExceptionHandlerModule : BaseExtension
    {
        public async Task HandleClientError(CommandsNextExtension c, CommandErrorEventArgs e)
        {
            if (e.Exception is CommandNotFoundException || e.Exception is UnauthorizedException || e.Exception.Message.StartsWith("Could not convert specified value to given type.")) return;

            if (e.Exception is ChecksFailedException)
            {
                await e.Context.RespondAsync("You can't do that. >:V");
                return;
            }

            await HandleClientError(e.Exception, "Command " + (e.Command?.Name ?? "unknown"));
        }

        public Task HandleClientError(DiscordClient c, ClientErrorEventArgs e)
        {
            return HandleClientError(e.Exception, "Event " + e.EventName);
        }
        public async Task HandleClientError(Exception e, string action)
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithTitle("Unhandled exception");
            builder.WithDescription($"Action: {action}\n```\n{e.ToString()}```");
            builder.WithColor(DiscordColor.Red);
            
            //DiscordChannel channel = await client.CreateDmAsync(client.CurrentApplication.Owner);
            //await client.SendMessageAsync(channel, embed: builder.Build());
            //TODO, can't do DmAsync without an associated guild
        }

        protected override void Setup(DiscordClient client)
        {
            Client = client;
            client.ClientErrored += HandleClientError;
            client.GetCommandsNext().CommandErrored += HandleClientError;
        }
    }
}
