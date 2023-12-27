﻿namespace GFXSD.Extensions
{
    public static class ExceptionUtils
    {
        private const string Log4j2ErrorMessage = "ERROR StatusLogger Log4j2 could not find a logging implementation. Please add log4j-core to the classpath. Using SimpleLogger to log to the console...";

        public static bool IsLog4j2Error(this string errorMessage)
            => errorMessage.ToLower().Contains(Log4j2ErrorMessage.ToLower());
    }
}
