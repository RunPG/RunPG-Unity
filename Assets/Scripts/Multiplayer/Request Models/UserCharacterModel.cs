using RunPG.Multi;

namespace RunPG.Multi
{
    public class UserCharacterModel
    {
        public int userId;
        public CharacterModel character;
        public StatisticsModel statistics;

        public UserCharacterModel(int userId, CharacterModel character, StatisticsModel statistics)
        {
            this.userId = userId;
            this.character = character;
            this.statistics = statistics;
        }
    }
}
