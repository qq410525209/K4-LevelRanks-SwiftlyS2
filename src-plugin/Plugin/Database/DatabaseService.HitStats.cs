using Dommel;
using Microsoft.Extensions.Logging;

namespace K4Ranks;

public sealed partial class Plugin
{
	public sealed partial class DatabaseService
	{
		// =========================================
		// =           CONSTANTS
		// =========================================

		internal const string HitsTableName = "lvl_base_hits";

		// =========================================
		// =           LOAD OPERATIONS
		// =========================================

		public async Task<HitData?> LoadHitDataAsync(string visibleSteamId)
		{
			if (!IsEnabled || !_modules.HitStatsEnabled)
				return null;

			try
			{
				using var connection = Core.Database.GetConnection(_connectionName);
				connection.Open();

				return await connection.GetAsync<HitData>(visibleSteamId);
			}
			catch (Exception ex)
			{
				Core.Logger.LogError(ex, "Failed to load hit data for {Steam}", visibleSteamId);
				return null;
			}
		}

		// =========================================
		// =           SAVE OPERATIONS
		// =========================================

		public async Task SaveHitDataAsync(HitData data)
		{
			if (!IsEnabled || !_modules.HitStatsEnabled || !data.IsDirty)
				return;

			try
			{
				using var connection = Core.Database.GetConnection(_connectionName);
				connection.Open();

				var existing = await connection.GetAsync<HitData>(data.Steam);
				if (existing != null)
				{
					await connection.UpdateAsync(data);
				}
				else
				{
					await connection.InsertAsync(data);
				}

				data.IsDirty = false;
			}
			catch (Exception ex)
			{
				Core.Logger.LogError(ex, "Failed to save hit data for {Steam}", data.Steam);
			}
		}
	}
}
