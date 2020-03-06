using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Tavisca.Common.Plugins.Configuration;
using Tavisca.Common.Plugins.Redis;
using Tavisca.Libraries.Configuration;
using Tavisca.Libraries.Logging.Sink.Redis;
using Tavisca.Platform.Common;
using Tavisca.Platform.Common.ExceptionManagement;
using Tavisca.Platform.Common.Logging;

namespace Tavisca.Libraries.Logging.Tests.Utilities
{
    public static class Utility
    {

        public static ApiLog GetApiLog()
        {
            var log = new ApiLog
            {
                Api = "api",
                Verb = "verb",
                Url = "https://www.google.com?param1=val1&param2=val2",
                IsSuccessful = true,
                TimeTakenInMs = 23324.4556,
                TransactionId = "13124423523",
            };
            return log;
        }

        public static RedisSink GetRedisSink()
        {
            //var redisLogSettings = new RedisLogSettings
            //{
            //    ApiSetting = new RedisSetting
            //    {
            //        Hosts = new List<RedisHost> {
            //            new RedisHost
            //            {
            //                Url = "master.travel-qa-logging.l86run.use1.cache.amazonaws.com",
            //                Port = "6379",
            //                IsSslEnabled = true
            //            }
            //        },
            //        QueueName = "travel-qa-logging-api"
            //    },
            //    ExceptionSetting = new RedisSetting
            //    {
            //        Hosts = new List<RedisHost> {
            //            new RedisHost
            //            {
            //                Url = "master.travel-qa-logging.l86run.use1.cache.amazonaws.com",
            //                Port = "6379",
            //                IsSslEnabled = true
            //            }
            //        },
            //        QueueName = "travel-qa-logging-exception"
            //    },
            //    TraceSetting = new RedisSetting
            //    {
            //        Hosts = new List<RedisHost> {
            //            new RedisHost
            //            {
            //                Url = "master.travel-qa-logging.l86run.use1.cache.amazonaws.com",
            //                Port = "6379",
            //                IsSslEnabled = true
            //            }
            //        },
            //        QueueName = "travel-qa-logging-trace"
            //    }
            //};

            var configProvider = new Tavisca.Common.Plugins.Configuration.ConfigurationProvider("hotel_content_service");
            var redisLogSettings = new RedisLogSettingsProvider(configProvider);

            return new RedisSink(redisLogSettings);
        }
    }
}
