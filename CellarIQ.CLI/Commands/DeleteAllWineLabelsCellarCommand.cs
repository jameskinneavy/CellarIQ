using CellarIQ.Data;

namespace CellarIQ.CLI.Commands
{
    internal class DeleteAllWineLabelsCellarCommand : CellarCommand
    {
        public DeleteAllWineLabelsCellarCommand(CellarManager manager) : base(manager)
        {
            
        }

        public override string Execute(string[] args)
        {
            CellarManager.DeleteAllWines().Wait();

            return "All wines deleted";
        }

        
    }
}