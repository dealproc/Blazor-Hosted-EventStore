using System;
using System.Threading.Tasks;

using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

using Microsoft.AspNetCore.Components;

namespace app {
    public class IndexViewModel : ComponentBase {
        const string _subscriptionStream = "$et-announcement-message";
        const string _channel = "announcements-channel";
        const string _eventName = "announcement-message";

        [Inject]
        protected IEventStoreConnection Connection { get; set; }
        protected long EventsReceived { get; private set; }
        private EventStorePersistentSubscriptionBase _persistentSubscription;
        private UserCredentials _credentials = new UserCredentials("admin", "changeit");

        protected override async Task OnInitializedAsync() {
            var subscriptionSettings = PersistentSubscriptionSettings.Create()
                .ResolveLinkTos()
                .StartFromCurrent()
                .Build();

            await Connection.CreatePersistentSubscriptionAsync(
                _subscriptionStream,
                _channel,
                subscriptionSettings,
                _credentials);

            _persistentSubscription = await Connection.ConnectToPersistentSubscriptionAsync(
                stream: _subscriptionStream,
                groupName: _channel,
                eventAppeared: OnEventAppeared,
                userCredentials: _credentials);
        }

        private Task OnEventAppeared(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2, int? arg3) {
            EventsReceived += 1;
            return Task.CompletedTask;
        }

        protected async Task Send() {
            var msg = new AnAnnouncement {
                Title = "Test Message",
                Message = "Some message to send over to a user."
            };
            var msgbytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(msg);
            var data = new EventStore.ClientAPI.EventData(Guid.NewGuid(), _eventName, true, msgbytes, new byte[0]);

            await Connection.AppendToStreamAsync(_channel, ExpectedVersion.Any, _credentials, data);
        }
    }
}