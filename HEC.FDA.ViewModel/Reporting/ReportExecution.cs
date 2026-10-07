using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Reporting;

public static class ReportExecution
{
    /// <summary>Commits a completed result before subsequent work. Cancellation discards the active result.</summary>
    public static async Task<T> ComputeAndSaveAsync<T>(Func<Task<T>> compute, Action<T> save,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        T result = await compute();
        cancellationToken.ThrowIfCancellationRequested();
        save(result);
        return result;
    }
}

/// <summary>Run-local cache for inexpensive, transient results shared by report sections.</summary>
public class ReportResultCache<TKey, TResult>
{
    private readonly Dictionary<TKey, TResult> _results = new();

    public async Task<TResult> GetOrComputeAsync(TKey key, Func<Task<TResult>> compute)
    {
        if (_results.TryGetValue(key, out TResult result)) return result;
        result = await compute();
        _results.Add(key, result);
        return result;
    }
}
