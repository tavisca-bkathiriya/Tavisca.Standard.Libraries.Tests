using System;
using System.Threading;
using Tavisca.Libraries.Logging.Tests.Utilities;
using Tavisca.Platform.Common.Logging;
using Tavisca.Platform.Common.Plugins.Json;

namespace Tavisca.Standard.ClassLibrary.Test
{
    public class Class1
    {
        public void TestMethod()
        {
            try
            {
                throw new ArgumentNullException();
            }
            catch (Exception exception)
            {
                var id = Convert.ToString(Guid.NewGuid());
                var apiLog = Utility.GetApiLog();
                apiLog.Id = id;
                var exceptionLog = GetErrorEntry(exception, apiLog);
                ILogFormatter formatter = JsonLogFormatter.Instance;
                var redisSink = Utility.GetRedisSink();
                var logWriter = new LogWriter(formatter, redisSink);
                logWriter.WriteAsync(exceptionLog).GetAwaiter().GetResult();

                Thread.Sleep(40000);

            }
        }

        private ExceptionLog GetErrorEntry(Exception exception, ILog log)
        {
            var exceptionLog = new ExceptionLog(exception);

            var baseLog = log as LogBase;
            if (baseLog == null)
            {
                return exceptionLog;
            }

            exceptionLog.AppDomain = baseLog.AppDomain;
            exceptionLog.ApplicationName = baseLog.ApplicationName;
            exceptionLog.Id = baseLog.Id;
            return exceptionLog;
        }
    }
}
