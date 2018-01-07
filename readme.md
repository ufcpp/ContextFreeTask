## ContextFreeTask

### Usage

NuGet package: https://www.nuget.org/packages/ContextFreeTasks/

Use `ContextFreeTask` struct instead of `Task` class (`System.Threading.Tasks` namespace) for return types of async methods.

```csharp
private async ContextFreeTask FAsync() { ... }
private async ContextFreeTask<T> FAsync<T>() { ... }
```

These ignore the current synchronization context in the methods.
You do not have to write `ConfigureAwait(false)` anymore.

### Task-like

In [C# 7](https://docs.microsoft.com/ja-jp/dotnet/articles/csharp/csharp-7), async methods may return other types in addition to `Task`, `Task<T>` and `void`.
The returned type must satisfy a certain pattern. These types are called "Task-like".

The `ContextFreeTask` struct in this library satisfies the "Task-like" pattern and the "awaitable" pattern.

### What ContextFreeTask does

The methods which return `ContextFreeTask` do not capture the synchronization context.

For example, you can use this as following:

```csharp
// All awaits in this method don't capture SynchronizationContext
private async ContextFreeTask FAsync()
{
    // Whatever current context is
    await Task.Delay(100);
    // no context here
    await Task.Delay(100);
    // no context here
}
```

This code behaves almost the same as the following:

```csharp
private async Task FAsync()
{
    await Task.Delay(100).ConfigureAwait(false);
    await Task.Delay(100).ConfigureAwait(false);
}
```

### Implementation

The `ContextFreeTask` struct is a thin wrapper of `Task` class.

```cs
public struct ContextFreeTask
{
    public Task Task { get; }
}
```

And the `ContextFreeTask<T>` is that of `Task<T>`.

```cs
public struct ContextFreeTask<T>
{
    public Task<T> Task { get; }
}
```

Those AsyncMethodBuilders always clear current synchronization context.

```cs
public struct AsyncContextFreeTaskMethodBuilder
{
    private AsyncTaskMethodBuilder _methodBuilder;

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        var prevContext = SynchronizationContext.Current;
        try
        {
            SynchronizationContext.SetSynchronizationContext(null);
            _methodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(prevContext);
        }
    }
}
```
