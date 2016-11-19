## ContextFreeTask

### Usage

Use `ContextFreeTask` struct instead of `Task` class (`System.Threading.Tasks` namespace) for return types of async methods.

```csharp
private async ContextFreeTask FAsync()
{
    ...
}
```

This ignores the current synchronization context.
You do not have to write `ConfigureAwait(false)` anymore.

### Task-like

In [C# 7](https://docs.microsoft.com/ja-jp/dotnet/articles/csharp/csharp-7), async methods may return other types in addition to `Task`, `Task<T>` and `void`.
The returned type must satisfy a certain pattern. These types are called "Task-like".

The `ContextFreeTask` struct in this library satisfies the "Task-like" pattern and the "awaitable" pattern.

### What ContextFreeTask does

This struct ignores synchronization context at all.
In other words,

- `await` operations in the methods which return `ContextFreeTask` capture no context
- `await` operations on `ContextFreeTask` capture no context

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

private async Task GAsync()
{
    // You can await on ContextFreeTask
    // ContextFreeTask don't capture current context
    await FAsync();
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

private async Task GAsync()
{
    await FAsync().ConfigureAwait(false);
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

Those awaiters always use `ConfigureAwait(false)`.

```cs
public struct ContextFreeTaskAwaiter : ICriticalNotifyCompletion
{
    private readonly Task _value;
    public void OnCompleted(Action continuation) => _value.ConfigureAwait(false).GetAwaiter().OnCompleted(continuation);
}
```

Furthermore, those AsyncMethodBuilders always clear current synchronization context.

```cs
public struct AsyncContextFreeTaskMethodBuilder
{
    private AsyncTaskMethodBuilder _methodBuilder;

    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        SynchronizationContext.SetSynchronizationContext(null);
        _methodBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
    }
}
```
