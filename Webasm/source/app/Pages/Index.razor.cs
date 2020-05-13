using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.AspNetCore.Components;

namespace app {
    public class IndexViewModel : ComponentBase {
        const string _subscriptionStream = "$et-announcement-message";
        const string _channel = "announcements-channel";
        const string _eventName = "announcement-message";

        [Inject]
        protected EventStoreClient Client { get; set; }
        [Inject]
        protected EventStorePersistentSubscriptionsClient SubscriptionsClient { get; set; }
        protected UserCredentials Credentials => new UserCredentials("admin", "changeit");
        protected long EventsReceived { get; private set; }

        protected override async Task OnInitializedAsync()
        {
            await SubscriptionsClient.CreateAsync(_subscriptionStream, _channel,
                new PersistentSubscriptionSettings(resolveLinkTos: true), Credentials);

            await SubscriptionsClient.SubscribeAsync(
                _subscriptionStream,
                _channel, eventAppeared:
                OnEventAppeared, OnSubscriptionDropped);
        }

        private Task OnEventAppeared(PersistentSubscription arg1, ResolvedEvent arg2, int? arg3, CancellationToken arg4)
        {
            EventsReceived += 1;
            return Task.CompletedTask;
        }

        private void OnSubscriptionDropped(PersistentSubscription sub, SubscriptionDroppedReason reason, Exception? exc)
        {
            
        }

        protected async Task Send() {
            var msg = new AnAnnouncement {
                Title = "Test Message",
                Message = "Some message to send over to a user."
            };
            var data = new ReadOnlyMemory<byte>(JsonSerializer.SerializeToUtf8Bytes(msg));
            var eventData = new EventData(new Uuid(), _eventName, data, new byte[0]);

            await Client.AppendToStreamAsync(_subscriptionStream, StreamState.Any, new[]{eventData}, userCredentials: Credentials);
        }
    }
}