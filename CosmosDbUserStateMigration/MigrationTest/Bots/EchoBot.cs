using System;
using System.Collections.Generic;
using System.Text;

namespace V4CosmosDbStateBot.Bots
{
    public class GreetingState
    {
        public string Name { get; set; }
        public string City { get; set; }
    }

    public class EchoBot
    {
        public class TestDataClass
        {
            public string TestStringField { get; set; }
            public int TestIntField { get; set; }
            public Tuple<string, string> TestTuple { get; set; }
        }
    }
}
