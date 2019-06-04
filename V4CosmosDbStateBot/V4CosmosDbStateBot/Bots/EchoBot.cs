// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace V4CosmosDbStateBot.Bots
{
    public class GreetingState
    {
        public string Name { get; set; }
        public string City { get; set; }
    }

    public class EchoBot : ActivityHandler
    {
        public class TestDataClass
        {
            public string TestStringField { get; set; }
            public int TestIntField { get; set; }
            public Tuple<string, string> TestTuple { get; set; }
        }

        private readonly IStatePropertyAccessor<GreetingState> _greetingStateAccessor;
        private readonly IStatePropertyAccessor<TestDataClass> _testDataAccessor;
        private readonly IStatePropertyAccessor<bool> _askedNameAccessor;
        private readonly IStatePropertyAccessor<string> _testAccessor;

        private readonly UserState _userState;

        public EchoBot(UserState userState)
        {
            _greetingStateAccessor = userState.CreateProperty<GreetingState>("V4TestGreeting");
            _testDataAccessor = userState.CreateProperty<TestDataClass>("V4TestDataClass");
            _askedNameAccessor = userState.CreateProperty<bool>("AskedName");
            _testAccessor = userState.CreateProperty<string>("test");
            _userState = userState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var greetingState = await _greetingStateAccessor.GetAsync(turnContext, () => new GreetingState());
            if (greetingState != null && !string.IsNullOrEmpty(greetingState.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hi {greetingState.Name} from {greetingState.City}. You said: {turnContext.Activity.Text}"), cancellationToken);

                var testDataClass = await _testDataAccessor.GetAsync(turnContext,()=>new TestDataClass());
                var askedName = await _askedNameAccessor.GetAsync(turnContext, () => false);
                var test = await _testAccessor.GetAsync(turnContext, () => string.Empty);

                await turnContext.SendActivityAsync($"askedName: {askedName} test: {test} testDataClass.TestIntField: {testDataClass.TestIntField} testDataClass.TestStringField: {testDataClass.TestStringField} testDataClass.TestTuple.Item1: {testDataClass.TestTuple.Item1} testDataClass.TestTuple.Item2: {testDataClass.TestTuple.Item2}");
            }
            else
            {
                await _greetingStateAccessor.SetAsync(turnContext, new GreetingState() { Name = "Eric auto", City = "Seattle (auto)" });
                await _testDataAccessor.SetAsync(turnContext, new TestDataClass()
                            {
                                TestIntField = 11,
                                TestStringField = "auto string field",
                                TestTuple = new Tuple<string, string>("item 1 auto", "item 2 auto")
                            });
                await _askedNameAccessor.SetAsync(turnContext, true);
                await _testAccessor.SetAsync(turnContext, "test");


                await turnContext.SendActivityAsync(MessageFactory.Text($"(greetingState was present...auto saved) Echo: {turnContext.Activity.Text}"), cancellationToken);
            }

            await _userState.SaveChangesAsync(turnContext);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and Welcome!"), cancellationToken);
                }
            }
        }
    }
}
