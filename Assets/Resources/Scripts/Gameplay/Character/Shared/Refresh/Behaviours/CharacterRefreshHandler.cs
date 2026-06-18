namespace FlameOfTheLost
{
    public class CharacterRefreshHandler : CharacterBehaviourControllerBase
    {
        public virtual void Refresh(CharacterRefreshTarget spawnResult,  CharacterRefreshOrigin refreshOrigin)
        {
            CharacterRefreshUtility.RefreshCharacter
            (
                character: this.CharacterRootController,
                position: spawnResult.Position,
                rotation: spawnResult.Rotation,
                facing: spawnResult.Facing,
                refreshOrigin: refreshOrigin
            );
        }
    }
}
