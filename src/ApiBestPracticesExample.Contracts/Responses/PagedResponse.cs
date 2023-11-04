namespace ApiBestPracticesExample.Contracts.Responses;
public sealed record PagedResponse<TEntity>
{
	public List<TEntity> Data { get; set; } = default!;
	public int TotalCount { get; set; }
}