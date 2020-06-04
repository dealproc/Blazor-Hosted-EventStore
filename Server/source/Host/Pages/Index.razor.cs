using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using EventStore.ClientAPI;

using Microsoft.AspNetCore.Components;

using Serilog;
using ILogger = Serilog.ILogger;

namespace Host.Pages {
    public class IndexViewModel : ComponentBase, IDisposable {
        static ILogger s_log = Log.Logger.ForContext<IndexViewModel>();
        const string _subscriptionStream = "$et-announcement-message";
        const string _channel = "announcements-channel";
        const string _eventName = "announcement-message";

        private int _nextCounterNumber = 0;
        private EventStorePersistentSubscriptionBase _subscription;

        [Inject]
        protected IEventStoreConnection Connection { get; set; }
        protected List<Counter> Counters { get; private set; } = new List<Counter>();

        protected async Task AddCounter() {
            _nextCounterNumber++;
            s_log.Information("Adding a counter.");

            await InvokeAsync(() => {
                Counters.Add(new Counter {
                    CounterId = _nextCounterNumber,
                        Label = $"Counter {_nextCounterNumber}",
                        NumberOfClicks = 0
                });
                StateHasChanged();
                s_log.Information("Counter added.");
            });
        }

        protected async Task ClickCounter(Counter counter) {
            s_log.Information("Counter clicked.");
            var msg = new CounterClicked {
                CounterId = counter.CounterId
            };
            var data = new EventData(
                Guid.NewGuid(),
                "counter-clicked",
                true,
                JsonSerializer.SerializeToUtf8Bytes(msg),
                new byte[0]
            );
            await Connection.AppendToStreamAsync(_subscriptionStream, ExpectedVersion.Any, new [] { data }, default);
        }

        protected override async Task OnInitializedAsync() {
            await base.OnInitializedAsync();
            var subSettings = PersistentSubscriptionSettings.Create()
                .ResolveLinkTos()
                .StartFromBeginning()
                .Build();

            try {
                await Connection.CreatePersistentSubscriptionAsync(_subscriptionStream,
                    _channel,
                    subSettings,
                    default);
            } catch (Exception) {
                await Connection.UpdatePersistentSubscriptionAsync(_subscriptionStream,
                    _channel,
                    subSettings,
                    default);
            }

            _subscription = await Connection.ConnectToPersistentSubscriptionAsync(
                stream: _subscriptionStream,
                groupName: _channel,
                eventAppeared: async(sub, e, position) => {
                    s_log.Information("Received counter-click.");
                    await InvokeAsync(() => {
                        s_log.Information("Received counter-click.");
                        var clickedOn = JsonSerializer.Deserialize<CounterClicked>(new ReadOnlySpan<byte>(e.Event.Data));
                        var counter = Counters.SingleOrDefault(c => c.CounterId == clickedOn.CounterId);
                        if (counter != null) {
                            s_log.Information("Counter entry found.  Adding +1 to value.");
                            counter.NumberOfClicks += 1;
                        }

                        StateHasChanged();
                    });
                },
                subscriptionDropped: (sub, reason, exc) => { },
                userCredentials : default
            );
        }

        public void Dispose() {
            _subscription?.Stop(TimeSpan.FromSeconds(30));
            _subscription = null;
        }
    }

    public class Counter {
        public int CounterId { get; set; }
        public string Label { get; set; }
        public int NumberOfClicks { get; set; }
    }

    public struct CounterClicked {
        public int CounterId { get; set; }
    }
}