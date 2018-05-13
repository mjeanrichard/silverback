﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Silverback.Domain;
using Silverback.Extensions;
using Silverback.Messaging.Publishing;

namespace Silverback.EntityFrameworkCore
{
    /// <summary>
    /// Exposes an extension method for <see cref="IEventPublisher{TEvent}"/> that automatically publishes the events in all entities tracked by a <see cref="DbContext"/>
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the domain events generated by the tracked entities.
        /// </summary>
        /// <param name="publisher">The events publisher.</param>
        /// <param name="dbContext">The <see cref="DbContext"/> tracking the entities that must publish the events.</param>
        /// <returns></returns>
        public static void PublishDomainEvents(this IEventPublisher<IDomainEvent<IDomainEntity>> publisher, DbContext dbContext)
        {
            var events = GetPendingEvents(dbContext);

            // Keep publishing events fired inside the event handlers
            while (events.Any())
            {
                events.ForEach(publisher.Publish);
                events = GetPendingEvents(dbContext);
            }
        }

        /// <summary>
        /// Publishes the domain events generated by the tracked entities.
        /// </summary>
        /// <param name="publisher">The events publisher.</param>
        /// <param name="dbContext">The <see cref="DbContext"/> tracking the entities that must publish the events.</param>
        /// <returns></returns>
        public static async Task PublishDomainEventsAsync(this IEventPublisher<IDomainEvent<IDomainEntity>> publisher, DbContext dbContext)
        {
            var events = GetPendingEvents(dbContext);

            // Keep publishing events fired inside the event handlers
            while (events.Any())
            {
                await events.ForEachAsync(publisher.PublishAsync);
                events = GetPendingEvents(dbContext);
            }
        }

        /// <summary>
        /// Gets the events to be published.
        /// </summary>
        /// <returns></returns>
        private static List<IDomainEvent<IDomainEntity>> GetPendingEvents(DbContext dbContext)
        {
            var events = dbContext.ChangeTracker.Entries<IDomainEntity>()
                .Where(e => e.Entity.GetDomainEvents() != null)
                .SelectMany(e => e.Entity.GetDomainEvents())
                .ToList();

            // Clear events to avoid firing the same event multiple times during the recursion
            events.ForEach(e => e.Source.ClearEvents());

            return events;
        }
    }
}
