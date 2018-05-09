using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;

namespace WebJobApp
{
    public class NameResolver : INameResolver
    {   
     
        public string Resolve(string name)
        {
            string value = null;

            value = Environment.GetEnvironmentVariable(name);

            return value;
        }
    }
}
