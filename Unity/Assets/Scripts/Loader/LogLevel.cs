namespace ET
{

    public enum LogLevel
    {
        /// <summary>
        ///   <para>Only unexpected errors and failures are logged.</para>
        /// </summary>
        Error,
        /// <summary>
        ///   <para>Abnormal situations that may result in problems are reported, in addition to anything from the LogLevel.Error level.</para>
        /// </summary>
        Warn,
        /// <summary>
        ///   <para>High-level informational messages are reported, in addition to anything from the LogLevel.Warn level.</para>
        /// </summary>
        Info,
        /// <summary>
        ///   <para>Detailed informational messages are reported, in addition to anything from the LogLevel.Info level.</para>
        /// </summary>
        Verbose,
        /// <summary>
        ///   <para>Debugging messages are reported, in addition to anything from the LogLevel.Verbose level.</para>
        /// </summary>
        Debug,
        /// <summary>
        ///   <para>Extremely detailed messages are reported, in addition to anything from the LogLevel.Debug level.</para>
        /// </summary>
        Silly,
    }
}
