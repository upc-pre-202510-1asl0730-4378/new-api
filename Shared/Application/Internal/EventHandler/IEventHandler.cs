using Cortex.Mediator.Notifications;
using eb4378u202318323.API.Shared.Domain.Model.Events;

namespace eb4378u202318323.API.Shared.Application.Internal.EventHandler;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
    
}