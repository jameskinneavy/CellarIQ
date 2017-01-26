using CellarIQ.Data;

namespace CellarIQ.CLI.Commands
{
    internal abstract class CellarCommand
    {
        
        public CellarCommand(CellarManager manager)
        {
            CellarManager = manager;
        }

        protected CellarManager CellarManager { get; set; }
        public abstract string Execute(string[] args);
    }
}