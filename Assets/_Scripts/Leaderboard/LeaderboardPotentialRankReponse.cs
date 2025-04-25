using System;

namespace Assets._Scripts.Leaderboard
{
    public class LeaderboardPotentialRankResponse
    {
        private readonly int? _rank;

        public bool HasError { get; private set; }
        public LeaderboardPotentialRankReponseErrorType Error { get; private set; }

        public bool HasRank => !HasError && _rank.HasValue;

        public int Rank
        {
            get
            {
                if (HasError || !_rank.HasValue)
                    throw new InvalidOperationException("Cannot access Rank because an error occurred or Rank is not set.");
                return _rank.Value;
            }
        }

        private LeaderboardPotentialRankResponse(int? rank, bool hasError, LeaderboardPotentialRankReponseErrorType error)
        {
            _rank = rank;
            HasError = hasError;
            Error = error;
        }

        public static LeaderboardPotentialRankResponse Success(int rank) =>
            new LeaderboardPotentialRankResponse(rank, false, LeaderboardPotentialRankReponseErrorType.None);

        public static LeaderboardPotentialRankResponse Failure(LeaderboardPotentialRankReponseErrorType error) =>
            new LeaderboardPotentialRankResponse(null, true, error);
    }
}
