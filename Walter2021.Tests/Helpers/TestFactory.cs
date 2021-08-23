using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using Walter2021.Common.Models;
using Walter2021.Function.Entities;

namespace Walter2021.Tests.Helpers
{
    public class TestFactory
    {
        public static WalterEntity GetWalterEntity()
        {
            return new WalterEntity
            {
                ETag = "*",
                PartitionKey = "WALTER",
                RowKey = Guid.NewGuid().ToString(),
                CreateTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = "Task: kill the humans."
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid walterId, Walter walterRequest)
        {
            string request = JsonConvert.SerializeObject(walterRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStringFromString(request),
                Path = $"/{walterId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid walterId)
        {

            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{walterId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Walter walterRequest)
        {
            string request = JsonConvert.SerializeObject(walterRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStringFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Walter GetWalterRequest()
        {
            return new Walter
            {
                CreateTime = DateTime.UtcNow,
                IsCompleted = false,
                TaskDescription = "Try to conquer the world"
            };
        }

        public static Stream GenerateStringFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;

        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null logger");
            }
            return logger;
        }
    }
}
