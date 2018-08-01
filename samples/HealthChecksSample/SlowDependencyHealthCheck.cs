﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecksSample
{
    // Simulates a health check for an  application dependency that takes a while to initialize.
    // This is part of the readiness/liveness probe sample.
    public class SlowDependencyHealthCheck : IHealthCheck
    {
        public static readonly string HealthCheckName = "slow_dependency";

        private readonly Task _task;

        public SlowDependencyHealthCheck()
        {
            _task = Task.Delay(15 * 1000);
        }

        public string Name => HealthCheckName;

        public Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_task.IsCompleted)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Dependency is ready"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("Dependency is still initializing"));
        }
    }
}
