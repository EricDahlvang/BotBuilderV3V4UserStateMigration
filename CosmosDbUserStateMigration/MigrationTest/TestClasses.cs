using System;

namespace TestCosmosBot
{
    public class TestCosmosBotBot
    {
        public class TestDataClass
        {
            public string TestStringField { get; set; }
            public int TestIntField { get; set; }
            public Tuple<string, string> TestTuple { get; set; }
        }
    }
    
    public class GreetingState
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
}