namespace ONGES.Users.Application.Events
{
    public interface IEventStore
    {
        public Task AppendAsync<T>(string aggregateId, T evt, int version, string correlationId);
        public Task<IReadOnlyList<IDomainEvent>> GetEventsAsync(string aggregateId);
    }
}
