using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tavisca.Common.Plugins.Redis;
using Tavisca.Libraries.Logging.Sink.Redis;

namespace Tavisca.Standard.Libraries.Logging.Tests.Web.Utilities
{
    public static class Utility
    {
        public static RedisSink GetRedisSink()
        {
            var configProvider = new Tavisca.Common.Plugins.Configuration.ConfigurationProvider("hotel_content_service");
            var redisLogSettings = new RedisLogSettingsProvider(configProvider);

            return new RedisSink(redisLogSettings);
        }
    }
}
