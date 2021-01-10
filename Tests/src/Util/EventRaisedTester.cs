using System;

namespace Tests.Util
{
    class EventRaisedTester
    {
        public int TimesRaised { get; private set; }

        public EventRaisedTester(Action<EventHandler> subscribeToEvent)
        {
            subscribeToEvent(TestHandler);
        }

        private void TestHandler(object sender, EventArgs e)
        {
            TimesRaised++;
        }
    }
}
