using System;

namespace CellarIQ.CLI.Commands
{
    internal class NullCellarCommand : CellarCommand
    {
        public NullCellarCommand() : base(null)
        {
        }

        public override string Execute(string[] args)
        {
            throw new NotSupportedException("The command you requested is not implmented or supported");
        }
    }
}