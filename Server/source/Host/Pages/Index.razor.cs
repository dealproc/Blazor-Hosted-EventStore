using System;
using System.Text.Json;
using System.Threading.Tasks;

using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

using Microsoft.AspNetCore.Components;

namespace Host.Pages {
    public class IndexViewModel : ComponentBase {
        const string _subscriptionStream = "$et-announcement-message";
        const string _channel = "announcements-channel";
        const string _eventName = "announcement-message";

        [Inject]
        protected IEventStoreConnection Connection { get; set; }
        protected long EventsReceived { get; private set; }
        protected EventStorePersistentSubscriptionBase ClickSubscription { get; private set; }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            
            var subscriptionSettings = PersistentSubscriptionSettings.Create()
                .ResolveLinkTos()
                .StartFromBeginning()
                .Build();

            try {
                await Connection.CreatePersistentSubscriptionAsync(_subscriptionStream, _channel,
                    subscriptionSettings, new UserCredentials("admin", "changeit"));
            } catch (Exception) {
                await Connection.UpdatePersistentSubscriptionAsync(_subscriptionStream, _channel,
                    subscriptionSettings, new UserCredentials("admin", "changeit"));
            }

            ClickSubscription = await Connection.ConnectToPersistentSubscriptionAsync(
                stream: _subscriptionStream,
                groupName: _channel,
                eventAppeared: async(sub, e, position) => {
                    await InvokeAsync(() => {
                        EventsReceived += 1;
                        StateHasChanged();
                    });
                },
                subscriptionDropped: (sub, reason, exc) => { },
                userCredentials : new UserCredentials("admin", "changeit"));
        }

        protected async Task Send() {
            var msg = new AnAnnouncement {
                Title = "Test Message",
                Message = "Some message to send over to a user."
            };
            var data = new EventData(
                Guid.NewGuid(),
                _eventName,
                true,
                JsonSerializer.SerializeToUtf8Bytes(msg),
                new byte[0]);

            await Connection.AppendToStreamAsync(_subscriptionStream, ExpectedVersion.Any, new [] { data }, new UserCredentials("admin", "changeit"));
        }
    }
}