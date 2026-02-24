using System;

public interface IReplicaEffect<TConfig, TModel>
{
    string EffectId { get; }

    void Initialize(ReplicaHostContext context, TConfig config);
    void SetModel(TModel model, bool animated = true);
    void PlayIn(ReplicaTransition transition);
    void PlayOut(ReplicaTransition transition, Action onComplete = null);
    void Tick(float deltaTime, float unscaledDeltaTime);
    void Dispose();
}
