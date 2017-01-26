namespace CellarIQ.CLI.Commands
{
    internal class QuitCommand : CellarCommand
    {
        public QuitCommand() : base(null)
        {
        }

        public override string Execute(string[] args)
        {
            return "quit";
        }
    }
}