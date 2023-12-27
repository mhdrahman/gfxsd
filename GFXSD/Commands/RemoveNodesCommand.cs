using GFXSD.Extensions;

namespace GFXSD.Commands
{
    public class RemoveNodesCommand : ICommand
    {
        public string Xml { get; set; }

        public string NodeName { get; set; }

        public CommandResult Execute()
            => Xml.RemoveNodes(NodeName);
    }
}
