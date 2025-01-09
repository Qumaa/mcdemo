namespace Project.World.Generation.Chunks
{
    public readonly ref struct QueryResult<T>
    {
        public readonly T Value;
        public readonly QueryStatus Status;

        public static QueryResult<T> Failed => new(QueryStatus.Failed);

        public static QueryResult<T> Successful(T val) =>
            new(val, QueryStatus.Successful);
        
        public QueryResult(T value, QueryStatus status)
        {
            Value = value;
            Status = status;
        }

        public QueryResult(QueryStatus status) : this() {
            Status = status;
        }
    }
}
