using Npgsql;
using Pgvector;

namespace BindingChaos.SignalProcessing;

/// <summary>
/// Persists signal embeddings to the <c>signal_processing.signal_embeddings</c> pgvector table.
/// </summary>
public sealed class SignalEmbeddingRepository(NpgsqlDataSource dataSource) : ISignalEmbeddingRepository
{
    /// <inheritdoc />
    public async Task UpsertAsync(string signalId, float[] embedding)
    {
        var connection = await dataSource.OpenConnectionAsync().ConfigureAwait(false);
        await using (connection.ConfigureAwait(false))
        {
            var cmd = connection.CreateCommand();
            await using (cmd.ConfigureAwait(false))
            {
                cmd.CommandText = """
                    INSERT INTO signal_processing.signal_embeddings (signal_id, embedding)
                    VALUES ($1, $2)
                    ON CONFLICT (signal_id) DO UPDATE SET embedding = $2
                    """;
                cmd.Parameters.AddWithValue(signalId);
                cmd.Parameters.AddWithValue(new Vector(embedding));
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SignalEmbedding>> GetAllAsync()
    {
        var connection = await dataSource.OpenConnectionAsync().ConfigureAwait(false);
        await using (connection.ConfigureAwait(false))
        {
            var cmd = connection.CreateCommand();
            await using (cmd.ConfigureAwait(false))
            {
                cmd.CommandText = "SELECT signal_id, embedding FROM signal_processing.signal_embeddings";
                var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
                await using (reader.ConfigureAwait(false))
                {
                    var results = new List<SignalEmbedding>();
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var signalId = reader.GetString(0);
                        var vector = reader.GetFieldValue<Vector>(1);
                        results.Add(new SignalEmbedding(signalId, vector.ToArray()));
                    }

                    return results;
                }
            }
        }
    }
}
