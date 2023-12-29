using Newtonsoft.Json;
using System;

namespace GFXSD.Commands
{
    public static class ExceptionHandler
    {
        public static CommandResult Handle(Exception exception)
            => new CommandResult { Error = JsonConvert.SerializeObject(exception) };
    }
}
