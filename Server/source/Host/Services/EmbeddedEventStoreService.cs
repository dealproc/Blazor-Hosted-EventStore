using System;
using System.Threading;
using System.Threading.Tasks;

using EventStore.ClientAPI.Embedded;
using EventStore.Core;

using Microsoft.Extensions.Hosting;

namespace Host.Services {
    public class EmbeddedEventStoreService : IHostedService {
        ClusterVNode _node;
        public async Task StartAsync(CancellationToken cancellationToken) {
            //RESEARCH: Should this be in a hosted service instead?
            var nodeBuilder = EmbeddedVNodeBuilder.AsSingleNode()
                .OnDefaultEndpoints()
                .DisableExternalTls()
                .DisableInternalTls()
                .RunInMemory();
            _node = nodeBuilder.Build();
            await _node.StartAsync(true);
        }

        public async Task StopAsync(CancellationToken cancellationToken) {
            await _node.StopAsync(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }
}