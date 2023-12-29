using GFXSD.Extensions;
using System;

namespace GFXSD.Commands
{
    public class RemoveNodesCommand : ICommand
    {
        public string Xml { get; set; }

        public string NodeName { get; set; }

        public CommandResult Execute()
        {
            try
            {
                return Xml.RemoveNodes(NodeName);
            }
            catch (Exception exception)
            {
                return ExceptionUtils.HandleExecuteException(exception);
            }
        }
    }
}
