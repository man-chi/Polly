﻿namespace Polly.Specs.Caching;

[Collection(Constants.SystemClockDependentTestCollection)]
public class CacheAsyncSpecs : IDisposable
{
    #region Configuration

    private static CancellationToken CancellationToken => CancellationToken.None;

    [Fact]
    public void Should_throw_when_action_is_null()
    {
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        Func<Context, CancellationToken, Task<EmptyStruct>> action = null!;
        Func<Context, CancellationToken, Task> actionVoid = null!;

        IAsyncCacheProvider asyncCacheProvider = new StubCacheProvider();
        ITtlStrategy ttlStrategy = new ContextualTtl();
        Func<Context, string> cacheKeyStrategy = (_) => string.Empty;
        Action<Context, string> onCacheGet = (_, _) => { };
        Action<Context, string> onCacheMiss = (_, _) => { };
        Action<Context, string> onCachePut = (_, _) => { };
        Action<Context, string, Exception>? onCacheGetError = null;
        Action<Context, string, Exception>? onCachePutError = null;

        var instance = Activator.CreateInstance(
            typeof(AsyncCachePolicy),
            flags,
            null,
            [
                asyncCacheProvider,
                ttlStrategy,
                cacheKeyStrategy,
                onCacheGet,
                onCacheMiss,
                onCachePut,
                onCacheGetError,
                onCachePutError,
            ],
            null)!;
        var instanceType = instance.GetType();
        var methods = instanceType.GetMethods(flags);
        var methodInfo = methods.First(method => method is { Name: "ImplementationAsync", ReturnType.Name: "Task`1" });
        var generic = methodInfo.MakeGenericMethod(typeof(EmptyStruct));

        var func = () => generic.Invoke(instance, [action, new Context(), CancellationToken, false]);

        var exceptionAssertions = Should.Throw<TargetInvocationException>(func);
        exceptionAssertions.Message.ShouldBe("Exception has been thrown by the target of an invocation.");
        exceptionAssertions.InnerException.ShouldBeOfType<ArgumentNullException>()
            .ParamName.ShouldBe("action");

        methodInfo = methods.First(method => method is { Name: "ImplementationAsync", ReturnType.Name: "Task" });

        func = () => methodInfo.Invoke(instance, [actionVoid, new Context(), CancellationToken, false]);

        exceptionAssertions = Should.Throw<TargetInvocationException>(func);
        exceptionAssertions.Message.ShouldBe("Exception has been thrown by the target of an invocation.");
        exceptionAssertions.InnerException.ShouldBeOfType<ArgumentNullException>()
            .ParamName.ShouldBe("action");
    }

    [Fact]
    public void Should_throw_when_cache_provider_is_null()
    {
        IAsyncCacheProvider cacheProvider = null!;
        var ttl = TimeSpan.MaxValue;
        ITtlStrategy ttlStrategy = new ContextualTtl();
        ICacheKeyStrategy cacheKeyStrategy = new StubCacheKeyStrategy(context => context.OperationKey + context["id"]);
        Func<Context, string> cacheKeyStrategyFunc = (_) => string.Empty;
        Action<Context, string> onCache = (_, _) => { };
        Action<Context, string, Exception>? onCacheError = null;
        const string CacheProviderExpected = "cacheProvider";

        Action action = () => Policy.CacheAsync(cacheProvider, ttl, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategy, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategyFunc, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategy, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategyFunc, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);

        action = () => Policy.CacheAsync<string>(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheProviderExpected);
    }

