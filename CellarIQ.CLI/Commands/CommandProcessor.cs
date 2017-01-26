using System;
using System.Collections.Generic;
using System.Linq;
using CellarIQ.Data;

namespace CellarIQ.CLI.Commands
{
    class CommandProcessor
    {
        private readonly CellarManager _manager;
        private readonly Dictionary<string, CellarCommand> _commands;

        public CommandProcessor(CellarManager manager)
        {
            _manager = manager;
            _commands = new Dictionary<string, CellarCommand>
            {
                { "delete-all-winelabels",new DeleteAllWineLabelsCellarCommand(manager)  },
                { "delete-all-cellaritems", new DeleteAllCellarItemsCommand(manager) },
                { "null-command", new NullCellarCommand() },
                { "quit", new QuitCommand() },
                { "load-wines-from-csv", new LoadWinesFromCsv(manager) },
                { "load-cellaritems-from-csv", new LoadCellarItemsFromCsv(manager) },
                { "list-all-wines", new ListAllWineLabels(manager) },
                { "list-all-cellaritems", new ListAllCellarItems(manager) },


            };

            _commands.Add("daw", _commands["delete-all-winelabels"]);
            _commands.Add("daci", _commands["delete-all-cellaritems"]);
            _commands.Add("lwcsv", _commands["load-wines-from-csv"]);
            _commands.Add("lcicsv", _commands["load-cellaritems-from-csv"]);
            _commands.Add("law", _commands["list-all-wines"]);
            _commands.Add("laci", _commands["list-all-cellaritems"]);
        }

        public string Execute(string commandText)
        {
            CellarCommand commandToExecute = null;
            string result = "unknown";
            string[] tokens = commandText.Split(' ');
            if (tokens.Length > 0)
            {
                string command = tokens[0].ToLower();
                commandToExecute = _commands.ContainsKey(command) ? _commands[command] : _commands["null-command"];
                result = commandToExecute.Execute(tokens.Length > 1 ? tokens.Skip(1).ToArray() : null);
            }
           

            return result;
        }
    }
}
