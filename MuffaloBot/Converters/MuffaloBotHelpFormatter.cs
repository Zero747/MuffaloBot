using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBot.Converters
{
    /// <summary>
    /// Help formatter based off of the default help formatter
    /// </summary>
    public class MuffaloBotHelpFormatter : BaseHelpFormatter
    {
        /// <summary>
        /// Creates a new default help formatter.
        /// </summary>
        public MuffaloBotHelpFormatter(CommandContext ctx) : base(ctx)
        {
            _embed = new DiscordEmbedBuilder();
            _name = null;
            _desc = null;
            _gexec = false;
        }

        /// <summary>
        /// Sets the name of the current command.
        /// </summary>
        /// <param name="name">Name of the command for which the help is displayed.</param>
        /// <returns>Current formatter.</returns>
        public BaseHelpFormatter WithCommandName(string name)
        {
            _name = name;
            return this;
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            // _embed.AddField(command.Name, command.Description);            
            // _strBuilder.AppendLine($"{command.Name} - {command.Description}");
            _name = command.Name;
            return this;
        }

        /// <summary>
        /// Sets the description of the current command.
        /// </summary>
        /// <param name="description">Description of the command for which help is displayed.</param>
        /// <returns>Current formatter.</returns>
        public BaseHelpFormatter WithDescription(string description)
        {
            _desc = description;
            return this;
        }

        /// <summary>
        /// Sets aliases for the current command.
        /// </summary>
        /// <param name="aliases">Aliases of the command for which help is displayed.</param>
        /// <returns>Current formatter.</returns>
        public BaseHelpFormatter WithAliases(IEnumerable<string> aliases)
        {
            if (aliases.Any())
            {
                _embed.AddField("Aliases", string.Join(", ", aliases.Select(new Func<string, string>(Formatter.InlineCode))), false);
            }
            return this;
        }

        /// <summary>
        /// Sets the arguments the current command takes.
        /// </summary>
        /// <param name="arguments">Arguments that the command for which help is displayed takes.</param>
        /// <returns>Current formatter.</returns>
        public BaseHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments)
        {
            if (arguments.Any<CommandArgument>())
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (CommandArgument commandArgument in arguments)
                {
                    if (commandArgument.IsOptional || commandArgument.IsCatchAll)
                    {
                        stringBuilder.Append("`[");
                    }
                    else
                    {
                        stringBuilder.Append("`<");
                    }
                    stringBuilder.Append(commandArgument.Name);
                    if (commandArgument.IsCatchAll)
                    {
                        stringBuilder.Append("...");
                    }
                    if (commandArgument.IsOptional || commandArgument.IsCatchAll)
                    {
                        stringBuilder.Append("]: ");
                    }
                    else
                    {
                        stringBuilder.Append(">: ");
                    }
                    stringBuilder.Append(commandArgument.Type.Name).Append("`: ");
                    stringBuilder.Append(string.IsNullOrWhiteSpace(commandArgument.Description) ? "No description provided." : commandArgument.Description);
                    if (commandArgument.IsOptional)
                    {
                        stringBuilder.Append(" Default value: ").Append(commandArgument.DefaultValue);
                    }
                    stringBuilder.AppendLine();
                }
                _embed.AddField("Arguments", stringBuilder.ToString(), false);
            }
            return this;
        }

        /// <summary>
        /// When the current command is a group, this sets it as executable.
        /// </summary>
        /// <returns>Current formatter.</returns>
        public BaseHelpFormatter WithGroupExecutable()
        {
            _gexec = true;
            return this;
        }

        /// <summary>
        /// Sets subcommands of the current command. This is also invoked for top-level command listing.
        /// </summary>
        /// <param name="subcommands">Subcommands of the command for which help is displayed.</param>
        /// <returns>Current formatter.</returns>
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            if (subcommands.Any())
            {
                _embed.AddField((_name != null) ? "Subcommands" : "Commands", string.Join(", ", from xc in subcommands
                                                                                                          select Formatter.InlineCode(xc.QualifiedName)), false);
            }
            return this;
        }

        /// <summary>
        /// Construct the help message.
        /// </summary>
        /// <returns>Data for the help message.</returns>
        public override CommandHelpMessage Build()
        {
            _embed.Title = "MuffaloBot Help";
            _embed.Color = DiscordColor.Green;
            string description = "Listing all public commands. Type `!help <command>` to learn more about a command. Type `!quotes` for all quote commands.";
            if (_name != null)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(Formatter.InlineCode(_name)).Append(": ").Append(string.IsNullOrWhiteSpace(_desc) ? "No description provided." : _desc);
                if (_gexec)
                {
                    stringBuilder.AppendLine().AppendLine().Append("This can be executed as a standalone command.");
                }
                description = stringBuilder.ToString();
            }
            _embed.Description = description;
            return new CommandHelpMessage(null, _embed);
        }

        private DiscordEmbedBuilder _embed;

        private string _name;

        private string _desc;

        private bool _gexec;
    }
}