    [Fact]
    public void Should_throw_when_ttl_strategy_is_null()
    {
        var cacheProvider = new StubCacheProvider();
        ITtlStrategy ttlStrategy = null!;
        ICacheKeyStrategy cacheKeyStrategy = new StubCacheKeyStrategy(context => context.OperationKey + context["id"]);
        Func<Context, string> cacheKeyStrategyFunc = (_) => string.Empty;
        Action<Context, string> onCache = (_, _) => { };
        Action<Context, string, Exception>? onCacheError = null;
        const string TtlStrategyExpected = "ttlStrategy";

        Action action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(TtlStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(TtlStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(TtlStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(TtlStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(TtlStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCache, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(TtlStrategyExpected);
    }

    [Fact]
    public void Should_throw_when_cache_key_strategy_is_null()
    {
        var cacheProvider = new StubCacheProvider();
        var ttl = TimeSpan.MaxValue;
        ITtlStrategy ttlStrategy = new ContextualTtl();
        ICacheKeyStrategy cacheKeyStrategy = null!;
        Func<Context, string> cacheKeyStrategyFunc = null!;
        Action<Context, string> onCache = (_, _) => { };
        Action<Context, string, Exception>? onCacheError = null;
        const string CacheKeyStrategyExpected = "cacheKeyStrategy";

        Action action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategy, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategyFunc, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);

        action = () => Policy.CacheAsync(
            cacheProvider,
            ttl,
            cacheKeyStrategy,
            onCache,
            onCache,
            onCache,
            onCacheError,
            onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);

        action = () => Policy.CacheAsync(
            cacheProvider,
            ttlStrategy,
            cacheKeyStrategy,
            onCache,
            onCache,
            onCache,
            onCacheError,
            onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);

        action = () => Policy.CacheAsync(
            cacheProvider,
            ttl,
            cacheKeyStrategyFunc,
            onCache,
            onCache,
            onCache,
            onCacheError,
            onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);

        action = () => Policy.CacheAsync(
            cacheProvider,
            ttlStrategy,
            cacheKeyStrategyFunc,
            onCache,
            onCache,
            onCache,
            onCacheError,
            onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(CacheKeyStrategyExpected);
    }

    [Fact]
    public void Should_throw_when_on_cache_get_is_null()
    {
        var cacheProvider = new StubCacheProvider();
        var ttl = TimeSpan.MaxValue;
        ITtlStrategy ttlStrategy = new ContextualTtl();
        ICacheKeyStrategy cacheKeyStrategy = new StubCacheKeyStrategy(context => context.OperationKey + context["id"]);
        Func<Context, string> cacheKeyStrategyFunc = (_) => string.Empty;
        Action<Context, string> onCacheGet = null!;
        Action<Context, string> onCache = (_, _) => { };
        Action<Context, string, Exception>? onCacheError = null;
        const string OnCacheGetExpected = "onCacheGet";

        Action action = () => Policy.CacheAsync(cacheProvider, ttl, onCacheGet, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheGetExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, onCacheGet, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheGetExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategy, onCacheGet, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheGetExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCacheGet, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheGetExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategyFunc, onCacheGet, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheGetExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCacheGet, onCache, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheGetExpected);
    }

    [Fact]
    public void Should_throw_when_on_cache_miss_is_null()
    {
        var cacheProvider = new StubCacheProvider();
        var ttl = TimeSpan.MaxValue;
        ITtlStrategy ttlStrategy = new ContextualTtl();
        var cacheKeyStrategy = new StubCacheKeyStrategy(context => context.OperationKey + context["id"]);
        Func<Context, string> cacheKeyStrategyFunc = (_) => string.Empty;
        Action<Context, string> onCacheMiss = null!;
        Action<Context, string> onCache = (_, _) => { };
        Action<Context, string, Exception>? onCacheError = null;
        const string OnCacheMissExpected = "onCacheMiss";

        Action action = () => Policy.CacheAsync(cacheProvider, ttl, onCache, onCacheMiss, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheMissExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, onCache, onCacheMiss, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheMissExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategy, onCache, onCacheMiss, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheMissExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCache, onCacheMiss, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheMissExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategyFunc, onCache, onCacheMiss, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheMissExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCache, onCacheMiss, onCache, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCacheMissExpected);
    }

    [Fact]
    public void Should_throw_when_on_cache_put_is_null()
    {
        IAsyncCacheProvider cacheProvider = new StubCacheProvider();
        var ttl = TimeSpan.MaxValue;
        ITtlStrategy ttlStrategy = new ContextualTtl();
        ICacheKeyStrategy cacheKeyStrategy = new StubCacheKeyStrategy(context => context.OperationKey + context["id"]);
        Func<Context, string> cacheKeyStrategyFunc = (_) => string.Empty;
        Action<Context, string> onCachePut = null!;
        Action<Context, string> onCache = (_, _) => { };
        Action<Context, string, Exception>? onCacheError = null;
        const string OnCachePutExpected = "onCachePut";

        Action action = () => Policy.CacheAsync(cacheProvider, ttl, onCache, onCache, onCachePut, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCachePutExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, onCache, onCache, onCachePut, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCachePutExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategy, onCache, onCache, onCachePut, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCachePutExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategy, onCache, onCache, onCachePut, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCachePutExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttl, cacheKeyStrategyFunc, onCache, onCache, onCachePut, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCachePutExpected);

        action = () => Policy.CacheAsync(cacheProvider, ttlStrategy, cacheKeyStrategyFunc, onCache, onCache, onCachePut, onCacheError, onCacheError);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe(OnCachePutExpected);
    }

    [Fact]
    public void Should_throw_when_policies_is_null()
    {
        IAsyncPolicy[] policies = null!;
        IAsyncPolicy<int>[] policiesGeneric = null!;

        Action action = () => Policy.WrapAsync(policies);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe("policies");

        action = () => Policy.WrapAsync(policiesGeneric);
        Should.Throw<ArgumentNullException>(action).ParamName.ShouldBe("policies");
    }
    #endregion

    #region Caching behaviours

    [Fact]
    public async Task Should_return_value_from_cache_and_not_execute_delegate_if_cache_holds_value()
    {
        const string ValueToReturnFromCache = "valueToReturnFromCache";
        const string ValueToReturnFromExecution = "valueToReturnFromExecution";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);
        await stubCacheProvider.PutAsync(OperationKey, ValueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;

        (await cache.ExecuteAsync(async _ =>
        {
            delegateExecuted = true;
            await TaskHelper.EmptyTask;
            return ValueToReturnFromExecution;
        }, new Context(OperationKey)))
            .ShouldBe(ValueToReturnFromCache);

        delegateExecuted.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_execute_delegate_and_put_value_in_cache_if_cache_does_not_hold_value()
    {
        const string ValueToReturn = "valueToReturn";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);

        (bool cacheHit1, object? fromCache1) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit1.ShouldBeFalse();
        fromCache1.ShouldBeNull();

        (await cache.ExecuteAsync(async _ => { await TaskHelper.EmptyTask; return ValueToReturn; }, new Context(OperationKey))).ShouldBe(ValueToReturn);

        (bool cacheHit2, object? fromCache2) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit2.ShouldBeTrue();
        fromCache2.ShouldBe(ValueToReturn);
    }

    [Fact]
    public async Task Should_execute_delegate_and_put_value_in_cache_but_when_it_expires_execute_delegate_again()
    {
        const string ValueToReturn = "valueToReturn";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var ttl = TimeSpan.FromMinutes(30);
        var cache = Policy.CacheAsync(stubCacheProvider, ttl);

        (bool cacheHit1, object? fromCache1) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit1.ShouldBeFalse();
        fromCache1.ShouldBeNull();

        int delegateInvocations = 0;
        Func<Context, Task<string>> func = async _ =>
        {
            delegateInvocations++;
            await TaskHelper.EmptyTask;
            return ValueToReturn;
        };

        DateTimeOffset fixedTime = SystemClock.DateTimeOffsetUtcNow();
        SystemClock.DateTimeOffsetUtcNow = () => fixedTime;

        // First execution should execute delegate and put result in the cache.
        (await cache.ExecuteAsync(func, new Context(OperationKey))).ShouldBe(ValueToReturn);
        delegateInvocations.ShouldBe(1);
        (bool cacheHit2, object? fromCache2) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit2.ShouldBeTrue();
        fromCache2.ShouldBe(ValueToReturn);

        // Second execution (before cache expires) should get it from the cache - no further delegate execution.
        // (Manipulate time so just prior cache expiry).
        SystemClock.DateTimeOffsetUtcNow = () => fixedTime.Add(ttl).AddSeconds(-1);
        (await cache.ExecuteAsync(func, new Context(OperationKey))).ShouldBe(ValueToReturn);
        delegateInvocations.ShouldBe(1);

        // Manipulate time to force cache expiry.
        SystemClock.DateTimeOffsetUtcNow = () => fixedTime.Add(ttl).AddSeconds(1);

        // Third execution (cache expired) should not get it from the cache - should cause further delegate execution.
        (await cache.ExecuteAsync(func, new Context(OperationKey))).ShouldBe(ValueToReturn);
        delegateInvocations.ShouldBe(2);
    }

    [Fact]
    public async Task Should_execute_delegate_but_not_put_value_in_cache_if_cache_does_not_hold_value_but_ttl_indicates_not_worth_caching()
    {
        const string ValueToReturn = "valueToReturn";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.Zero);

        (bool cacheHit1, object? fromCache1) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit1.ShouldBeFalse();
        fromCache1.ShouldBeNull();

        (await cache.ExecuteAsync(async _ => { await TaskHelper.EmptyTask; return ValueToReturn; }, new Context(OperationKey))).ShouldBe(ValueToReturn);

        (bool cacheHit2, object? fromCache2) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit2.ShouldBeFalse();
        fromCache2.ShouldBeNull();
    }

    [Fact]
    public async Task Should_return_value_from_cache_and_not_execute_delegate_if_prior_execution_has_cached()
    {
        const string ValueToReturn = "valueToReturn";
        const string OperationKey = "SomeOperationKey";

        var cache = Policy.CacheAsync(new StubCacheProvider(), TimeSpan.MaxValue);

        int delegateInvocations = 0;
        Func<Context, Task<string>> func = async _ =>
        {
            delegateInvocations++;
            await TaskHelper.EmptyTask;
            return ValueToReturn;
        };

        (await cache.ExecuteAsync(func, new Context(OperationKey))).ShouldBe(ValueToReturn);
        delegateInvocations.ShouldBe(1);

        (await cache.ExecuteAsync(func, new Context(OperationKey))).ShouldBe(ValueToReturn);
        delegateInvocations.ShouldBe(1);

        (await cache.ExecuteAsync(func, new Context(OperationKey))).ShouldBe(ValueToReturn);
        delegateInvocations.ShouldBe(1);
    }

    [Fact]
    public async Task Should_allow_custom_FuncCacheKeyStrategy()
    {
        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue, context => context.OperationKey + context["id"]);

        object person1 = new();
        await stubCacheProvider.PutAsync("person1", person1, new Ttl(TimeSpan.MaxValue), CancellationToken, false);
        object person2 = new();
        await stubCacheProvider.PutAsync("person2", person2, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool funcExecuted = false;
        Func<Context, Task<object>> func = async _ => { funcExecuted = true; await TaskHelper.EmptyTask; return new object(); };

        (await cache.ExecuteAsync(func, new Context("person", CreateDictionary("id", "1")))).ShouldBeSameAs(person1);
        funcExecuted.ShouldBeFalse();

        (await cache.ExecuteAsync(func, new Context("person", CreateDictionary("id", "2")))).ShouldBeSameAs(person2);
        funcExecuted.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_allow_custom_ICacheKeyStrategy()
    {
        Action<Context, string, Exception> noErrorHandling = (_, _, _) => { };
        Action<Context, string> emptyDelegate = (_, _) => { };

        var stubCacheProvider = new StubCacheProvider();
        var cacheKeyStrategy = new StubCacheKeyStrategy(context => context.OperationKey + context["id"]);
        var cache = Policy.CacheAsync(stubCacheProvider, new RelativeTtl(TimeSpan.MaxValue), cacheKeyStrategy, emptyDelegate, emptyDelegate, emptyDelegate, noErrorHandling, noErrorHandling);

        object person1 = new();
        await stubCacheProvider.PutAsync("person1", person1, new Ttl(TimeSpan.MaxValue), CancellationToken, false);
        object person2 = new();
        await stubCacheProvider.PutAsync("person2", person2, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool funcExecuted = false;
        Func<Context, Task<object>> func = async _ => { funcExecuted = true; await TaskHelper.EmptyTask; return new object(); };

        (await cache.ExecuteAsync(func, new Context("person", CreateDictionary("id", "1")))).ShouldBeSameAs(person1);
        funcExecuted.ShouldBeFalse();

        (await cache.ExecuteAsync(func, new Context("person", CreateDictionary("id", "2")))).ShouldBeSameAs(person2);
        funcExecuted.ShouldBeFalse();
    }

    #endregion

    #region Caching behaviours, default(TResult)

    [Fact]
    public async Task Should_execute_delegate_and_put_value_in_cache_if_cache_does_not_hold_value__default_for_reference_type()
    {
        ResultClass? valueToReturn = null;
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);

        (bool cacheHit1, object? fromCache1) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit1.ShouldBeFalse();
        fromCache1.ShouldBeNull();

        (await cache.ExecuteAsync(async _ => { await TaskHelper.EmptyTask; return valueToReturn; }, new Context(OperationKey))).ShouldBe(valueToReturn);

        (bool cacheHit2, object? fromCache2) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit2.ShouldBeTrue();
        fromCache2.ShouldBe(valueToReturn);
    }

    [Fact]
    public async Task Should_return_value_from_cache_and_not_execute_delegate_if_cache_holds_value__default_for_reference_type()
    {
        ResultClass? valueToReturnFromCache = null;
        ResultClass valueToReturnFromExecution = new ResultClass(ResultPrimitive.Good);
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);
        await stubCacheProvider.PutAsync(OperationKey, valueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;

        (await cache.ExecuteAsync(async _ =>
                {
                    delegateExecuted = true;
                    await TaskHelper.EmptyTask;
                    return valueToReturnFromExecution;
                }, new Context(OperationKey)))
            .ShouldBe(valueToReturnFromCache);

        delegateExecuted.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_execute_delegate_and_put_value_in_cache_if_cache_does_not_hold_value__default_for_value_type()
    {
        ResultPrimitive valueToReturn = default;
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);

        (bool cacheHit1, object? fromCache1) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit1.ShouldBeFalse();
        fromCache1.ShouldBeNull();

        (await cache.ExecuteAsync(async _ => { await TaskHelper.EmptyTask; return valueToReturn; }, new Context(OperationKey))).ShouldBe(valueToReturn);

        (bool cacheHit2, object? fromCache2) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit2.ShouldBeTrue();
        fromCache2.ShouldBe(valueToReturn);
    }

    [Fact]
    public async Task Should_return_value_from_cache_and_not_execute_delegate_if_cache_holds_value__default_for_value_type()
    {
        ResultPrimitive valueToReturnFromCache = default;
        ResultPrimitive valueToReturnFromExecution = ResultPrimitive.Good;
        valueToReturnFromExecution.ShouldNotBe(valueToReturnFromCache);
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);
        await stubCacheProvider.PutAsync(OperationKey, valueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;

        (await cache.ExecuteAsync(async _ =>
                {
                    delegateExecuted = true;
                    await TaskHelper.EmptyTask;
                    return valueToReturnFromExecution;
                }, new Context(OperationKey)))
            .ShouldBe(valueToReturnFromCache);

        delegateExecuted.ShouldBeFalse();
    }

    #endregion

    #region Non-generic CachePolicy in non-generic PolicyWrap

    [Fact]
    public async Task Should_return_value_from_cache_and_not_execute_delegate_if_cache_holds_value_when_outermost_in_policywrap()
    {
        const string ValueToReturnFromCache = "valueToReturnFromCache";
        const string ValueToReturnFromExecution = "valueToReturnFromExecution";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);
        var noop = Policy.NoOpAsync();
        var wrap = Policy.WrapAsync(cache, noop);

        await stubCacheProvider.PutAsync(OperationKey, ValueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;

        (await wrap.ExecuteAsync(async _ =>
        {
            delegateExecuted = true;
            await TaskHelper.EmptyTask;
            return ValueToReturnFromExecution;
        }, new Context(OperationKey)))
            .ShouldBe(ValueToReturnFromCache);

        delegateExecuted.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_return_value_from_cache_and_not_execute_delegate_if_cache_holds_value_when_innermost_in_policywrap()
    {
        const string ValueToReturnFromCache = "valueToReturnFromCache";
        const string ValueToReturnFromExecution = "valueToReturnFromExecution";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);
        var noop = Policy.NoOpAsync();
        var wrap = Policy.WrapAsync(noop, cache);

        await stubCacheProvider.PutAsync(OperationKey, ValueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;

        (await wrap.ExecuteAsync(async _ =>
        {
            delegateExecuted = true;
            await TaskHelper.EmptyTask;
            return ValueToReturnFromExecution;
        }, new Context(OperationKey)))
            .ShouldBe(ValueToReturnFromCache);

        delegateExecuted.ShouldBeFalse();
    }

    [Fact]
    public async Task Should_return_value_from_cache_and_not_execute_delegate_if_cache_holds_value_when_mid_policywrap()
    {
        const string ValueToReturnFromCache = "valueToReturnFromCache";
        const string ValueToReturnFromExecution = "valueToReturnFromExecution";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);
        var noop = Policy.NoOpAsync();
        var wrap = Policy.WrapAsync(noop, cache, noop);

        await stubCacheProvider.PutAsync(OperationKey, ValueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;

        (await wrap.ExecuteAsync(async _ =>
        {
            delegateExecuted = true;
            await TaskHelper.EmptyTask;
            return ValueToReturnFromExecution;
        }, new Context(OperationKey)))
            .ShouldBe(ValueToReturnFromCache);

        delegateExecuted.ShouldBeFalse();
    }

    #endregion

    #region No-op pass-through behaviour

    [Fact]
    public async Task Should_always_execute_delegate_if_execution_key_not_set()
    {
        string valueToReturn = Guid.NewGuid().ToString();

        var cache = Policy.CacheAsync(new StubCacheProvider(), TimeSpan.MaxValue);

        int delegateInvocations = 0;
        Func<Task<string>> func = async () =>
        {
            delegateInvocations++;
            await TaskHelper.EmptyTask;
            return valueToReturn;
        };

        (await cache.ExecuteAsync(func /*, no operation key */)).ShouldBe(valueToReturn);
        delegateInvocations.ShouldBe(1);

        (await cache.ExecuteAsync(func /*, no operation key */)).ShouldBe(valueToReturn);
        delegateInvocations.ShouldBe(2);
    }

    [Fact]
    public void Should_always_execute_delegate_if_execution_is_void_returning()
    {
        string operationKey = "SomeKey";

        var cache = Policy.CacheAsync(new StubCacheProvider(), TimeSpan.MaxValue);

        int delegateInvocations = 0;
        Func<Context, Task> action = async _ => { delegateInvocations++; await TaskHelper.EmptyTask; };

        cache.ExecuteAsync(action, new Context(operationKey));
        delegateInvocations.ShouldBe(1);

        cache.ExecuteAsync(action, new Context(operationKey));
        delegateInvocations.ShouldBe(2);
    }

    #endregion

    #region Cancellation

    [Fact]
    public async Task Should_honour_cancellation_even_if_prior_execution_has_cached()
    {
        const string ValueToReturn = "valueToReturn";
        const string OperationKey = "SomeOperationKey";

        var policy = Policy.CacheAsync(new StubCacheProvider(), TimeSpan.MaxValue);

        int delegateInvocations = 0;

        using (var tokenSource = new CancellationTokenSource())
        {
            Func<Context, CancellationToken, Task<string>> func = async (_, _) =>
            {
                // delegate does not observe cancellation token; test is whether CacheEngine does.
                delegateInvocations++;
                await TaskHelper.EmptyTask;
                return ValueToReturn;
            };

            (await policy.ExecuteAsync(func, new Context(OperationKey), tokenSource.Token)).ShouldBe(ValueToReturn);
            delegateInvocations.ShouldBe(1);

            tokenSource.Cancel();

            await Should.ThrowAsync<OperationCanceledException>(() => policy.ExecuteAsync(func, new Context(OperationKey), tokenSource.Token));
        }

        delegateInvocations.ShouldBe(1);
    }

    [Fact]
    public async Task Should_honour_cancellation_during_delegate_execution_and_not_put_to_cache()
    {
        const string ValueToReturn = "valueToReturn";
        const string OperationKey = "SomeOperationKey";

        var stubCacheProvider = new StubCacheProvider();
        var policy = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue);

        using (var tokenSource = new CancellationTokenSource())
        {
            Func<Context, CancellationToken, Task<string>> func = async (_, ct) =>
            {
                tokenSource.Cancel(); // simulate cancellation raised during delegate execution
                ct.ThrowIfCancellationRequested();
                await TaskHelper.EmptyTask;
                return ValueToReturn;
            };

            await Should.ThrowAsync<OperationCanceledException>(() => policy.ExecuteAsync(func, new Context(OperationKey), tokenSource.Token));
        }

        (bool cacheHit, object? fromCache) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit.ShouldBeFalse();
        fromCache.ShouldBeNull();
    }

    #endregion

    #region Policy hooks

    [Fact]
    public async Task Should_call_onError_delegate_if_cache_get_errors()
    {
        Exception ex = new Exception();
        var stubCacheProvider = new StubErroringCacheProvider(getException: ex, putException: null);

        Exception? exceptionFromCacheProvider = null;

        const string ValueToReturnFromCache = "valueToReturnFromCache";
        const string ValueToReturnFromExecution = "valueToReturnFromExecution";
        const string OperationKey = "SomeOperationKey";

        Action<Context, string, Exception> onError = (_, _, exc) => exceptionFromCacheProvider = exc;

        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue, onError);

        await stubCacheProvider.PutAsync(OperationKey, ValueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;

        // Even though value is in cache, get will error; so value is returned from execution.
        (await cache.ExecuteAsync(async _ =>
        {
            delegateExecuted = true;
            await TaskHelper.EmptyTask;
            return ValueToReturnFromExecution;

        }, new Context(OperationKey)))
           .ShouldBe(ValueToReturnFromExecution);
        delegateExecuted.ShouldBeTrue();

        // And error should be captured by onError delegate.
        exceptionFromCacheProvider.ShouldBe(ex);
    }

    [Fact]
    public async Task Should_call_onError_delegate_if_cache_put_errors()
    {
        var ex = new Exception();
        var stubCacheProvider = new StubErroringCacheProvider(getException: null, putException: ex);

        Exception? exceptionFromCacheProvider = null;

        const string ValueToReturn = "valueToReturn";
        const string OperationKey = "SomeOperationKey";

        Action<Context, string, Exception> onError = (_, _, exc) => exceptionFromCacheProvider = exc;

        var cache = Policy.CacheAsync(stubCacheProvider, TimeSpan.MaxValue, onError);

        (bool cacheHit1, object? fromCache1) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit1.ShouldBeFalse();
        fromCache1.ShouldBeNull();

        (await cache.ExecuteAsync(async _ => { await TaskHelper.EmptyTask; return ValueToReturn; }, new Context(OperationKey))).ShouldBe(ValueToReturn);

        // error should be captured by onError delegate.
        exceptionFromCacheProvider.ShouldBe(ex);

        // failed to put it in the cache
        (bool cacheHit2, object? fromCache2) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit2.ShouldBeFalse();
        fromCache2.ShouldBeNull();
    }

    [Fact]
    public async Task Should_execute_oncacheget_after_got_from_cache()
    {
        const string ValueToReturnFromCache = "valueToReturnFromCache";
        const string ValueToReturnFromExecution = "valueToReturnFromExecution";

        const string OperationKey = "SomeOperationKey";
        string? keyPassedToDelegate = null;

        Context contextToExecute = new Context(OperationKey);
        Context? contextPassedToDelegate = null;

        Action<Context, string, Exception> noErrorHandling = (_, _, _) => { };
        Action<Context, string> emptyDelegate = (_, _) => { };
        Action<Context, string> onCacheAction = (ctx, key) =>
        {
            contextPassedToDelegate = ctx;
            keyPassedToDelegate = key;
        };

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, new RelativeTtl(TimeSpan.MaxValue), DefaultCacheKeyStrategy.Instance, onCacheAction, emptyDelegate, emptyDelegate, noErrorHandling, noErrorHandling);
        await stubCacheProvider.PutAsync(OperationKey, ValueToReturnFromCache, new Ttl(TimeSpan.MaxValue), CancellationToken, false);

        bool delegateExecuted = false;
        (await cache.ExecuteAsync(async _ =>
                {
                    delegateExecuted = true;
                    await TaskHelper.EmptyTask;
                    return ValueToReturnFromExecution;
                }, contextToExecute))
            .ShouldBe(ValueToReturnFromCache);
        delegateExecuted.ShouldBeFalse();

        contextPassedToDelegate.ShouldBeSameAs(contextToExecute);
        keyPassedToDelegate.ShouldBe(OperationKey);
    }

    [Fact]
    public async Task Should_execute_oncachemiss_and_oncacheput_if_cache_does_not_hold_value_and_put()
    {
        const string ValueToReturn = "valueToReturn";

        const string OperationKey = "SomeOperationKey";
        string? keyPassedToOnCacheMiss = null;
        string? keyPassedToOnCachePut = null;

        Context contextToExecute = new Context(OperationKey);
        Context? contextPassedToOnCacheMiss = null;
        Context? contextPassedToOnCachePut = null;

        Action<Context, string, Exception> noErrorHandling = (_, _, _) => { };
        Action<Context, string> emptyDelegate = (_, _) => { };
        Action<Context, string> onCacheMiss = (ctx, key) => { contextPassedToOnCacheMiss = ctx; keyPassedToOnCacheMiss = key; };
        Action<Context, string> onCachePut = (ctx, key) => { contextPassedToOnCachePut = ctx; keyPassedToOnCachePut = key; };

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, new RelativeTtl(TimeSpan.MaxValue), DefaultCacheKeyStrategy.Instance, emptyDelegate, onCacheMiss, onCachePut, noErrorHandling, noErrorHandling);

        (bool cacheHit1, object? fromCache1) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit1.ShouldBeFalse();
        fromCache1.ShouldBeNull();

        (await cache.ExecuteAsync(async _ => { await TaskHelper.EmptyTask; return ValueToReturn; }, contextToExecute)).ShouldBe(ValueToReturn);

        (bool cacheHit2, object? fromCache2) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit2.ShouldBeTrue();
        fromCache2.ShouldBe(ValueToReturn);

        contextPassedToOnCachePut.ShouldBeSameAs(contextToExecute);
        keyPassedToOnCachePut.ShouldBe(OperationKey);
        contextPassedToOnCacheMiss.ShouldNotBeNull();
        keyPassedToOnCacheMiss.ShouldBe("SomeOperationKey");
    }

    [Fact]
    public async Task Should_execute_oncachemiss_but_not_oncacheput_if_cache_does_not_hold_value_and_returned_value_not_worth_caching()
    {
        const string ValueToReturn = "valueToReturn";

        const string OperationKey = "SomeOperationKey";
        string? keyPassedToOnCacheMiss = null;
        string? keyPassedToOnCachePut = null;

        Context contextToExecute = new Context(OperationKey);
        Context? contextPassedToOnCacheMiss = null;
        Context? contextPassedToOnCachePut = null;

        Action<Context, string, Exception> noErrorHandling = (_, _, _) => { };
        Action<Context, string> emptyDelegate = (_, _) => { };
        Action<Context, string> onCacheMiss = (ctx, key) => { contextPassedToOnCacheMiss = ctx; keyPassedToOnCacheMiss = key; };
        Action<Context, string> onCachePut = (ctx, key) => { contextPassedToOnCachePut = ctx; keyPassedToOnCachePut = key; };

        var stubCacheProvider = new StubCacheProvider();
        var cache = Policy.CacheAsync(stubCacheProvider, new RelativeTtl(TimeSpan.Zero), DefaultCacheKeyStrategy.Instance, emptyDelegate, onCacheMiss, onCachePut, noErrorHandling, noErrorHandling);

        (bool cacheHit, object? fromCache) = await stubCacheProvider.TryGetAsync(OperationKey, CancellationToken, false);
        cacheHit.ShouldBeFalse();
        fromCache.ShouldBeNull();

        (await cache.ExecuteAsync(async _ => { await TaskHelper.EmptyTask; return ValueToReturn; }, contextToExecute)).ShouldBe(ValueToReturn);

        contextPassedToOnCachePut.ShouldBeNull();
        keyPassedToOnCachePut.ShouldBeNull();
        contextPassedToOnCacheMiss.ShouldBeEmpty();
        keyPassedToOnCacheMiss.ShouldBe("SomeOperationKey");
    }

    [Fact]
    public async Task Should_not_execute_oncachemiss_if_dont_query_cache_because_cache_key_not_set()
    {
        string valueToReturn = Guid.NewGuid().ToString();

        Action<Context, string, Exception> noErrorHandling = (_, _, _) => { };
        Action<Context, string> emptyDelegate = (_, _) => { };

        bool onCacheMissExecuted = false;
        Action<Context, string> onCacheMiss = (_, _) => onCacheMissExecuted = true;

        var cache = Policy.CacheAsync(new StubCacheProvider(), new RelativeTtl(TimeSpan.MaxValue), DefaultCacheKeyStrategy.Instance, emptyDelegate, onCacheMiss, emptyDelegate, noErrorHandling, noErrorHandling);

        (await cache.ExecuteAsync(async () =>
        {
            await TaskHelper.EmptyTask;
            return valueToReturn;
        }  /*, no operation key */))
        .ShouldBe(valueToReturn);

        onCacheMissExecuted.ShouldBeFalse();
    }

    #endregion

    public void Dispose() =>
        SystemClock.Reset();
}
