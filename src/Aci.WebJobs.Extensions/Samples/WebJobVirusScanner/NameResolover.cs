using Microsoft.Azure.WebJobs;
using System;

namespace WebJobVirusScanner
{
    public class NameResolver : INameResolver
    {

        public string Resolve(string name)
        {
            string value = null;

            if (!string.IsNullOrEmpty(name))
            {
                value = Environment.GetEnvironmentVariable(name);
            }
            return value;
        }
    }
}
