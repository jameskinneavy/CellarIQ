using System;
using System.Collections.Generic;
using CellarIQ.Data;

namespace CellarIQ.CLI.Commands
{
    internal class DeleteAllCellarItemsCommand : CellarCommand
    {
        private CellarManager _manager;

        public DeleteAllCellarItemsCommand(CellarManager manager) : base(manager)
        {
            
        }

        public override string Execute(string[] args)
        {
            CellarManager.DeleteAllCellarItems().Wait();

            return "All cellar items deleted";
        }
    }
}