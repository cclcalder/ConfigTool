using System;

namespace Mvvm
{
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    public class MessageBus : IMessageBus
    {
        private static readonly Subject<IMessage> Stream = new Subject<IMessage>();

        public IObservable<IMessage> All
        {
            get
            {
                return Stream.AsObservable();
            }
        }

        public void Publish(IMessage message)
        {
            Stream.OnNext(message);
        }

        public IDisposable Subscribe<T>(Action<T> onNext) where T : IMessage
        {
            return Stream.OfType<T>().Subscribe(onNext);
        }

        public IDisposable Subscribe<T>(Action<T> onNext, Action<Exception> onError) where T : IMessage
        {
            return Stream.OfType<T>().Subscribe(onNext, onError);
        }

        public IDisposable Subscribe<T>(Action<T> onNext, Action onCompleted) where T : IMessage
        {
            return Stream.OfType<T>().Subscribe(onNext, onCompleted);
        }

        public IDisposable Subscribe<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted) where T : IMessage
        {
            return Stream.OfType<T>().Subscribe(onNext, onError, onCompleted);
        }

        private static IMessageBus _instance = new MessageBus();

        public static IMessageBus Instance
        {
            get { return _instance; }
        }

        internal static void SetInstance(IMessageBus instance)
        {
            _instance = instance;
        }
    }

    public interface IMessageBus
    {
        IObservable<IMessage> All { get; }
        void Publish(IMessage message);
        IDisposable Subscribe<T>(Action<T> onNext) where T : IMessage;
        IDisposable Subscribe<T>(Action<T> onNext, Action<Exception> onError) where T : IMessage;
        IDisposable Subscribe<T>(Action<T> onNext, Action onCompleted) where T : IMessage;
        IDisposable Subscribe<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted) where T : IMessage;
    }

    public interface IMessage
    {
        
    }
}
